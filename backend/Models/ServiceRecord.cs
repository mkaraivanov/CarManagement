using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public enum ServiceType
{
    OilChange,
    TireRotation,
    BrakeService,
    Inspection,
    General,
    Other
}

public class ServiceRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey(nameof(Vehicle))]
    public Guid VehicleId { get; set; }

    [Required]
    public DateTime ServiceDate { get; set; }

    [Required]
    public int MileageAtService { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? EngineHoursAtService { get; set; }

    [Required]
    public ServiceType ServiceType { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceCenter { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Cost { get; set; }

    public DateTime? NextServiceDue { get; set; }

    public int? NextServiceMileage { get; set; }

    [MaxLength(500)]
    public string? ReceiptUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Vehicle Vehicle { get; set; } = null!;
}
