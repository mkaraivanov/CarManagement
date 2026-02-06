using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs;

public class UpdateServiceRecordRequest
{
    public DateTime? ServiceDate { get; set; }

    [Range(0, int.MaxValue)]
    public int? MileageAtService { get; set; }

    public ServiceType? ServiceType { get; set; }

    [MaxLength(100)]
    public string? ServiceCenter { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Cost { get; set; }

    public DateTime? NextServiceDue { get; set; }

    [Range(0, int.MaxValue)]
    public int? NextServiceMileage { get; set; }

    [MaxLength(500)]
    public string? ReceiptUrl { get; set; }
}
