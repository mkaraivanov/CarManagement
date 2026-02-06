using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public enum FuelType
{
    Regular,
    Premium,
    Diesel,
    Electric
}

public class FuelRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey(nameof(Vehicle))]
    public Guid VehicleId { get; set; }

    [Required]
    public DateTime RefuelDate { get; set; }

    [Required]
    public int Mileage { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Quantity { get; set; }  // liters or gallons

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal PricePerUnit { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalCost { get; set; }

    [Required]
    public FuelType FuelType { get; set; }

    [MaxLength(100)]
    public string? GasStation { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? FuelEfficiency { get; set; }  // MPG or L/100km

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Vehicle Vehicle { get; set; } = null!;
}
