using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKey12345SuperSecretKey12345";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "CarManagementAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "CarManagementClient";
        var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "1");

        var token = _userService.GenerateJwtToken(user, jwtSecret, jwtIssuer, jwtAudience, expirationHours);

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(new AuthResponse
        {
            Token = token,
            User = userResponse
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _userService.RegisterAsync(request);

        if (user == null)
            return BadRequest(new { message = "Username or email already exists" });

        var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKey12345SuperSecretKey12345";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "CarManagementAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "CarManagementClient";
        var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "1");

        var token = _userService.GenerateJwtToken(user, jwtSecret, jwtIssuer, jwtAudience, expirationHours);

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(new AuthResponse
        {
            Token = token,
            User = userResponse
        });
    }
}
