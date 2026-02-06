using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs;

public class CreateVehicleRequest
{
    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [MaxLength(17)]
    public string? VIN { get; set; }

    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int CurrentMileage { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [MaxLength(30)]
    public string? Color { get; set; }

    public VehicleStatus Status { get; set; } = VehicleStatus.Active;

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }
}
