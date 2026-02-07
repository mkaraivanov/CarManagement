using Backend.DTOs;

namespace Backend.Services
{
    public interface IMaintenanceScheduleService
    {
        /// <summary>
        /// Get all maintenance schedules for a vehicle
        /// </summary>
        Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetSchedulesForVehicleAsync(Guid vehicleId, Guid userId);

        /// <summary>
        /// Get a single schedule by ID
        /// </summary>
        Task<MaintenanceScheduleDetailsDto?> GetScheduleByIdAsync(Guid scheduleId, Guid userId);

        /// <summary>
        /// Create a maintenance schedule (from template or custom)
        /// </summary>
        Task<MaintenanceScheduleDetailsDto> CreateScheduleAsync(Guid userId, CreateMaintenanceScheduleDto dto);

        /// <summary>
        /// Update a maintenance schedule
        /// </summary>
        Task<MaintenanceScheduleDetailsDto> UpdateScheduleAsync(Guid scheduleId, Guid userId, UpdateMaintenanceScheduleDto dto);

        /// <summary>
        /// Delete a maintenance schedule
        /// </summary>
        Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId);

        /// <summary>
        /// Mark a schedule as completed and update next due dates
        /// </summary>
        Task<MaintenanceScheduleDetailsDto> CompleteScheduleAsync(Guid scheduleId, Guid userId, CompleteMaintenanceScheduleDto dto);

        /// <summary>
        /// Link an existing service record to a schedule
        /// </summary>
        Task<MaintenanceScheduleDetailsDto> LinkServiceRecordAsync(Guid scheduleId, Guid userId, Guid serviceRecordId);

        /// <summary>
        /// Get all overdue schedules for a user
        /// </summary>
        Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetOverdueSchedulesAsync(Guid userId);

        /// <summary>
        /// Get all upcoming schedules for a user (within reminder threshold)
        /// </summary>
        Task<IEnumerable<MaintenanceScheduleDetailsDto>> GetUpcomingSchedulesAsync(Guid userId);

        /// <summary>
        /// Recalculate next due dates for a schedule
        /// </summary>
        Task RecalculateNextDueAsync(Guid scheduleId);

        /// <summary>
        /// Recalculate next due dates for all schedules of a vehicle
        /// </summary>
        Task RecalculateNextDueForVehicleAsync(Guid vehicleId);
    }
}
