using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Get all notifications for a user
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId, int limit = 50);

        /// <summary>
        /// Get unread notifications for a user
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(Guid userId);

        /// <summary>
        /// Get notification count for a user
        /// </summary>
        Task<NotificationCountDto> GetNotificationCountAsync(Guid userId);

        /// <summary>
        /// Get a single notification by ID
        /// </summary>
        Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Create a notification
        /// </summary>
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);

        /// <summary>
        /// Create notification from reminder
        /// </summary>
        Task<NotificationDto> CreateNotificationFromReminderAsync(Guid reminderId, NotificationChannel channel);

        /// <summary>
        /// Mark notification as read
        /// </summary>
        Task<NotificationDto> MarkAsReadAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Mark all notifications as read for a user
        /// </summary>
        Task MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Delete a notification
        /// </summary>
        Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Send pending notifications (called by background service)
        /// </summary>
        Task SendPendingNotificationsAsync();

        /// <summary>
        /// Delete old read notifications (cleanup)
        /// </summary>
        Task DeleteOldNotificationsAsync(int daysOld = 30);
    }
}
