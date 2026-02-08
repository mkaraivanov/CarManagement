using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound(new { message = "User not found" });

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(userResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var userResponses = new List<UserDetailResponse>();

        foreach (var user in users)
        {
            var statistics = await _userService.GetUserStatisticsAsync(user.Id);
            userResponses.Add(new UserDetailResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Statistics = statistics
            });
        }

        return Ok(userResponses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var statistics = await _userService.GetUserStatisticsAsync(user.Id);
        var userResponse = new UserDetailResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Statistics = statistics
        };

        return Ok(userResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.UpdateUserAsync(id, request);
        if (user == null)
            return BadRequest(new { message = "Failed to update user. Username or email may already be in use." });

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(userResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var currentUserId))
            return Unauthorized();

        // Prevent self-deletion
        if (id == currentUserId)
            return BadRequest(new { message = "Cannot delete your own account" });

        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound(new { message = "User not found" });

        return NoContent();
    }
}
