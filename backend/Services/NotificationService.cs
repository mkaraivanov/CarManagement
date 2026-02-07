using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId, int limit = 50)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return notifications.Select(MapToDto);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.Status != NotificationStatus.Read)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(MapToDto);
        }

        public async Task<NotificationCountDto> GetNotificationCountAsync(Guid userId)
        {
            var total = await _context.Notifications
                .CountAsync(n => n.UserId == userId);

            var unread = await _context.Notifications
                .CountAsync(n => n.UserId == userId && n.Status != NotificationStatus.Read);

            return new NotificationCountDto
            {
                TotalCount = total,
                UnreadCount = unread
            };
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            return notification == null ? null : MapToDto(notification);
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ReminderId = dto.ReminderId,
                Type = dto.Type,
                Channel = dto.Channel,
                Status = NotificationStatus.Pending,
                Title = dto.Title,
                Message = dto.Message,
                ActionUrl = dto.ActionUrl,
                ScheduledAt = dto.ScheduledAt ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return MapToDto(notification);
        }

        public async Task<NotificationDto> CreateNotificationFromReminderAsync(Guid reminderId, NotificationChannel channel)
        {
            var reminder = await _context.Reminders
                .Include(r => r.MaintenanceSchedule)
                    .ThenInclude(s => s.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == reminderId);

            if (reminder == null)
            {
                throw new InvalidOperationException("Reminder not found");
            }

            var schedule = reminder.MaintenanceSchedule;
            var vehicle = schedule.Vehicle;

            var title = schedule.UseCompoundRule
                ? $"{schedule.TaskName} Due Soon"
                : $"{schedule.TaskName} Reminder";

            var actionUrl = $"/vehicles/{vehicle.Id}/maintenance/{schedule.Id}";

            var createDto = new CreateNotificationDto
            {
                UserId = reminder.UserId,
                ReminderId = reminderId,
                Type = NotificationType.MaintenanceDue,
                Channel = channel,
                Title = title,
                Message = reminder.Message,
                ActionUrl = actionUrl
            };

            return await CreateNotificationAsync(createDto);
        }

        public async Task<NotificationDto> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                throw new InvalidOperationException("Notification not found");
            }

            notification.Status = NotificationStatus.Read;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(notification);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.Status != NotificationStatus.Read)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var notification in unreadNotifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.ReadAt = now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return false;
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SendPendingNotificationsAsync()
        {
            var now = DateTime.UtcNow;

            var pendingNotifications = await _context.Notifications
                .Where(n => n.Status == NotificationStatus.Pending &&
                    (n.ScheduledAt == null || n.ScheduledAt <= now))
                .ToListAsync();

            foreach (var notification in pendingNotifications)
            {
                try
                {
                    // For MVP, we only support in-app notifications
                    // Email and push notifications would be sent here via providers
                    if (notification.Channel == NotificationChannel.InApp)
                    {
                        // In-app notifications are already created, just mark as sent
                        notification.Status = NotificationStatus.Sent;
                        notification.SentAt = now;
                    }
                    else if (notification.Channel == NotificationChannel.Email)
                    {
                        // TODO: Implement email provider (SendGrid, SMTP, etc.)
                        // For now, mark as sent (placeholder)
                        notification.Status = NotificationStatus.Sent;
                        notification.SentAt = now;
                    }
                    else if (notification.Channel == NotificationChannel.Push)
                    {
                        // TODO: Implement push notification provider (FCM)
                        // For now, mark as sent (placeholder)
                        notification.Status = NotificationStatus.Sent;
                        notification.SentAt = now;
                    }
                }
                catch (Exception ex)
                {
                    notification.Status = NotificationStatus.Failed;
                    notification.ErrorMessage = ex.Message;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteOldNotificationsAsync(int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

            var oldNotifications = await _context.Notifications
                .Where(n => n.CreatedAt < cutoffDate && n.Status == NotificationStatus.Read)
                .ToListAsync();

            _context.Notifications.RemoveRange(oldNotifications);
            await _context.SaveChangesAsync();
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                ReminderId = notification.ReminderId,
                Type = notification.Type,
                Channel = notification.Channel,
                Status = notification.Status,
                Title = notification.Title,
                Message = notification.Message,
                ActionUrl = notification.ActionUrl,
                ScheduledAt = notification.ScheduledAt,
                SentAt = notification.SentAt,
                ReadAt = notification.ReadAt,
                ErrorMessage = notification.ErrorMessage,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
