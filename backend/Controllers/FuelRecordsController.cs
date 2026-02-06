using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class FuelRecordsController : ControllerBase
{
    private readonly IFuelRecordService _fuelRecordService;

    public FuelRecordsController(IFuelRecordService fuelRecordService)
    {
        _fuelRecordService = fuelRecordService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("vehicles/{vehicleId}/fuel-records")]
    public async Task<IActionResult> GetVehicleFuelRecords(Guid vehicleId)
    {
        var userId = GetUserId();
        var fuelRecords = await _fuelRecordService.GetVehicleFuelRecordsAsync(vehicleId, userId);

        var fuelRecordDtos = fuelRecords.Select(fr => new FuelRecordDto
        {
            Id = fr.Id,
            VehicleId = fr.VehicleId,
            RefuelDate = fr.RefuelDate,
            Mileage = fr.Mileage,
            Quantity = fr.Quantity,
            PricePerUnit = fr.PricePerUnit,
            TotalCost = fr.TotalCost,
            FuelType = fr.FuelType,
            GasStation = fr.GasStation,
            FuelEfficiency = fr.FuelEfficiency,
            Notes = fr.Notes,
            CreatedAt = fr.CreatedAt
        });

        return Ok(fuelRecordDtos);
    }

    [HttpGet("fuel-records/{id}")]
    public async Task<IActionResult> GetFuelRecord(Guid id)
    {
        var userId = GetUserId();
        var fuelRecord = await _fuelRecordService.GetFuelRecordByIdAsync(id, userId);

        if (fuelRecord == null)
            return NotFound(new { message = "Fuel record not found" });

        var fuelRecordDto = new FuelRecordDto
        {
            Id = fuelRecord.Id,
            VehicleId = fuelRecord.VehicleId,
            RefuelDate = fuelRecord.RefuelDate,
            Mileage = fuelRecord.Mileage,
            Quantity = fuelRecord.Quantity,
            PricePerUnit = fuelRecord.PricePerUnit,
            TotalCost = fuelRecord.TotalCost,
            FuelType = fuelRecord.FuelType,
            GasStation = fuelRecord.GasStation,
            FuelEfficiency = fuelRecord.FuelEfficiency,
            Notes = fuelRecord.Notes,
            CreatedAt = fuelRecord.CreatedAt
        };

        return Ok(fuelRecordDto);
    }

    [HttpPost("vehicles/{vehicleId}/fuel-records")]
    public async Task<IActionResult> CreateFuelRecord(Guid vehicleId, [FromBody] CreateFuelRecordRequest request)
    {
        var userId = GetUserId();
        var fuelRecord = await _fuelRecordService.CreateFuelRecordAsync(vehicleId, request, userId);

        if (fuelRecord == null)
            return NotFound(new { message = "Vehicle not found" });

        var fuelRecordDto = new FuelRecordDto
        {
            Id = fuelRecord.Id,
            VehicleId = fuelRecord.VehicleId,
            RefuelDate = fuelRecord.RefuelDate,
            Mileage = fuelRecord.Mileage,
            Quantity = fuelRecord.Quantity,
            PricePerUnit = fuelRecord.PricePerUnit,
            TotalCost = fuelRecord.TotalCost,
            FuelType = fuelRecord.FuelType,
            GasStation = fuelRecord.GasStation,
            FuelEfficiency = fuelRecord.FuelEfficiency,
            Notes = fuelRecord.Notes,
            CreatedAt = fuelRecord.CreatedAt
        };

        return CreatedAtAction(nameof(GetFuelRecord), new { id = fuelRecord.Id }, fuelRecordDto);
    }

    [HttpPut("fuel-records/{id}")]
    public async Task<IActionResult> UpdateFuelRecord(Guid id, [FromBody] UpdateFuelRecordRequest request)
    {
        var userId = GetUserId();
        var fuelRecord = await _fuelRecordService.UpdateFuelRecordAsync(id, request, userId);

        if (fuelRecord == null)
            return NotFound(new { message = "Fuel record not found" });

        var fuelRecordDto = new FuelRecordDto
        {
            Id = fuelRecord.Id,
            VehicleId = fuelRecord.VehicleId,
            RefuelDate = fuelRecord.RefuelDate,
            Mileage = fuelRecord.Mileage,
            Quantity = fuelRecord.Quantity,
            PricePerUnit = fuelRecord.PricePerUnit,
            TotalCost = fuelRecord.TotalCost,
            FuelType = fuelRecord.FuelType,
            GasStation = fuelRecord.GasStation,
            FuelEfficiency = fuelRecord.FuelEfficiency,
            Notes = fuelRecord.Notes,
            CreatedAt = fuelRecord.CreatedAt
        };

        return Ok(fuelRecordDto);
    }

    [HttpDelete("fuel-records/{id}")]
    public async Task<IActionResult> DeleteFuelRecord(Guid id)
    {
        var userId = GetUserId();
        var result = await _fuelRecordService.DeleteFuelRecordAsync(id, userId);

        if (!result)
            return NotFound(new { message = "Fuel record not found" });

        return NoContent();
    }

    [HttpGet("vehicles/{vehicleId}/fuel-efficiency")]
    public async Task<IActionResult> GetFuelEfficiencyStats(Guid vehicleId)
    {
        var userId = GetUserId();
        var stats = await _fuelRecordService.GetFuelEfficiencyStatsAsync(vehicleId, userId);

        if (stats == null)
            return NotFound(new { message = "Vehicle not found" });

        return Ok(stats);
    }
}
