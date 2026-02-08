using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using BCrypt.Net;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return null;

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public string GenerateJwtToken(User user, string secret, string issuer, string audience, int expirationHours)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<User?> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        // Check for duplicate username if provided
        if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return null;
            user.Username = request.Username;
        }

        // Check for duplicate email if provided
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;
            user.Email = request.Email;
        }

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserStatistics> GetUserStatisticsAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Vehicles)
                .ThenInclude(v => v.ServiceRecords)
            .Include(u => u.Vehicles)
                .ThenInclude(v => v.FuelRecords)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new UserStatistics();
        }

        return new UserStatistics
        {
            VehicleCount = user.Vehicles.Count,
            ServiceRecordCount = user.Vehicles.Sum(v => v.ServiceRecords.Count),
            FuelRecordCount = user.Vehicles.Sum(v => v.FuelRecords.Count)
        };
    }
}
