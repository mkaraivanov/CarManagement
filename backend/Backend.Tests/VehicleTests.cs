using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.DTOs;
using Backend.Models;
using Xunit;

namespace Backend.Tests;

public class VehicleTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public VehicleTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<string> GetAuthTokenAsync(string username = "vehicleuser", string email = "vehicle@example.com")
    {
        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

        return authResponse!.Token!;
    }

    [Fact]
    public async Task CreateVehicle_WithValidData_ReturnsCreatedVehicle()
    {
        // Arrange
        var token = await GetAuthTokenAsync("createuser", "create@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateVehicleRequest
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            LicensePlate = "ABC123",
            CurrentMileage = 15000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vehicles", createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleDto>(content, _jsonOptions);

        Assert.NotNull(vehicle);
        Assert.Equal("Toyota", vehicle.Make);
        Assert.Equal("Camry", vehicle.Model);
        Assert.Equal(2022, vehicle.Year);
        Assert.Equal("ABC123", vehicle.LicensePlate);
        Assert.Equal(15000, vehicle.CurrentMileage);
    }

    [Fact]
    public async Task CreateVehicle_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var createRequest = new CreateVehicleRequest
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            LicensePlate = "XYZ789",
            CurrentMileage = 10000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vehicles", createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetVehicles_ReturnsOnlyUserVehicles()
    {
        // Arrange - Create two users with vehicles
        var token1 = await GetAuthTokenAsync("user1", "user1@example.com");
        var token2 = await GetAuthTokenAsync("user2", "user2@example.com");

        // User 1 creates a vehicle
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
        await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Honda",
            Model = "Civic",
            Year = 2021,
            LicensePlate = "USER1-1",
            CurrentMileage = 20000
        });

        // User 2 creates a vehicle
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);
        await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Ford",
            Model = "Mustang",
            Year = 2023,
            LicensePlate = "USER2-1",
            CurrentMileage = 5000
        });

        // Act - Get vehicles for User 1
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleDto>>(content, _jsonOptions);

        Assert.NotNull(vehicles);
        Assert.Single(vehicles); // User 1 should only see their own vehicle
        Assert.Equal("USER1-1", vehicles[0].LicensePlate);
    }

    [Fact]
    public async Task GetVehicleById_WithValidId_ReturnsVehicle()
    {
        // Arrange
        var token = await GetAuthTokenAsync("getuser", "get@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Tesla",
            Model = "Model 3",
            Year = 2024,
            LicensePlate = "TESLA1",
            CurrentMileage = 1000
        });

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<VehicleDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/vehicles/{createdVehicle!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleDto>(content, _jsonOptions);

        Assert.NotNull(vehicle);
        Assert.Equal(createdVehicle.Id, vehicle.Id);
        Assert.Equal("Tesla", vehicle.Make);
        Assert.Equal("Model 3", vehicle.Model);
    }

    [Fact]
    public async Task UpdateVehicle_WithValidData_ReturnsUpdatedVehicle()
    {
        // Arrange
        var token = await GetAuthTokenAsync("updateuser", "update@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Mazda",
            Model = "CX-5",
            Year = 2020,
            LicensePlate = "MAZDA1",
            CurrentMileage = 30000
        });

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<VehicleDto>(createContent, _jsonOptions);

        var updateRequest = new UpdateVehicleRequest
        {
            Make = "Mazda",
            Model = "CX-5",
            Year = 2020,
            LicensePlate = "MAZDA1-UPD",
            CurrentMileage = 35000,
            Status = VehicleStatus.Sold
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/vehicles/{createdVehicle!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleDto>(content, _jsonOptions);

        Assert.NotNull(vehicle);
        Assert.Equal("MAZDA1-UPD", vehicle.LicensePlate);
        Assert.Equal(35000, vehicle.CurrentMileage);
        Assert.Equal(VehicleStatus.Sold, vehicle.Status);
    }

    [Fact]
    public async Task DeleteVehicle_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync("deleteuser", "delete@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Subaru",
            Model = "Outback",
            Year = 2019,
            LicensePlate = "SUBARU1",
            CurrentMileage = 45000
        });

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<VehicleDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/vehicles/{createdVehicle!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify vehicle is deleted
        var getResponse = await _client.GetAsync($"/api/vehicles/{createdVehicle.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_OfAnotherUser_ReturnsForbidden()
    {
        // Arrange - User 1 creates a vehicle
        var token1 = await GetAuthTokenAsync("owner", "owner@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Make = "Nissan",
            Model = "Altima",
            Year = 2021,
            LicensePlate = "NISSAN1",
            CurrentMileage = 25000
        });

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<VehicleDto>(createContent, _jsonOptions);

        // User 2 tries to delete User 1's vehicle
        var token2 = await GetAuthTokenAsync("attacker", "attacker@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

        // Act
        var response = await _client.DeleteAsync($"/api/vehicles/{createdVehicle!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
