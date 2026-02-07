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
            // Registration fields
            RegistrationNumber = request.RegistrationNumber,
            RegistrationIssueDate = request.RegistrationIssueDate,
            RegistrationExpiryDate = request.RegistrationExpiryDate,
            RegistrationDocumentUrl = request.RegistrationDocumentUrl,
            RegistrationStatus = request.RegistrationStatus,
            // Owner information
            OwnerName = request.OwnerName,
            OwnerAddress = request.OwnerAddress,
            // Vehicle specifications
            BodyType = request.BodyType,
            EngineInfo = request.EngineInfo,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            Seats = request.Seats,
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

        // Registration fields
        if (request.RegistrationNumber != null)
            vehicle.RegistrationNumber = request.RegistrationNumber;

        if (request.RegistrationIssueDate.HasValue)
            vehicle.RegistrationIssueDate = request.RegistrationIssueDate;

        if (request.RegistrationExpiryDate.HasValue)
            vehicle.RegistrationExpiryDate = request.RegistrationExpiryDate;

        if (request.RegistrationDocumentUrl != null)
            vehicle.RegistrationDocumentUrl = request.RegistrationDocumentUrl;

        if (request.RegistrationStatus.HasValue)
            vehicle.RegistrationStatus = request.RegistrationStatus.Value;

        // Owner information
        if (request.OwnerName != null)
            vehicle.OwnerName = request.OwnerName;

        if (request.OwnerAddress != null)
            vehicle.OwnerAddress = request.OwnerAddress;

        // Vehicle specifications
        if (request.BodyType != null)
            vehicle.BodyType = request.BodyType;

        if (request.EngineInfo != null)
            vehicle.EngineInfo = request.EngineInfo;

        if (request.FuelType != null)
            vehicle.FuelType = request.FuelType;

        if (request.Transmission != null)
            vehicle.Transmission = request.Transmission;

        if (request.Seats.HasValue)
            vehicle.Seats = request.Seats;

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
