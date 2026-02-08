using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> RegisterAsync(RegisterRequest request);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    string GenerateJwtToken(User user, string secret, string issuer, string audience, int expirationHours);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<UserStatistics> GetUserStatisticsAsync(Guid userId);
}
