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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
