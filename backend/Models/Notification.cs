using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ReminderId { get; set; }

        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Reminder? Reminder { get; set; }
    }
}
