using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class MaintenanceScheduleService : IMaintenanceScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMaintenanceCalculationService _calculationService;

        public MaintenanceScheduleService(
            ApplicationDbContext context,
            IMaintenanceCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        public async Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetSchedulesForVehicleAsync(Guid vehicleId, Guid userId)
        {
            // Verify vehicle belongs to user
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            var schedules = await _context.MaintenanceSchedules
                .Include(s => s.Template)
                .Where(s => s.VehicleId == vehicleId)
                .OrderBy(s => s.NextDueDate)
                .ToListAsync();

            return schedules.Select(s => MapToDetailsDto(s, vehicle.CurrentMileage, vehicle.CurrentEngineHours));
        }

        public async Task<MaintenanceScheduleDetailsDto?> GetScheduleByIdAsync(Guid scheduleId, Guid userId)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Template)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Vehicle.UserId == userId);

            if (schedule == null)
            {
                return null;
            }

            return MapToDetailsDto(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
        }

        public async Task<MaintenanceScheduleDetailsDto> CreateScheduleAsync(Guid userId, CreateMaintenanceScheduleDto dto)
        {
            // Verify vehicle belongs to user
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && v.UserId == userId);

            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            // Get template if specified
            MaintenanceTemplate? template = null;
            if (dto.TemplateId.HasValue)
            {
                template = await _context.MaintenanceTemplates
                    .FirstOrDefaultAsync(t => t.Id == dto.TemplateId.Value);

                if (template == null)
                {
                    throw new InvalidOperationException("Template not found");
                }
            }

            var schedule = new MaintenanceSchedule
            {
                Id = Guid.NewGuid(),
                VehicleId = dto.VehicleId,
                TemplateId = dto.TemplateId,
                TaskName = dto.TaskName,
                Description = dto.Description,
                Category = dto.Category ?? template?.Category,
                IntervalMonths = dto.IntervalMonths ?? template?.DefaultIntervalMonths,
                IntervalKilometers = dto.IntervalKilometers ?? template?.DefaultIntervalKilometers,
                IntervalHours = dto.IntervalHours ?? template?.DefaultIntervalHours,
                UseCompoundRule = dto.UseCompoundRule,
                LastCompletedDate = dto.LastCompletedDate,
                LastCompletedMileage = dto.LastCompletedMileage,
                LastCompletedHours = dto.LastCompletedHours,
                ReminderDaysBefore = dto.ReminderDaysBefore,
                ReminderKilometersBefore = dto.ReminderKilometersBefore,
                ReminderHoursBefore = dto.ReminderHoursBefore,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Calculate next due dates
            var nextDue = _calculationService.CalculateNextDue(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);
            schedule.NextDueDate = nextDue.NextDueDate;
            schedule.NextDueMileage = nextDue.NextDueMileage;
            schedule.NextDueHours = nextDue.NextDueHours;

            _context.MaintenanceSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Reload with template for response
            schedule.Template = template;

            return MapToDetailsDto(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);
        }

        public async Task<MaintenanceScheduleDetailsDto> UpdateScheduleAsync(Guid scheduleId, Guid userId, UpdateMaintenanceScheduleDto dto)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Template)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Vehicle.UserId == userId);

            if (schedule == null)
            {
                throw new InvalidOperationException("Schedule not found");
            }

            if (dto.TaskName != null) schedule.TaskName = dto.TaskName;
            if (dto.Description != null) schedule.Description = dto.Description;
            if (dto.Category != null) schedule.Category = dto.Category;
            if (dto.IntervalMonths.HasValue) schedule.IntervalMonths = dto.IntervalMonths;
            if (dto.IntervalKilometers.HasValue) schedule.IntervalKilometers = dto.IntervalKilometers;
            if (dto.IntervalHours.HasValue) schedule.IntervalHours = dto.IntervalHours;
            if (dto.UseCompoundRule.HasValue) schedule.UseCompoundRule = dto.UseCompoundRule.Value;
            if (dto.ReminderDaysBefore.HasValue) schedule.ReminderDaysBefore = dto.ReminderDaysBefore.Value;
            if (dto.ReminderKilometersBefore.HasValue) schedule.ReminderKilometersBefore = dto.ReminderKilometersBefore.Value;
            if (dto.ReminderHoursBefore.HasValue) schedule.ReminderHoursBefore = dto.ReminderHoursBefore.Value;
            if (dto.IsActive.HasValue) schedule.IsActive = dto.IsActive.Value;

            schedule.UpdatedAt = DateTime.UtcNow;

            // Recalculate next due dates if intervals changed
            if (dto.IntervalMonths.HasValue || dto.IntervalKilometers.HasValue || dto.IntervalHours.HasValue)
            {
                var nextDue = _calculationService.CalculateNextDue(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
                schedule.NextDueDate = nextDue.NextDueDate;
                schedule.NextDueMileage = nextDue.NextDueMileage;
                schedule.NextDueHours = nextDue.NextDueHours;
            }

            await _context.SaveChangesAsync();

            return MapToDetailsDto(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
        }

        public async Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Vehicle.UserId == userId);

            if (schedule == null)
            {
                return false;
            }

            _context.MaintenanceSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<MaintenanceScheduleDetailsDto> CompleteScheduleAsync(Guid scheduleId, Guid userId, CompleteMaintenanceScheduleDto dto)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Template)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Vehicle.UserId == userId);

            if (schedule == null)
            {
                throw new InvalidOperationException("Schedule not found");
            }

            // Update last completed information
            schedule.LastCompletedDate = dto.CompletedDate;
            schedule.LastCompletedMileage = dto.CompletedMileage;
            schedule.LastCompletedHours = dto.CompletedHours;
            schedule.LastServiceRecordId = dto.ServiceRecordId;
            schedule.UpdatedAt = DateTime.UtcNow;

            // Recalculate next due dates
            var nextDue = _calculationService.CalculateNextDue(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
            schedule.NextDueDate = nextDue.NextDueDate;
            schedule.NextDueMileage = nextDue.NextDueMileage;
            schedule.NextDueHours = nextDue.NextDueHours;

            await _context.SaveChangesAsync();

            return MapToDetailsDto(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
        }

        public async Task<MaintenanceScheduleDetailsDto> LinkServiceRecordAsync(Guid scheduleId, Guid userId, Guid serviceRecordId)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Template)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Vehicle.UserId == userId);

            if (schedule == null)
            {
                throw new InvalidOperationException("Schedule not found");
            }

            // Verify service record exists and belongs to same vehicle
            var serviceRecord = await _context.ServiceRecords
                .FirstOrDefaultAsync(sr => sr.Id == serviceRecordId && sr.VehicleId == schedule.VehicleId);

            if (serviceRecord == null)
            {
                throw new InvalidOperationException("Service record not found");
            }

            // Complete the schedule with service record data
            var completeDto = new CompleteMaintenanceScheduleDto
            {
                CompletedDate = serviceRecord.ServiceDate,
                CompletedMileage = serviceRecord.MileageAtService,
                CompletedHours = serviceRecord.EngineHoursAtService,
                ServiceRecordId = serviceRecordId
            };

            return await CompleteScheduleAsync(scheduleId, userId, completeDto);
        }

        public async Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetOverdueSchedulesAsync(Guid userId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == userId)
                .Include(v => v.MaintenanceSchedules)
                .ThenInclude(s => s.Template)
                .ToListAsync();

            var overdueSchedules = new List<MaintenanceScheduleDetailsDto>();

            foreach (var vehicle in vehicles)
            {
                foreach (var schedule in vehicle.MaintenanceSchedules.Where(s => s.IsActive))
                {
                    if (_calculationService.IsOverdue(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours))
                    {
                        overdueSchedules.Add(MapToDetailsDto(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours));
                    }
                }
            }

            return overdueSchedules.OrderBy(s => s.NextDueDate);
        }

        public async Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetUpcomingSchedulesAsync(Guid userId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == userId)
                .Include(v => v.MaintenanceSchedules)
                .ThenInclude(s => s.Template)
                .ToListAsync();

            var upcomingSchedules = new List<MaintenanceScheduleDetailsDto>();

            foreach (var vehicle in vehicles)
            {
                foreach (var schedule in vehicle.MaintenanceSchedules.Where(s => s.IsActive))
                {
                    if (_calculationService.IsUpcoming(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours))
                    {
                        upcomingSchedules.Add(MapToDetailsDto(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours));
                    }
                }
            }

            return upcomingSchedules.OrderBy(s => s.NextDueDate);
        }

        public async Task RecalculateNextDueAsync(Guid scheduleId)
        {
            var schedule = await _context.MaintenanceSchedules
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
            {
                return;
            }

            var nextDue = _calculationService.CalculateNextDue(schedule, schedule.Vehicle.CurrentMileage, schedule.Vehicle.CurrentEngineHours);
            schedule.NextDueDate = nextDue.NextDueDate;
            schedule.NextDueMileage = nextDue.NextDueMileage;
            schedule.NextDueHours = nextDue.NextDueHours;

            await _context.SaveChangesAsync();
        }

        public async Task RecalculateNextDueForVehicleAsync(Guid vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.MaintenanceSchedules)
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
            {
                return;
            }

            foreach (var schedule in vehicle.MaintenanceSchedules)
            {
                var nextDue = _calculationService.CalculateNextDue(schedule, vehicle.CurrentMileage, vehicle.CurrentEngineHours);
                schedule.NextDueDate = nextDue.NextDueDate;
                schedule.NextDueMileage = nextDue.NextDueMileage;
                schedule.NextDueHours = nextDue.NextDueHours;
            }

            await _context.SaveChangesAsync();
        }

        private MaintenanceScheduleDetailsDto MapToDetailsDto(MaintenanceSchedule schedule, int currentMileage, decimal? currentHours)
        {
            var remaining = _calculationService.CalculateRemaining(schedule, currentMileage, currentHours);
            var status = _calculationService.GetMaintenanceStatus(schedule, currentMileage, currentHours);

            return new MaintenanceScheduleDetailsDto
            {
                Id = schedule.Id,
                VehicleId = schedule.VehicleId,
                TemplateId = schedule.TemplateId,
                TaskName = schedule.TaskName,
                Description = schedule.Description,
                Category = schedule.Category,
                IntervalMonths = schedule.IntervalMonths,
                IntervalKilometers = schedule.IntervalKilometers,
                IntervalHours = schedule.IntervalHours,
                UseCompoundRule = schedule.UseCompoundRule,
                LastCompletedDate = schedule.LastCompletedDate,
                LastCompletedMileage = schedule.LastCompletedMileage,
                LastCompletedHours = schedule.LastCompletedHours,
                LastServiceRecordId = schedule.LastServiceRecordId,
                NextDueDate = schedule.NextDueDate,
                NextDueMileage = schedule.NextDueMileage,
                NextDueHours = schedule.NextDueHours,
                ReminderDaysBefore = schedule.ReminderDaysBefore,
                ReminderKilometersBefore = schedule.ReminderKilometersBefore,
                ReminderHoursBefore = schedule.ReminderHoursBefore,
                IsActive = schedule.IsActive,
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt,
                DaysUntilDue = remaining.DaysRemaining,
                KilometersUntilDue = remaining.KilometersRemaining,
                HoursUntilDue = remaining.HoursRemaining,
                IsOverdue = remaining.IsOverdue,
                IsUpcoming = _calculationService.IsUpcoming(schedule, currentMileage, currentHours),
                Status = status,
                TemplateName = schedule.Template?.Name
            };
        }
    }
}
