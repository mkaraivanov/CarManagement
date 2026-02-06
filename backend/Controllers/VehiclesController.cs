using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        var userId = GetUserId();
        var vehicles = await _vehicleService.GetUserVehiclesAsync(userId);

        var vehicleDtos = vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            Make = v.Make,
            Model = v.Model,
            Year = v.Year,
            VIN = v.VIN,
            LicensePlate = v.LicensePlate,
            CurrentMileage = v.CurrentMileage,
            PurchaseDate = v.PurchaseDate,
            Color = v.Color,
            Status = v.Status,
            PhotoUrl = v.PhotoUrl,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        });

        return Ok(vehicleDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        var userId = GetUserId();
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id, userId);

        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found" });

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            CurrentMileage = vehicle.CurrentMileage,
            PurchaseDate = vehicle.PurchaseDate,
            Color = vehicle.Color,
            Status = vehicle.Status,
            PhotoUrl = vehicle.PhotoUrl,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt
        };

        return Ok(vehicleDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        var userId = GetUserId();
        var vehicle = await _vehicleService.CreateVehicleAsync(request, userId);

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            CurrentMileage = vehicle.CurrentMileage,
            PurchaseDate = vehicle.PurchaseDate,
            Color = vehicle.Color,
            Status = vehicle.Status,
            PhotoUrl = vehicle.PhotoUrl,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt
        };

        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicleDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        var userId = GetUserId();
        var vehicle = await _vehicleService.UpdateVehicleAsync(id, request, userId);

        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found" });

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            CurrentMileage = vehicle.CurrentMileage,
            PurchaseDate = vehicle.PurchaseDate,
            Color = vehicle.Color,
            Status = vehicle.Status,
            PhotoUrl = vehicle.PhotoUrl,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt
        };

        return Ok(vehicleDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var userId = GetUserId();
        var result = await _vehicleService.DeleteVehicleAsync(id, userId);

        if (!result)
            return NotFound(new { message = "Vehicle not found" });

        return NoContent();
    }

    [HttpPatch("{id}/mileage")]
    public async Task<IActionResult> UpdateMileage(Guid id, [FromBody] UpdateMileageRequest request)
    {
        var userId = GetUserId();
        var result = await _vehicleService.UpdateMileageAsync(id, request.Mileage, userId);

        if (!result)
            return NotFound(new { message = "Vehicle not found" });

        return NoContent();
    }
}

public class UpdateMileageRequest
{
    public int Mileage { get; set; }
}
