using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs;

public class CreateServiceRecordRequest
{
    [Required]
    public DateTime ServiceDate { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MileageAtService { get; set; }

    [Required]
    public ServiceType ServiceType { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceCenter { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    public DateTime? NextServiceDue { get; set; }

    [Range(0, int.MaxValue)]
    public int? NextServiceMileage { get; set; }

    [MaxLength(500)]
    public string? ReceiptUrl { get; set; }
}
