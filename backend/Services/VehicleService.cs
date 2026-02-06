using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(Guid userId)
    {
        return await _context.Vehicles
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId, Guid userId)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);
    }

    public async Task<Vehicle> CreateVehicleAsync(CreateVehicleRequest request, Guid userId)
    {
        var vehicle = new Vehicle
        {
            UserId = userId,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            VIN = request.VIN,
            LicensePlate = request.LicensePlate,
            CurrentMileage = request.CurrentMileage,
            PurchaseDate = request.PurchaseDate,
            Color = request.Color,
            Status = request.Status,
            PhotoUrl = request.PhotoUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<Vehicle?> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, Guid userId)
    {
        var vehicle = await GetVehicleByIdAsync(vehicleId, userId);

        if (vehicle == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Make))
            vehicle.Make = request.Make;

        if (!string.IsNullOrEmpty(request.Model))
            vehicle.Model = request.Model;

        if (request.Year.HasValue)
            vehicle.Year = request.Year.Value;

        if (request.VIN != null)
            vehicle.VIN = request.VIN;

        if (!string.IsNullOrEmpty(request.LicensePlate))
            vehicle.LicensePlate = request.LicensePlate;

        if (request.CurrentMileage.HasValue)
            vehicle.CurrentMileage = request.CurrentMileage.Value;

        if (request.PurchaseDate.HasValue)
            vehicle.PurchaseDate = request.PurchaseDate;

        if (request.Color != null)
            vehicle.Color = request.Color;

        if (request.Status.HasValue)
            vehicle.Status = request.Status.Value;

        if (request.PhotoUrl != null)
            vehicle.PhotoUrl = request.PhotoUrl;

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<bool> DeleteVehicleAsync(Guid vehicleId, Guid userId)
    {
        var vehicle = await GetVehicleByIdAsync(vehicleId, userId);

        if (vehicle == null)
            return false;

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateMileageAsync(Guid vehicleId, int mileage, Guid userId)
    {
        var vehicle = await GetVehicleByIdAsync(vehicleId, userId);

        if (vehicle == null)
            return false;

        vehicle.CurrentMileage = mileage;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
