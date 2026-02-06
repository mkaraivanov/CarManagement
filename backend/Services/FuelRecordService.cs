using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public class FuelRecordService : IFuelRecordService
{
    private readonly ApplicationDbContext _context;

    public FuelRecordService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FuelRecord>> GetVehicleFuelRecordsAsync(Guid vehicleId, Guid userId)
    {
        // Verify user owns the vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
            return Enumerable.Empty<FuelRecord>();

        return await _context.FuelRecords
            .Where(fr => fr.VehicleId == vehicleId)
            .OrderByDescending(fr => fr.RefuelDate)
            .ToListAsync();
    }

    public async Task<FuelRecord?> GetFuelRecordByIdAsync(Guid fuelRecordId, Guid userId)
    {
        return await _context.FuelRecords
            .Include(fr => fr.Vehicle)
            .FirstOrDefaultAsync(fr => fr.Id == fuelRecordId && fr.Vehicle.UserId == userId);
    }

    public async Task<FuelRecord?> CreateFuelRecordAsync(Guid vehicleId, CreateFuelRecordRequest request, Guid userId)
    {
        // Verify user owns the vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
            return null;

        // Calculate fuel efficiency based on previous refueling
        decimal? fuelEfficiency = null;
        var previousFuelRecord = await _context.FuelRecords
            .Where(fr => fr.VehicleId == vehicleId && fr.Mileage < request.Mileage)
            .OrderByDescending(fr => fr.Mileage)
            .FirstOrDefaultAsync();

        if (previousFuelRecord != null)
        {
            var mileageDifference = request.Mileage - previousFuelRecord.Mileage;
            if (mileageDifference > 0 && request.Quantity > 0)
            {
                // Calculate efficiency (miles per gallon or km per liter)
                fuelEfficiency = mileageDifference / request.Quantity;
            }
        }

        var totalCost = request.Quantity * request.PricePerUnit;

        var fuelRecord = new FuelRecord
        {
            VehicleId = vehicleId,
            RefuelDate = request.RefuelDate,
            Mileage = request.Mileage,
            Quantity = request.Quantity,
            PricePerUnit = request.PricePerUnit,
            TotalCost = totalCost,
            FuelType = request.FuelType,
            GasStation = request.GasStation,
            FuelEfficiency = fuelEfficiency,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Auto-update vehicle mileage if fuel record mileage is higher
        if (request.Mileage > vehicle.CurrentMileage)
        {
            vehicle.CurrentMileage = request.Mileage;
            vehicle.UpdatedAt = DateTime.UtcNow;
        }

        _context.FuelRecords.Add(fuelRecord);
        await _context.SaveChangesAsync();

        return fuelRecord;
    }

    public async Task<FuelRecord?> UpdateFuelRecordAsync(Guid fuelRecordId, UpdateFuelRecordRequest request, Guid userId)
    {
        var fuelRecord = await GetFuelRecordByIdAsync(fuelRecordId, userId);

        if (fuelRecord == null)
            return null;

        var needsRecalculation = false;

        // Update only provided fields
        if (request.RefuelDate.HasValue)
            fuelRecord.RefuelDate = request.RefuelDate.Value;

        if (request.Mileage.HasValue)
        {
            fuelRecord.Mileage = request.Mileage.Value;
            needsRecalculation = true;
        }

        if (request.Quantity.HasValue)
        {
            fuelRecord.Quantity = request.Quantity.Value;
            needsRecalculation = true;
        }

        if (request.PricePerUnit.HasValue)
        {
            fuelRecord.PricePerUnit = request.PricePerUnit.Value;
            needsRecalculation = true;
        }

        if (request.FuelType.HasValue)
            fuelRecord.FuelType = request.FuelType.Value;

        if (request.GasStation != null)
            fuelRecord.GasStation = request.GasStation;

        if (request.Notes != null)
            fuelRecord.Notes = request.Notes;

        // Recalculate total cost if needed
        if (needsRecalculation)
        {
            fuelRecord.TotalCost = fuelRecord.Quantity * fuelRecord.PricePerUnit;
        }

        await _context.SaveChangesAsync();

        return fuelRecord;
    }

    public async Task<bool> DeleteFuelRecordAsync(Guid fuelRecordId, Guid userId)
    {
        var fuelRecord = await GetFuelRecordByIdAsync(fuelRecordId, userId);

        if (fuelRecord == null)
            return false;

        _context.FuelRecords.Remove(fuelRecord);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<FuelEfficiencyStats?> GetFuelEfficiencyStatsAsync(Guid vehicleId, Guid userId)
    {
        // Verify user owns the vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
            return null;

        var fuelRecords = await _context.FuelRecords
            .Where(fr => fr.VehicleId == vehicleId && fr.FuelEfficiency.HasValue)
            .OrderByDescending(fr => fr.RefuelDate)
            .Take(10) // Last 10 records with efficiency
            .ToListAsync();

        if (!fuelRecords.Any())
            return new FuelEfficiencyStats();

        var stats = new FuelEfficiencyStats
        {
            AverageEfficiency = fuelRecords.Average(fr => fr.FuelEfficiency ?? 0),
            TotalCost = fuelRecords.Sum(fr => fr.TotalCost),
            TotalQuantity = fuelRecords.Sum(fr => fr.Quantity),
            RecordCount = fuelRecords.Count
        };

        return stats;
    }
}
