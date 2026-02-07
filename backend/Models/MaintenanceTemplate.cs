using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class MaintenanceTemplate
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public bool IsSystemTemplate { get; set; }
        public Guid? UserId { get; set; }

        // Default intervals (can be overridden per vehicle)
        public int? DefaultIntervalMonths { get; set; }
        public int? DefaultIntervalKilometers { get; set; }
        public decimal? DefaultIntervalHours { get; set; }
        public bool UseCompoundRule { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
    }
}
