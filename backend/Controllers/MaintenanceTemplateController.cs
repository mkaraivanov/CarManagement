using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/maintenance-templates")]
    public class MaintenanceTemplateController : ControllerBase
    {
        private readonly IMaintenanceTemplateService _templateService;

        public MaintenanceTemplateController(IMaintenanceTemplateService templateService)
        {
            _templateService = templateService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
        }

        /// <summary>
        /// Get all templates (system + user custom)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceTemplateDto>>> GetAllTemplates()
        {
            try
            {
                var userId = GetCurrentUserId();
                var templates = await _templateService.GetAllTemplatesAsync(userId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving templates", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system templates only
        /// </summary>
        [HttpGet("system")]
        public async Task<ActionResult<IEnumerable<MaintenanceTemplateDto>>> GetSystemTemplates()
        {
            try
            {
                var templates = await _templateService.GetSystemTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving system templates", error = ex.Message });
            }
        }

        /// <summary>
        /// Get templates by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MaintenanceTemplateDto>>> GetTemplatesByCategory(string category)
        {
            try
            {
                var userId = GetCurrentUserId();
                var templates = await _templateService.GetTemplatesByCategoryAsync(category, userId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving templates by category", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a single template by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceTemplateDto>> GetTemplateById(Guid id)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    return NotFound(new { message = "Template not found" });
                }
                return Ok(template);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the template", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all template categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _templateService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Create custom user template
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MaintenanceTemplateDto>> CreateCustomTemplate([FromBody] CreateMaintenanceTemplateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var template = await _templateService.CreateCustomTemplateAsync(userId, dto);
                return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the template", error = ex.Message });
            }
        }

        /// <summary>
        /// Update custom user template
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MaintenanceTemplateDto>> UpdateCustomTemplate(Guid id, [FromBody] UpdateMaintenanceTemplateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var template = await _templateService.UpdateCustomTemplateAsync(id, userId, dto);
                return Ok(template);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the template", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete custom user template
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomTemplate(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _templateService.DeleteCustomTemplateAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { message = "Template not found or cannot be deleted (system templates cannot be deleted)" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the template", error = ex.Message });
            }
        }
    }
}
