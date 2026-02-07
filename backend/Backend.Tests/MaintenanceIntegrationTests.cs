using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.DTOs;
using Xunit;

namespace Backend.Tests;

public class MaintenanceIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public MaintenanceIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<string> GetAuthTokenAsync(string username = "maintenanceuser", string email = "maintenance@example.com")
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

    private async Task<Guid> CreateTestVehicleAsync(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateVehicleRequest
        {
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            LicensePlate = "MAINT123",
            CurrentMileage = 50000
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleDto>(content, _jsonOptions);

        return vehicle!.Id;
    }

    [Fact]
    public async Task GetSystemTemplates_ReturnsTemplates()
    {
        // Arrange
        var token = await GetAuthTokenAsync("templatesuser", "templates@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/maintenance-templates/system");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var templates = JsonSerializer.Deserialize<List<MaintenanceTemplateDto>>(content, _jsonOptions);

        Assert.NotNull(templates);
        // Note: In test environment (in-memory DB), seed data isn't auto-applied
        // This test verifies the endpoint works correctly
    }

    [Fact]
    public async Task GetTemplateCategories_ReturnsCategories()
    {
        // Arrange
        var token = await GetAuthTokenAsync("categoriesuser", "categories@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/maintenance-templates/categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var categories = JsonSerializer.Deserialize<List<string>>(content, _jsonOptions);

        Assert.NotNull(categories);
        // Note: In test environment (in-memory DB), seed data isn't auto-applied
        // This test verifies the endpoint returns a list (may be empty in tests)
    }

    [Fact]
    public async Task CreateMaintenanceSchedule_WithValidData_ReturnsCreatedSchedule()
    {
        // Arrange
        var token = await GetAuthTokenAsync("scheduleuser1", "schedule1@example.com");
        var vehicleId = await CreateTestVehicleAsync(token);

        var createRequest = new CreateMaintenanceScheduleDto
        {
            VehicleId = vehicleId,
            TaskName = "Oil Change",
            Category = "Engine",
            IntervalMonths = 6,
            IntervalKilometers = 10000,
            UseCompoundRule = true,
            LastCompletedDate = DateTime.UtcNow.AddMonths(-3),
            LastCompletedMileage = 45000,
            ReminderDaysBefore = 30,
            ReminderKilometersBefore = 1000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance-schedules", createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(content, _jsonOptions);

        Assert.NotNull(schedule);
        Assert.Equal("Oil Change", schedule.TaskName);
        Assert.Equal(6, schedule.IntervalMonths);
        Assert.Equal(10000, schedule.IntervalKilometers);
        Assert.True(schedule.UseCompoundRule);
        Assert.NotNull(schedule.NextDueDate);
        Assert.NotNull(schedule.NextDueMileage);
        Assert.Equal(55000, schedule.NextDueMileage); // 45000 + 10000
    }

    [Fact]
    public async Task GetSchedulesForVehicle_ReturnsVehicleSchedules()
    {
        // Arrange
        var token = await GetAuthTokenAsync("scheduleuser2", "schedule2@example.com");
        var vehicleId = await CreateTestVehicleAsync(token);

        // Create a schedule first
        var createRequest = new CreateMaintenanceScheduleDto
        {
            VehicleId = vehicleId,
            TaskName = "Tire Rotation",
            IntervalKilometers = 10000,
            UseCompoundRule = false,
            LastCompletedMileage = 40000
        };

        await _client.PostAsJsonAsync("/api/maintenance-schedules", createRequest);

        // Act
        var response = await _client.GetAsync($"/api/maintenance-schedules/vehicle/{vehicleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var schedules = JsonSerializer.Deserialize<List<MaintenanceScheduleDetailsDto>>(content, _jsonOptions);

        Assert.NotNull(schedules);
        Assert.Single(schedules);
        Assert.Equal("Tire Rotation", schedules[0].TaskName);
    }

    [Fact]
    public async Task CompleteSchedule_RecalculatesNextDue()
    {
        // Arrange
        var token = await GetAuthTokenAsync("scheduleuser3", "schedule3@example.com");
        var vehicleId = await CreateTestVehicleAsync(token);

        // Create a schedule
        var createRequest = new CreateMaintenanceScheduleDto
        {
            VehicleId = vehicleId,
            TaskName = "Oil Change",
            IntervalMonths = 6,
            IntervalKilometers = 10000,
            UseCompoundRule = true,
            LastCompletedDate = DateTime.UtcNow.AddMonths(-6),
            LastCompletedMileage = 40000
        };

        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-schedules", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdSchedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(createContent, _jsonOptions);

        // Act - Complete the schedule
        var completeRequest = new CompleteMaintenanceScheduleDto
        {
            CompletedDate = DateTime.UtcNow,
            CompletedMileage = 50000
        };

        var response = await _client.PostAsJsonAsync($"/api/maintenance-schedules/{createdSchedule!.Id}/complete", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(content, _jsonOptions);

        Assert.NotNull(schedule);
        Assert.Equal(50000, schedule.LastCompletedMileage);
        Assert.Equal(60000, schedule.NextDueMileage); // 50000 + 10000
        Assert.NotNull(schedule.NextDueDate);
    }

    [Fact]
    public async Task UpdateSchedule_UpdatesScheduleDetails()
    {
        // Arrange
        var token = await GetAuthTokenAsync("scheduleuser4", "schedule4@example.com");
        var vehicleId = await CreateTestVehicleAsync(token);

        // Create a schedule
        var createRequest = new CreateMaintenanceScheduleDto
        {
            VehicleId = vehicleId,
            TaskName = "Brake Inspection",
            IntervalMonths = 12,
            UseCompoundRule = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-schedules", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdSchedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(createContent, _jsonOptions);

        // Act - Update the schedule
        var updateRequest = new UpdateMaintenanceScheduleDto
        {
            TaskName = "Brake Pad Inspection",
            IntervalMonths = 6,
            ReminderDaysBefore = 45
        };

        var response = await _client.PutAsJsonAsync($"/api/maintenance-schedules/{createdSchedule!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(content, _jsonOptions);

        Assert.NotNull(schedule);
        Assert.Equal("Brake Pad Inspection", schedule.TaskName);
        Assert.Equal(6, schedule.IntervalMonths);
        Assert.Equal(45, schedule.ReminderDaysBefore);
    }

    [Fact]
    public async Task DeleteSchedule_RemovesSchedule()
    {
        // Arrange
        var token = await GetAuthTokenAsync("scheduleuser5", "schedule5@example.com");
        var vehicleId = await CreateTestVehicleAsync(token);

        // Create a schedule
        var createRequest = new CreateMaintenanceScheduleDto
        {
            VehicleId = vehicleId,
            TaskName = "Test Schedule",
            IntervalMonths = 12,
            UseCompoundRule = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-schedules", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdSchedule = JsonSerializer.Deserialize<MaintenanceScheduleDetailsDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/maintenance-schedules/{createdSchedule!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/maintenance-schedules/{createdSchedule.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetNotificationCount_ReturnsCount()
    {
        // Arrange
        var token = await GetAuthTokenAsync("notifuser1", "notif1@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications/count");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var count = JsonSerializer.Deserialize<NotificationCountDto>(content, _jsonOptions);

        Assert.NotNull(count);
        Assert.True(count.TotalCount >= 0);
        Assert.True(count.UnreadCount >= 0);
    }

    [Fact]
    public async Task CreateCustomTemplate_WithValidData_ReturnsCreatedTemplate()
    {
        // Arrange
        var token = await GetAuthTokenAsync("customtemplateuser", "customtemplate@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateMaintenanceTemplateDto
        {
            Name = "Custom Maintenance Task",
            Description = "My custom maintenance task",
            Category = "Custom",
            DefaultIntervalMonths = 3,
            DefaultIntervalKilometers = 5000,
            UseCompoundRule = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance-templates", createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var template = JsonSerializer.Deserialize<MaintenanceTemplateDto>(content, _jsonOptions);

        Assert.NotNull(template);
        Assert.Equal("Custom Maintenance Task", template.Name);
        Assert.Equal("Custom", template.Category);
        Assert.Equal(3, template.DefaultIntervalMonths);
        Assert.Equal(5000, template.DefaultIntervalKilometers);
        Assert.False(template.IsSystemTemplate);
    }
}
