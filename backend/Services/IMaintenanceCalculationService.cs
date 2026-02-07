using Backend.Models;

namespace Backend.Services
{
    public interface IMaintenanceCalculationService
    {
        /// <summary>
        /// Calculate next due date, mileage, and hours for a maintenance schedule
        /// </summary>
        MaintenanceCalculationResult CalculateNextDue(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours);

        /// <summary>
        /// Calculate remaining time, mileage, and hours until maintenance is due
        /// </summary>
        MaintenanceRemainingResult CalculateRemaining(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours);

        /// <summary>
        /// Determine if maintenance is overdue
        /// </summary>
        bool IsOverdue(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours);

        /// <summary>
        /// Determine if maintenance is upcoming (within reminder threshold)
        /// </summary>
        bool IsUpcoming(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours);

        /// <summary>
        /// Get maintenance status string ("Overdue", "Due Soon", "OK")
        /// </summary>
        string GetMaintenanceStatus(
            MaintenanceSchedule schedule,
            int currentVehicleMileage,
            decimal? currentVehicleHours);
    }

    public class MaintenanceCalculationResult
    {
        public DateTime? NextDueDate { get; set; }
        public int? NextDueMileage { get; set; }
        public decimal? NextDueHours { get; set; }
    }

    public class MaintenanceRemainingResult
    {
        public int? DaysRemaining { get; set; }
        public int? KilometersRemaining { get; set; }
        public decimal? HoursRemaining { get; set; }
        public bool IsOverdue { get; set; }
    }
}
