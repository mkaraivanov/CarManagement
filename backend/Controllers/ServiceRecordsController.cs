using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class ServiceRecordsController : ControllerBase
{
    private readonly IServiceRecordService _serviceRecordService;

    public ServiceRecordsController(IServiceRecordService serviceRecordService)
    {
        _serviceRecordService = serviceRecordService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("vehicles/{vehicleId}/services")]
    public async Task<IActionResult> GetVehicleServiceRecords(Guid vehicleId)
    {
        var userId = GetUserId();
        var serviceRecords = await _serviceRecordService.GetVehicleServiceRecordsAsync(vehicleId, userId);

        var serviceRecordDtos = serviceRecords.Select(sr => new ServiceRecordDto
        {
            Id = sr.Id,
            VehicleId = sr.VehicleId,
            ServiceDate = sr.ServiceDate,
            MileageAtService = sr.MileageAtService,
            ServiceType = sr.ServiceType,
            ServiceCenter = sr.ServiceCenter,
            Description = sr.Description,
            Cost = sr.Cost,
            NextServiceDue = sr.NextServiceDue,
            NextServiceMileage = sr.NextServiceMileage,
            ReceiptUrl = sr.ReceiptUrl,
            CreatedAt = sr.CreatedAt
        });

        return Ok(serviceRecordDtos);
    }

    [HttpGet("services/{id}")]
    public async Task<IActionResult> GetServiceRecord(Guid id)
    {
        var userId = GetUserId();
        var serviceRecord = await _serviceRecordService.GetServiceRecordByIdAsync(id, userId);

        if (serviceRecord == null)
            return NotFound(new { message = "Service record not found" });

        var serviceRecordDto = new ServiceRecordDto
        {
            Id = serviceRecord.Id,
            VehicleId = serviceRecord.VehicleId,
            ServiceDate = serviceRecord.ServiceDate,
            MileageAtService = serviceRecord.MileageAtService,
            ServiceType = serviceRecord.ServiceType,
            ServiceCenter = serviceRecord.ServiceCenter,
            Description = serviceRecord.Description,
            Cost = serviceRecord.Cost,
            NextServiceDue = serviceRecord.NextServiceDue,
            NextServiceMileage = serviceRecord.NextServiceMileage,
            ReceiptUrl = serviceRecord.ReceiptUrl,
            CreatedAt = serviceRecord.CreatedAt
        };

        return Ok(serviceRecordDto);
    }

    [HttpPost("vehicles/{vehicleId}/services")]
    public async Task<IActionResult> CreateServiceRecord(Guid vehicleId, [FromBody] CreateServiceRecordRequest request)
    {
        var userId = GetUserId();
        var serviceRecord = await _serviceRecordService.CreateServiceRecordAsync(vehicleId, request, userId);

        if (serviceRecord == null)
            return NotFound(new { message = "Vehicle not found" });

        var serviceRecordDto = new ServiceRecordDto
        {
            Id = serviceRecord.Id,
            VehicleId = serviceRecord.VehicleId,
            ServiceDate = serviceRecord.ServiceDate,
            MileageAtService = serviceRecord.MileageAtService,
            ServiceType = serviceRecord.ServiceType,
            ServiceCenter = serviceRecord.ServiceCenter,
            Description = serviceRecord.Description,
            Cost = serviceRecord.Cost,
            NextServiceDue = serviceRecord.NextServiceDue,
            NextServiceMileage = serviceRecord.NextServiceMileage,
            ReceiptUrl = serviceRecord.ReceiptUrl,
            CreatedAt = serviceRecord.CreatedAt
        };

        return CreatedAtAction(nameof(GetServiceRecord), new { id = serviceRecord.Id }, serviceRecordDto);
    }

    [HttpPut("services/{id}")]
    public async Task<IActionResult> UpdateServiceRecord(Guid id, [FromBody] UpdateServiceRecordRequest request)
    {
        var userId = GetUserId();
        var serviceRecord = await _serviceRecordService.UpdateServiceRecordAsync(id, request, userId);

        if (serviceRecord == null)
            return NotFound(new { message = "Service record not found" });

        var serviceRecordDto = new ServiceRecordDto
        {
            Id = serviceRecord.Id,
            VehicleId = serviceRecord.VehicleId,
            ServiceDate = serviceRecord.ServiceDate,
            MileageAtService = serviceRecord.MileageAtService,
            ServiceType = serviceRecord.ServiceType,
            ServiceCenter = serviceRecord.ServiceCenter,
            Description = serviceRecord.Description,
            Cost = serviceRecord.Cost,
            NextServiceDue = serviceRecord.NextServiceDue,
            NextServiceMileage = serviceRecord.NextServiceMileage,
            ReceiptUrl = serviceRecord.ReceiptUrl,
            CreatedAt = serviceRecord.CreatedAt
        };

        return Ok(serviceRecordDto);
    }

    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteServiceRecord(Guid id)
    {
        var userId = GetUserId();
        var result = await _serviceRecordService.DeleteServiceRecordAsync(id, userId);

        if (!result)
            return NotFound(new { message = "Service record not found" });

        return NoContent();
    }
}
