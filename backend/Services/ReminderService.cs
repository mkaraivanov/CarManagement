using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMaintenanceCalculationService _calculationService;

        public ReminderService(
            ApplicationDbContext context,
            IMaintenanceCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        public async Task<IEnumerable<ReminderDto>> GetRemindersForUserAsync(Guid userId)
        {
            var reminders = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reminders.Select(MapToDto);
        }

        public async Task<IEnumerable<ReminderDto>> GetPendingRemindersAsync(Guid userId)
        {
            var reminders = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .Where(r => r.UserId == userId && r.Status == ReminderStatus.Pending)
                .OrderBy(r => r.ScheduledDate)
                .ToListAsync();

            return reminders.Select(MapToDto);
        }

        public async Task<ReminderDto?> GetReminderByIdAsync(Guid reminderId, Guid userId)
        {
            var reminder = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            return reminder == null ? null : MapToDto(reminder);
        }

        public async Task<ReminderDto> CreateReminderAsync(
            Guid maintenanceScheduleId,
            Guid userId,
            ReminderType type,
            string message,
            DateTime? scheduledDate = null)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == maintenanceScheduleId);

            if (schedule == null)
            {
                throw new InvalidOperationException("Maintenance schedule not found");
            }

            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                MaintenanceScheduleId = maintenanceScheduleId,
                UserId = userId,
                Status = ReminderStatus.Pending,
                Type = type,
                Message = message,
                ScheduledDate = scheduledDate ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            reminder.MaintenanceSchedule = schedule;

            return MapToDto(reminder);
        }

        public async Task MarkReminderAsSentAsync(Guid reminderId)
        {
            var reminder = await _context.Reminders.FindAsync(reminderId);
            if (reminder != null)
            {
                reminder.Status = ReminderStatus.Sent;
                reminder.SentDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ReminderDto> DismissReminderAsync(Guid reminderId, Guid userId)
        {
            var reminder = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            if (reminder == null)
            {
                throw new InvalidOperationException("Reminder not found");
            }

            reminder.Status = ReminderStatus.Dismissed;
            reminder.DismissedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(reminder);
        }

        public async Task<ReminderDto> CompleteReminderAsync(Guid reminderId, Guid userId)
        {
            var reminder = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            if (reminder == null)
            {
                throw new InvalidOperationException("Reminder not found");
            }

            reminder.Status = ReminderStatus.Completed;
            await _context.SaveChangesAsync();

            return MapToDto(reminder);
        }

        public async Task CheckAndCreateRemindersAsync()
        {
            var now = DateTime.UtcNow;

            // Get all active schedules
            var schedules = await _context.MaintenanceSchedules
                .Include(s => s.Vehicle)
                    .ThenInclude(v => v.User)
                .Include(s => s.Reminders)
                .Where(s => s.IsActive)
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                var vehicle = schedule.Vehicle;
                var userId = vehicle.UserId;

                // Skip if there's already a pending or sent reminder for this schedule
                var hasActiveReminder = schedule.Reminders.Any(r =>
                    r.Status == ReminderStatus.Pending || r.Status == ReminderStatus.Sent);

                if (hasActiveReminder)
                {
                    continue;
                }

                // Check if maintenance is overdue or upcoming
                var isOverdue = _calculationService.IsOverdue(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);
                var isUpcoming = _calculationService.IsUpcoming(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);

                if (isOverdue || isUpcoming)
                {
                    var remaining = _calculationService.CalculateRemaining(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);
                    var reminderType = DetermineReminderType(schedule, remaining);
                    var message = BuildReminderMessage(schedule, vehicle, remaining, isOverdue);

                    await CreateReminderAsync(schedule.Id, userId, reminderType, message, now);
                }
            }
        }

        public async Task DeleteOldRemindersAsync(int daysOld = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

            var oldReminders = await _context.Reminders
                .Where(r => r.CreatedAt < cutoffDate &&
                    (r.Status == ReminderStatus.Completed || r.Status == ReminderStatus.Dismissed))
                .ToListAsync();

            _context.Reminders.RemoveRange(oldReminders);
            await _context.SaveChangesAsync();
        }

        private ReminderType DetermineReminderType(MaintenanceSchedule schedule, MaintenanceRemainingResult remaining)
        {
            // Determine which interval type will trigger first
            if (schedule.UseCompoundRule)
            {
                // For compound rules, find which will trigger soonest
                var daysTrigger = remaining.DaysRemaining.HasValue && remaining.DaysRemaining.Value > 0;
                var kmTrigger = remaining.KilometersRemaining.HasValue && remaining.KilometersRemaining.Value > 0;
                var hoursTrigger = remaining.HoursRemaining.HasValue && remaining.HoursRemaining.Value > 0;

                if (daysTrigger && (!kmTrigger || remaining.DaysRemaining <= 30))
                {
                    return ReminderType.TimeBased;
                }
                else if (kmTrigger)
                {
                    return ReminderType.MileageBased;
                }
                else if (hoursTrigger)
                {
                    return ReminderType.HoursBased;
                }

                return ReminderType.Compound;
            }
            else
            {
                // For non-compound rules, return the primary type
                if (schedule.IntervalMonths.HasValue)
                {
                    return ReminderType.TimeBased;
                }
                else if (schedule.IntervalKilometers.HasValue)
                {
                    return ReminderType.MileageBased;
                }
                else if (schedule.IntervalHours.HasValue)
                {
                    return ReminderType.HoursBased;
                }

                return ReminderType.Compound;
            }
        }

        private string BuildReminderMessage(
            MaintenanceSchedule schedule,
            Vehicle vehicle,
            MaintenanceRemainingResult remaining,
            bool isOverdue)
        {
            var vehicleInfo = $"{vehicle.Year} {vehicle.Make} {vehicle.Model}";

            if (isOverdue)
            {
                return $"{schedule.TaskName} is overdue for {vehicleInfo}";
            }

            var parts = new List<string>();

            if (remaining.DaysRemaining.HasValue && remaining.DaysRemaining.Value > 0)
            {
                parts.Add($"{remaining.DaysRemaining} days");
            }

            if (remaining.KilometersRemaining.HasValue && remaining.KilometersRemaining.Value > 0)
            {
                parts.Add($"{remaining.KilometersRemaining:N0} km");
            }

            if (remaining.HoursRemaining.HasValue && remaining.HoursRemaining.Value > 0)
            {
                parts.Add($"{remaining.HoursRemaining:N1} hours");
            }

            var remainingText = parts.Any() ? string.Join(" or ", parts) : "soon";

            return $"{schedule.TaskName} due in {remainingText} for {vehicleInfo}";
        }

        private ReminderDto MapToDto(Reminder reminder)
        {
            return new ReminderDto
            {
                Id = reminder.Id,
                MaintenanceScheduleId = reminder.MaintenanceScheduleId,
                UserId = reminder.UserId,
                Status = reminder.Status,
                Type = reminder.Type,
                ScheduledDate = reminder.ScheduledDate,
                SentDate = reminder.SentDate,
                DismissedDate = reminder.DismissedDate,
                Message = reminder.Message,
                CreatedAt = reminder.CreatedAt,
                TaskName = reminder.MaintenanceSchedule?.TaskName,
                VehicleMake = reminder.MaintenanceSchedule?.Vehicle?.Make,
                VehicleModel = reminder.MaintenanceSchedule?.Vehicle?.Model,
                VehicleYear = reminder.MaintenanceSchedule?.Vehicle?.Year
            };
        }
    }
}
