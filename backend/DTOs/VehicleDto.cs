using Backend.Models;

namespace Backend.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? VIN { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? Color { get; set; }
    public VehicleStatus Status { get; set; }
    public string? PhotoUrl { get; set; }

    // Registration fields
    public string? RegistrationNumber { get; set; }
    public DateTime? RegistrationIssueDate { get; set; }
    public DateTime? RegistrationExpiryDate { get; set; }
    public string? RegistrationDocumentUrl { get; set; }
    public RegistrationStatus RegistrationStatus { get; set; }

    // Owner information
    public string? OwnerName { get; set; }
    public string? OwnerAddress { get; set; }

    // Vehicle specifications
    public string? BodyType { get; set; }
    public string? EngineInfo { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int? Seats { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
