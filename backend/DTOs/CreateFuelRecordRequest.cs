using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs;

public class CreateFuelRecordRequest
{
    [Required]
    public DateTime RefuelDate { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PricePerUnit { get; set; }

    [Required]
    public FuelType FuelType { get; set; }

    [MaxLength(100)]
    public string? GasStation { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
