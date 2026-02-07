using Backend.Models;

namespace Backend.DTOs
{
    public class ReminderDto
    {
        public Guid Id { get; set; }
        public Guid MaintenanceScheduleId { get; set; }
        public Guid UserId { get; set; }
        public ReminderStatus Status { get; set; }
        public ReminderType Type { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? DismissedDate { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Additional fields for detailed view
        public string? TaskName { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
    }
}
