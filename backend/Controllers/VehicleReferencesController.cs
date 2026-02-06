using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleReferencesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VehicleReferencesController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all car makes with their models
    /// </summary>
    [HttpGet("makes")]
    public async Task<ActionResult<IEnumerable<CarMakeDto>>> GetMakes()
    {
        var makes = await _context.CarMakes
            .Include(m => m.Models)
            .OrderBy(m => m.Name)
            .Select(m => new CarMakeDto
            {
                Id = m.Id,
                Name = m.Name,
                Models = m.Models.Select(model => new CarModelDto
                {
                    Id = model.Id,
                    MakeId = model.MakeId,
                    Name = model.Name
                }).OrderBy(model => model.Name).ToList()
            })
            .ToListAsync();

        return Ok(makes);
    }

    /// <summary>
    /// Get models for a specific make
    /// </summary>
    [HttpGet("makes/{makeId}/models")]
    public async Task<ActionResult<IEnumerable<CarModelDto>>> GetModelsByMake(int makeId)
    {
        var make = await _context.CarMakes
            .Include(m => m.Models)
            .FirstOrDefaultAsync(m => m.Id == makeId);

        if (make == null)
        {
            return NotFound(new { message = "Make not found" });
        }

        var models = make.Models
            .Select(m => new CarModelDto
            {
                Id = m.Id,
                MakeId = m.MakeId,
                Name = m.Name
            })
            .OrderBy(m => m.Name)
            .ToList();

        return Ok(models);
    }

    /// <summary>
    /// Get just the list of make names (lightweight)
    /// </summary>
    [HttpGet("makes/names")]
    public async Task<ActionResult<IEnumerable<string>>> GetMakeNames()
    {
        var makes = await _context.CarMakes
            .OrderBy(m => m.Name)
            .Select(m => m.Name)
            .ToListAsync();

        return Ok(makes);
    }
}
