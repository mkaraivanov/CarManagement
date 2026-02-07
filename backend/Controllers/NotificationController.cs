using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
        }

        /// <summary>
        /// Get all notifications for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications([FromQuery] int limit = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsForUserAsync(userId, limit);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notifications", error = ex.Message });
            }
        }

        /// <summary>
        /// Get unread notifications for the current user
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving unread notifications", error = ex.Message });
            }
        }

        /// <summary>
        /// Get notification count for the current user
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<NotificationCountDto>> GetNotificationCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _notificationService.GetNotificationCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notification count", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a single notification by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotificationById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found" });
                }
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the notification", error = ex.Message });
            }
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPost("{id}/read")]
        public async Task<ActionResult<NotificationDto>> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notification = await _notificationService.MarkAsReadAsync(id, userId);
                return Ok(notification);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while marking notification as read", error = ex.Message });
            }
        }

        /// <summary>
        /// Mark all notifications as read for the current user
        /// </summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { message = "All notifications marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while marking all notifications as read", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _notificationService.DeleteNotificationAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { message = "Notification not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the notification", error = ex.Message });
            }
        }
    }
}
