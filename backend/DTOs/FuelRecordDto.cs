using Backend.Models;

namespace Backend.DTOs;

public class FuelRecordDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime RefuelDate { get; set; }
    public int Mileage { get; set; }
    public decimal Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalCost { get; set; }
    public FuelType FuelType { get; set; }
    public string? GasStation { get; set; }
    public decimal? FuelEfficiency { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
