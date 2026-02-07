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

    // Registration fields
    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    public DateTime? RegistrationIssueDate { get; set; }

    public DateTime? RegistrationExpiryDate { get; set; }

    [MaxLength(500)]
    public string? RegistrationDocumentUrl { get; set; }

    public RegistrationStatus RegistrationStatus { get; set; } = RegistrationStatus.Unknown;

    // Owner information
    [MaxLength(100)]
    public string? OwnerName { get; set; }

    [MaxLength(300)]
    public string? OwnerAddress { get; set; }

    // Vehicle specifications
    [MaxLength(50)]
    public string? BodyType { get; set; }

    [MaxLength(100)]
    public string? EngineInfo { get; set; }

    [MaxLength(30)]
    public string? FuelType { get; set; }

    [MaxLength(30)]
    public string? Transmission { get; set; }

    [Range(1, 20)]
    public int? Seats { get; set; }
}
