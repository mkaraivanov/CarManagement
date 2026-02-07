using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class MaintenanceTemplateService : IMaintenanceTemplateService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceTemplateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceTemplateDto>> GetAllTemplatesAsync(Guid userId)
        {
            var templates = await _context.MaintenanceTemplates
                .Where(t => t.IsSystemTemplate || t.UserId == userId)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceTemplateDto>> GetSystemTemplatesAsync()
        {
            var templates = await _context.MaintenanceTemplates
                .Where(t => t.IsSystemTemplate)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceTemplateDto>> GetTemplatesByCategoryAsync(string category, Guid userId)
        {
            var templates = await _context.MaintenanceTemplates
                .Where(t => t.Category == category && (t.IsSystemTemplate || t.UserId == userId))
                .OrderBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToDto);
        }

        public async Task<MaintenanceTemplateDto?> GetTemplateByIdAsync(Guid templateId)
        {
            var template = await _context.MaintenanceTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId);

            return template == null ? null : MapToDto(template);
        }

        public async Task<MaintenanceTemplateDto> CreateCustomTemplateAsync(Guid userId, CreateMaintenanceTemplateDto dto)
        {
            var template = new MaintenanceTemplate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IsSystemTemplate = false,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                DefaultIntervalMonths = dto.DefaultIntervalMonths,
                DefaultIntervalKilometers = dto.DefaultIntervalKilometers,
                DefaultIntervalHours = dto.DefaultIntervalHours,
                UseCompoundRule = dto.UseCompoundRule,
                CreatedAt = DateTime.UtcNow
            };

            _context.MaintenanceTemplates.Add(template);
            await _context.SaveChangesAsync();

            return MapToDto(template);
        }

        public async Task<MaintenanceTemplateDto> UpdateCustomTemplateAsync(Guid templateId, Guid userId, UpdateMaintenanceTemplateDto dto)
        {
            var template = await _context.MaintenanceTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId && !t.IsSystemTemplate);

            if (template == null)
            {
                throw new InvalidOperationException("Template not found or cannot be modified (system templates cannot be updated)");
            }

            if (dto.Name != null) template.Name = dto.Name;
            if (dto.Description != null) template.Description = dto.Description;
            if (dto.Category != null) template.Category = dto.Category;
            if (dto.DefaultIntervalMonths.HasValue) template.DefaultIntervalMonths = dto.DefaultIntervalMonths;
            if (dto.DefaultIntervalKilometers.HasValue) template.DefaultIntervalKilometers = dto.DefaultIntervalKilometers;
            if (dto.DefaultIntervalHours.HasValue) template.DefaultIntervalHours = dto.DefaultIntervalHours;
            if (dto.UseCompoundRule.HasValue) template.UseCompoundRule = dto.UseCompoundRule.Value;

            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(template);
        }

        public async Task<bool> DeleteCustomTemplateAsync(Guid templateId, Guid userId)
        {
            var template = await _context.MaintenanceTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId && !t.IsSystemTemplate);

            if (template == null)
            {
                return false;
            }

            _context.MaintenanceTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.MaintenanceTemplates
                .Where(t => t.IsSystemTemplate)
                .Select(t => t.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        private static MaintenanceTemplateDto MapToDto(MaintenanceTemplate template)
        {
            return new MaintenanceTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                IsSystemTemplate = template.IsSystemTemplate,
                UserId = template.UserId,
                DefaultIntervalMonths = template.DefaultIntervalMonths,
                DefaultIntervalKilometers = template.DefaultIntervalKilometers,
                DefaultIntervalHours = template.DefaultIntervalHours,
                UseCompoundRule = template.UseCompoundRule,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }
    }
}
