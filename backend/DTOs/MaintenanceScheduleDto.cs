namespace Backend.DTOs
{
    public class MaintenanceScheduleDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid? TemplateId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? IntervalMonths { get; set; }
        public int? IntervalKilometers { get; set; }
        public decimal? IntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }
        public DateTime? LastCompletedDate { get; set; }
        public int? LastCompletedMileage { get; set; }
        public decimal? LastCompletedHours { get; set; }
        public Guid? LastServiceRecordId { get; set; }
        public DateTime? NextDueDate { get; set; }
        public int? NextDueMileage { get; set; }
        public decimal? NextDueHours { get; set; }
        public int ReminderDaysBefore { get; set; }
        public int ReminderKilometersBefore { get; set; }
        public decimal ReminderHoursBefore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MaintenanceScheduleDetailsDto : MaintenanceScheduleDto
    {
        // Additional calculated fields
        public int? DaysUntilDue { get; set; }
        public int? KilometersUntilDue { get; set; }
        public decimal? HoursUntilDue { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsUpcoming { get; set; }
        public string Status { get; set; } = string.Empty; // "Overdue", "Due Soon", "OK"
        public string? TemplateName { get; set; }
    }

    public class CreateMaintenanceScheduleDto
    {
        public Guid VehicleId { get; set; }
        public Guid? TemplateId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? IntervalMonths { get; set; }
        public int? IntervalKilometers { get; set; }
        public decimal? IntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }
        public DateTime? LastCompletedDate { get; set; }
        public int? LastCompletedMileage { get; set; }
        public decimal? LastCompletedHours { get; set; }
        public int ReminderDaysBefore { get; set; } = 30;
        public int ReminderKilometersBefore { get; set; } = 1000;
        public decimal ReminderHoursBefore { get; set; } = 10;
    }

    public class UpdateMaintenanceScheduleDto
    {
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? IntervalMonths { get; set; }
        public int? IntervalKilometers { get; set; }
        public decimal? IntervalHours { get; set; }
        public bool? UseCompoundRule { get; set; }
        public int? ReminderDaysBefore { get; set; }
        public int? ReminderKilometersBefore { get; set; }
        public decimal? ReminderHoursBefore { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CompleteMaintenanceScheduleDto
    {
        public DateTime CompletedDate { get; set; }
        public int CompletedMileage { get; set; }
        public decimal? CompletedHours { get; set; }
        public Guid? ServiceRecordId { get; set; }
    }
}
