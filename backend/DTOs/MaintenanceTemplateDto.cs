namespace Backend.DTOs
{
    public class MaintenanceTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsSystemTemplate { get; set; }
        public Guid? UserId { get; set; }
        public int? DefaultIntervalMonths { get; set; }
        public int? DefaultIntervalKilometers { get; set; }
        public decimal? DefaultIntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMaintenanceTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int? DefaultIntervalMonths { get; set; }
        public int? DefaultIntervalKilometers { get; set; }
        public decimal? DefaultIntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }
    }

    public class UpdateMaintenanceTemplateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? DefaultIntervalMonths { get; set; }
        public int? DefaultIntervalKilometers { get; set; }
        public decimal? DefaultIntervalHours { get; set; }
        public bool? UseCompoundRule { get; set; }
    }
}
