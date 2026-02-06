using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public enum VehicleStatus
{
    Active,
    Sold,
    Inactive
}

public class Vehicle
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public int Year { get; set; }

    [MaxLength(17)]
    public string? VIN { get; set; }

    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    public int CurrentMileage { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [MaxLength(30)]
    public string? Color { get; set; }

    [Required]
    public VehicleStatus Status { get; set; } = VehicleStatus.Active;

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
    public virtual ICollection<FuelRecord> FuelRecords { get; set; } = new List<FuelRecord>();
}
