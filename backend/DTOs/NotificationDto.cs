using Backend.Models;

namespace Backend.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ReminderId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatus Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationCountDto
    {
        public int UnreadCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public Guid? ReminderId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
