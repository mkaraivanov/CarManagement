using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Reminder
    {
        public Guid Id { get; set; }
        public Guid MaintenanceScheduleId { get; set; }
        public Guid UserId { get; set; }

        public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
        public ReminderType Type { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? DismissedDate { get; set; }

        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public MaintenanceSchedule MaintenanceSchedule { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
