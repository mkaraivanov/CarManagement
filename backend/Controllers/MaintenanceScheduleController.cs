using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/maintenance-schedules")]
    public class MaintenanceScheduleController : ControllerBase
    {
        private readonly IMaintenanceScheduleService _scheduleService;

        public MaintenanceScheduleController(IMaintenanceScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
        }

        /// <summary>
        /// Get all maintenance schedules for a vehicle
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<MaintenanceScheduleDetailsDto>>> GetSchedulesForVehicle(Guid vehicleId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedules = await _scheduleService.GetSchedulesForVehicleAsync(vehicleId, userId);
                return Ok(schedules);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving schedules", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a single schedule by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceScheduleDetailsDto>> GetScheduleById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedule = await _scheduleService.GetScheduleByIdAsync(id, userId);
                if (schedule == null)
                {
                    return NotFound(new { message = "Schedule not found" });
                }
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the schedule", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a maintenance schedule
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MaintenanceScheduleDetailsDto>> CreateSchedule([FromBody] CreateMaintenanceScheduleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedule = await _scheduleService.CreateScheduleAsync(userId, dto);
                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the schedule", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a maintenance schedule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MaintenanceScheduleDetailsDto>> UpdateSchedule(Guid id, [FromBody] UpdateMaintenanceScheduleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedule = await _scheduleService.UpdateScheduleAsync(id, userId, dto);
                return Ok(schedule);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the schedule", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a maintenance schedule
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _scheduleService.DeleteScheduleAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { message = "Schedule not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the schedule", error = ex.Message });
            }
        }

        /// <summary>
        /// Mark a schedule as completed
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult<MaintenanceScheduleDetailsDto>> CompleteSchedule(Guid id, [FromBody] CompleteMaintenanceScheduleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedule = await _scheduleService.CompleteScheduleAsync(id, userId, dto);
                return Ok(schedule);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while completing the schedule", error = ex.Message });
            }
        }

        /// <summary>
        /// Link an existing service record to a schedule
        /// </summary>
        [HttpPost("{id}/link-service/{serviceRecordId}")]
        public async Task<ActionResult<MaintenanceScheduleDetailsDto>> LinkServiceRecord(Guid id, Guid serviceRecordId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedule = await _scheduleService.LinkServiceRecordAsync(id, userId, serviceRecordId);
                return Ok(schedule);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while linking the service record", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all overdue schedules for the current user
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<MaintenanceScheduleDetailsDto>>> GetOverdueSchedules()
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedules = await _scheduleService.GetOverdueSchedulesAsync(userId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving overdue schedules", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all upcoming schedules for the current user
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<MaintenanceScheduleDetailsDto>>> GetUpcomingSchedules()
        {
            try
            {
                var userId = GetCurrentUserId();
                var schedules = await _scheduleService.GetUpcomingSchedulesAsync(userId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving upcoming schedules", error = ex.Message });
            }
        }

        /// <summary>
        /// Recalculate next due dates for a schedule
        /// </summary>
        [HttpPost("{id}/recalculate")]
        public async Task<IActionResult> RecalculateNextDue(Guid id)
        {
            try
            {
                await _scheduleService.RecalculateNextDueAsync(id);
                return Ok(new { message = "Next due dates recalculated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while recalculating next due dates", error = ex.Message });
            }
        }

        /// <summary>
        /// Recalculate next due dates for all schedules of a vehicle
        /// </summary>
        [HttpPost("vehicle/{vehicleId}/recalculate")]
        public async Task<IActionResult> RecalculateNextDueForVehicle(Guid vehicleId)
        {
            try
            {
                await _scheduleService.RecalculateNextDueForVehicleAsync(vehicleId);
                return Ok(new { message = "Next due dates recalculated for all vehicle schedules" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while recalculating next due dates", error = ex.Message });
            }
        }
    }
}
