using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IVehicleService
{
    Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(Guid userId);
    Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId, Guid userId);
    Task<Vehicle> CreateVehicleAsync(CreateVehicleRequest request, Guid userId);
    Task<Vehicle?> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequest request, Guid userId);
    Task<bool> DeleteVehicleAsync(Guid vehicleId, Guid userId);
    Task<bool> UpdateMileageAsync(Guid vehicleId, int mileage, Guid userId);
}
