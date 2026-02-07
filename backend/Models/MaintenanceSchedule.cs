using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class MaintenanceSchedule
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid? TemplateId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TaskName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        // Interval Configuration
        public int? IntervalMonths { get; set; }
        public int? IntervalKilometers { get; set; }
        public decimal? IntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }

        // Last Completion Tracking
        public DateTime? LastCompletedDate { get; set; }
        public int? LastCompletedMileage { get; set; }
        public decimal? LastCompletedHours { get; set; }
        public Guid? LastServiceRecordId { get; set; }

        // Next Due Calculation (computed)
        public DateTime? NextDueDate { get; set; }
        public int? NextDueMileage { get; set; }
        public decimal? NextDueHours { get; set; }

        // Reminder Settings
        public int ReminderDaysBefore { get; set; } = 30;
        public int ReminderKilometersBefore { get; set; } = 1000;
        public decimal ReminderHoursBefore { get; set; } = 10;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public Vehicle Vehicle { get; set; } = null!;
        public MaintenanceTemplate? Template { get; set; }
        public ServiceRecord? LastServiceRecord { get; set; }
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
