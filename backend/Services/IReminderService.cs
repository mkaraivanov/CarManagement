using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public interface IReminderService
    {
        /// <summary>
        /// Get all reminders for a user
        /// </summary>
        Task<IEnumerable<ReminderDto>> GetRemindersForUserAsync(Guid userId);

        /// <summary>
        /// Get pending reminders for a user
        /// </summary>
        Task<IEnumerable<ReminderDto>> GetPendingRemindersAsync(Guid userId);

        /// <summary>
        /// Get a single reminder by ID
        /// </summary>
        Task<ReminderDto?> GetReminderByIdAsync(Guid reminderId, Guid userId);

        /// <summary>
        /// Create a reminder for a maintenance schedule
        /// </summary>
        Task<ReminderDto> CreateReminderAsync(Guid maintenanceScheduleId, Guid userId, ReminderType type, string message, DateTime? scheduledDate = null);

        /// <summary>
        /// Mark reminder as sent
        /// </summary>
        Task MarkReminderAsSentAsync(Guid reminderId);

        /// <summary>
        /// Dismiss a reminder
        /// </summary>
        Task<ReminderDto> DismissReminderAsync(Guid reminderId, Guid userId);

        /// <summary>
        /// Mark reminder as completed
        /// </summary>
        Task<ReminderDto> CompleteReminderAsync(Guid reminderId, Guid userId);

        /// <summary>
        /// Check for maintenance schedules that need reminders and create them
        /// </summary>
        Task CheckAndCreateRemindersAsync();

        /// <summary>
        /// Delete old reminders (cleanup)
        /// </summary>
        Task DeleteOldRemindersAsync(int daysOld = 90);
    }
}
