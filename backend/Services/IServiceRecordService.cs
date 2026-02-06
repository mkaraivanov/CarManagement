using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IServiceRecordService
{
    Task<IEnumerable<ServiceRecord>> GetVehicleServiceRecordsAsync(Guid vehicleId, Guid userId);
    Task<ServiceRecord?> GetServiceRecordByIdAsync(Guid serviceRecordId, Guid userId);
    Task<ServiceRecord?> CreateServiceRecordAsync(Guid vehicleId, CreateServiceRecordRequest request, Guid userId);
    Task<ServiceRecord?> UpdateServiceRecordAsync(Guid serviceRecordId, UpdateServiceRecordRequest request, Guid userId);
    Task<bool> DeleteServiceRecordAsync(Guid serviceRecordId, Guid userId);
}
