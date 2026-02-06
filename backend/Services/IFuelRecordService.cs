using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IFuelRecordService
{
    Task<IEnumerable<FuelRecord>> GetVehicleFuelRecordsAsync(Guid vehicleId, Guid userId);
    Task<FuelRecord?> GetFuelRecordByIdAsync(Guid fuelRecordId, Guid userId);
    Task<FuelRecord?> CreateFuelRecordAsync(Guid vehicleId, CreateFuelRecordRequest request, Guid userId);
    Task<FuelRecord?> UpdateFuelRecordAsync(Guid fuelRecordId, UpdateFuelRecordRequest request, Guid userId);
    Task<bool> DeleteFuelRecordAsync(Guid fuelRecordId, Guid userId);
    Task<FuelEfficiencyStats?> GetFuelEfficiencyStatsAsync(Guid vehicleId, Guid userId);
}

public class FuelEfficiencyStats
{
    public decimal AverageEfficiency { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalQuantity { get; set; }
    public int RecordCount { get; set; }
}
