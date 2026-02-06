using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public class ServiceRecordService : IServiceRecordService
{
    private readonly ApplicationDbContext _context;

    public ServiceRecordService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceRecord>> GetVehicleServiceRecordsAsync(Guid vehicleId, Guid userId)
    {
        // Verify user owns the vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
            return Enumerable.Empty<ServiceRecord>();

        return await _context.ServiceRecords
            .Where(sr => sr.VehicleId == vehicleId)
            .OrderByDescending(sr => sr.ServiceDate)
            .ToListAsync();
    }

    public async Task<ServiceRecord?> GetServiceRecordByIdAsync(Guid serviceRecordId, Guid userId)
    {
        return await _context.ServiceRecords
            .Include(sr => sr.Vehicle)
            .FirstOrDefaultAsync(sr => sr.Id == serviceRecordId && sr.Vehicle.UserId == userId);
    }

    public async Task<ServiceRecord?> CreateServiceRecordAsync(Guid vehicleId, CreateServiceRecordRequest request, Guid userId)
    {
        // Verify user owns the vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

        if (vehicle == null)
            return null;

        var serviceRecord = new ServiceRecord
        {
            VehicleId = vehicleId,
            ServiceDate = request.ServiceDate,
            MileageAtService = request.MileageAtService,
            ServiceType = request.ServiceType,
            ServiceCenter = request.ServiceCenter,
            Description = request.Description,
            Cost = request.Cost,
            NextServiceDue = request.NextServiceDue,
            NextServiceMileage = request.NextServiceMileage,
            ReceiptUrl = request.ReceiptUrl,
            CreatedAt = DateTime.UtcNow
        };

        // Auto-update vehicle mileage if service mileage is higher
        if (request.MileageAtService > vehicle.CurrentMileage)
        {
            vehicle.CurrentMileage = request.MileageAtService;
            vehicle.UpdatedAt = DateTime.UtcNow;
        }

        _context.ServiceRecords.Add(serviceRecord);
        await _context.SaveChangesAsync();

        return serviceRecord;
    }

    public async Task<ServiceRecord?> UpdateServiceRecordAsync(Guid serviceRecordId, UpdateServiceRecordRequest request, Guid userId)
    {
        var serviceRecord = await GetServiceRecordByIdAsync(serviceRecordId, userId);

        if (serviceRecord == null)
            return null;

        // Update only provided fields
        if (request.ServiceDate.HasValue)
            serviceRecord.ServiceDate = request.ServiceDate.Value;

        if (request.MileageAtService.HasValue)
            serviceRecord.MileageAtService = request.MileageAtService.Value;

        if (request.ServiceType.HasValue)
            serviceRecord.ServiceType = request.ServiceType.Value;

        if (!string.IsNullOrEmpty(request.ServiceCenter))
            serviceRecord.ServiceCenter = request.ServiceCenter;

        if (!string.IsNullOrEmpty(request.Description))
            serviceRecord.Description = request.Description;

        if (request.Cost.HasValue)
            serviceRecord.Cost = request.Cost.Value;

        if (request.NextServiceDue.HasValue)
            serviceRecord.NextServiceDue = request.NextServiceDue;

        if (request.NextServiceMileage.HasValue)
            serviceRecord.NextServiceMileage = request.NextServiceMileage;

        if (request.ReceiptUrl != null)
            serviceRecord.ReceiptUrl = request.ReceiptUrl;

        await _context.SaveChangesAsync();

        return serviceRecord;
    }

    public async Task<bool> DeleteServiceRecordAsync(Guid serviceRecordId, Guid userId)
    {
        var serviceRecord = await GetServiceRecordByIdAsync(serviceRecordId, userId);

        if (serviceRecord == null)
            return false;

        _context.ServiceRecords.Remove(serviceRecord);
        await _context.SaveChangesAsync();

        return true;
    }
}
