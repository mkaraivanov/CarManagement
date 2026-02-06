using Backend.Models;

namespace Backend.DTOs;

public class ServiceRecordDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime ServiceDate { get; set; }
    public int MileageAtService { get; set; }
    public ServiceType ServiceType { get; set; }
    public string ServiceCenter { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime? NextServiceDue { get; set; }
    public int? NextServiceMileage { get; set; }
    public string? ReceiptUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
