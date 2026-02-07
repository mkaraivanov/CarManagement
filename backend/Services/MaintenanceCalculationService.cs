using Backend.Models;

namespace Backend.Services
{
    public class MaintenanceCalculationService : IMaintenanceCalculationService
    {
        public MaintenanceCalculationResult CalculateNextDue(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours)
        {
            var result = new MaintenanceCalculationResult();

            // Calculate next due date (time-based)
            if (schedule.IntervalMonths.HasValue && schedule.LastCompletedDate.HasValue)
            {
                result.NextDueDate = schedule.LastCompletedDate.Value.AddMonths(schedule.IntervalMonths.Value);
            }

            // Calculate next due mileage (mileage-based)
            if (schedule.IntervalKilometers.HasValue && schedule.LastCompletedMileage.HasValue)
            {
                result.NextDueMileage = schedule.LastCompletedMileage.Value + schedule.IntervalKilometers.Value;
            }

            // Calculate next due hours (hours-based)
            if (schedule.IntervalHours.HasValue && schedule.LastCompletedHours.HasValue)
            {
                result.NextDueHours = schedule.LastCompletedHours.Value + schedule.IntervalHours.Value;
            }

            return result;
        }

        public MaintenanceRemainingResult CalculateRemaining(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours)
        {
            var result = new MaintenanceRemainingResult();
            var now = DateTime.UtcNow;

            // Calculate days remaining
            if (schedule.NextDueDate.HasValue)
            {
                var timeSpan = schedule.NextDueDate.Value - now;
                result.DaysRemaining = (int)Math.Ceiling(timeSpan.TotalDays);
            }

            // Calculate kilometers remaining
            if (schedule.NextDueMileage.HasValue)
            {
                result.KilometersRemaining = schedule.NextDueMileage.Value - currentVehicleMileage;
            }

            // Calculate hours remaining
            if (schedule.NextDueHours.HasValue && currentVehicleHours.HasValue)
            {
                result.HoursRemaining = schedule.NextDueHours.Value - currentVehicleHours.Value;
            }

            // Determine if overdue based on compound rules
            result.IsOverdue = IsOverdue(schedule, currentVehicleMileage, currentVehicleHours);

            return result;
        }

        public bool IsOverdue(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours)
        {
            var now = DateTime.UtcNow;
            bool isOverdue = false;

            if (schedule.UseCompoundRule)
            {
                // OR logic: overdue if ANY condition is met
                if (schedule.NextDueDate.HasValue && now > schedule.NextDueDate.Value)
                {
                    isOverdue = true;
                }
                else if (schedule.NextDueMileage.HasValue && currentVehicleMileage > schedule.NextDueMileage.Value)
                {
                    isOverdue = true;
                }
                else if (schedule.NextDueHours.HasValue && currentVehicleHours.HasValue
                    && currentVehicleHours.Value > schedule.NextDueHours.Value)
                {
                    isOverdue = true;
                }
            }
            else
            {
                // AND logic: overdue only if ALL conditions are met
                bool allConditionsMet = true;
                bool hasAnyCondition = false;

                if (schedule.NextDueDate.HasValue)
                {
                    hasAnyCondition = true;
                    if (now <= schedule.NextDueDate.Value)
                    {
                        allConditionsMet = false;
                    }
                }

                if (schedule.NextDueMileage.HasValue)
                {
                    hasAnyCondition = true;
                    if (currentVehicleMileage <= schedule.NextDueMileage.Value)
                    {
                        allConditionsMet = false;
                    }
                }

                if (schedule.NextDueHours.HasValue && currentVehicleHours.HasValue)
                {
                    hasAnyCondition = true;
                    if (currentVehicleHours.Value <= schedule.NextDueHours.Value)
                    {
                        allConditionsMet = false;
                    }
                }

                isOverdue = hasAnyCondition && allConditionsMet;
            }

            return isOverdue;
        }

        public bool IsUpcoming(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours)
        {
            var remaining = CalculateRemaining(schedule, currentVehicleMileage, currentVehicleHours);

            // Check if within reminder threshold for any interval
            bool isUpcoming = false;

            if (remaining.DaysRemaining.HasValue && remaining.DaysRemaining.Value <= schedule.ReminderDaysBefore && remaining.DaysRemaining.Value > 0)
            {
                isUpcoming = true;
            }

            if (remaining.KilometersRemaining.HasValue && remaining.KilometersRemaining.Value <= schedule.ReminderKilometersBefore && remaining.KilometersRemaining.Value > 0)
            {
                isUpcoming = true;
            }

            if (remaining.HoursRemaining.HasValue && remaining.HoursRemaining.Value <= schedule.ReminderHoursBefore && remaining.HoursRemaining.Value > 0)
            {
                isUpcoming = true;
            }

            return isUpcoming;
        }

        public string GetMaintenanceStatus(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours)
        {
            if (IsOverdue(schedule, currentVehicleMileage, currentVehicleHours))
            {
                return "Overdue";
            }
            else if (IsUpcoming(schedule, currentVehicleMileage, currentVehicleHours))
            {
                return "Due Soon";
            }
            else
            {
                return "OK";
            }
        }
    }
}
