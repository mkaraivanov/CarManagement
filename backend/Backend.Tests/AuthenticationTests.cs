using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.DTOs;
using Xunit;

namespace Backend.Tests;

public class AuthenticationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthenticationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsTokenAndUser()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

        Assert.NotNull(authResponse);
        Assert.NotNull(authResponse.Token);
        Assert.NotNull(authResponse.User);
        Assert.Equal("testuser", authResponse.User.Username);
        Assert.Equal("test@example.com", authResponse.User.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "duplicateuser",
            Email = "first@example.com",
            Password = "password123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var duplicateRequest = new RegisterRequest
        {
            Username = "duplicateuser", // Same username
            Email = "second@example.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterRequest
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "password123"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = "loginuser",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

        Assert.NotNull(authResponse);
        Assert.NotNull(authResponse.Token);
        Assert.NotNull(authResponse.User);
        Assert.Equal("loginuser", authResponse.User.Username);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterRequest
        {
            Username = "passwordtest",
            Email = "passtest@example.com",
            Password = "correctpassword"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = "passwordtest",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserData()
    {
        // Arrange - Register and login to get token
        var registerRequest = new RegisterRequest
        {
            Username = "tokenuser",
            Email = "token@example.com",
            Password = "password123"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(registerContent, _jsonOptions);

        // Add token to request headers
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

        // Act
        var response = await _client.GetAsync("/api/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(content, _jsonOptions);

        Assert.NotNull(userResponse);
        Assert.Equal("tokenuser", userResponse.Username);
        Assert.Equal("token@example.com", userResponse.Email);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
