using Backend.DTOs;

namespace Backend.Services
{
    public interface IMaintenanceTemplateService
    {
        /// <summary>
        /// Get all templates (system + user custom)
        /// </summary>
        Task<IEnumerable<MaintenanceTemplateDto>> GetAllTemplatesAsync(Guid userId);

        /// <summary>
        /// Get system templates only
        /// </summary>
        Task<IEnumerable<MaintenanceTemplateDto>> GetSystemTemplatesAsync();

        /// <summary>
        /// Get templates by category
        /// </summary>
        Task<IEnumerable<MaintenanceTemplateDto>> GetTemplatesByCategoryAsync(string category, Guid userId);

        /// <summary>
        /// Get a single template by ID
        /// </summary>
        Task<MaintenanceTemplateDto?> GetTemplateByIdAsync(Guid templateId);

        /// <summary>
        /// Create custom user template
        /// </summary>
        Task<MaintenanceTemplateDto> CreateCustomTemplateAsync(Guid userId, CreateMaintenanceTemplateDto dto);

        /// <summary>
        /// Update custom user template (only user-created templates can be updated)
        /// </summary>
        Task<MaintenanceTemplateDto> UpdateCustomTemplateAsync(Guid templateId, Guid userId, UpdateMaintenanceTemplateDto dto);

        /// <summary>
        /// Delete custom user template (only user-created templates can be deleted)
        /// </summary>
        Task<bool> DeleteCustomTemplateAsync(Guid templateId, Guid userId);

        /// <summary>
        /// Get template categories
        /// </summary>
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
