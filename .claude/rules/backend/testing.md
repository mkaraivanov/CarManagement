---
paths:
  - "backend/**/*.cs"
  - "Backend.Tests/**/*.cs"
---

# Backend Testing

## Running Backend Tests

```bash
cd backend
dotnet test                                    # Run all backend tests
dotnet test --filter "ClassName~YourTest"     # Run specific test class
dotnet test --logger "console;verbosity=detailed"  # Verbose output
```

**Expected Results:**
- Backend: 18 tests passing ✅

## Test Infrastructure

- **xUnit** testing framework
- **WebApplicationFactory** for integration testing
- **In-Memory Database** automatically used in Testing environment
- Tests run against real API with isolated database per test run

## Current Test Coverage

**Authentication Tests (6):**
- Register with valid data returns token and user
- Register with duplicate username returns bad request
- Login with valid credentials returns token
- Login with invalid password returns unauthorized
- Get current user with valid token returns user data
- Get current user without token returns unauthorized

**Vehicle Tests (7):**
- Create vehicle with valid data returns created vehicle
- Create vehicle without token returns unauthorized
- Get vehicles returns only user's vehicles (authorization)
- Get vehicle by ID returns vehicle
- Update vehicle with valid data returns updated vehicle
- Delete vehicle with valid ID returns no content
- Delete vehicle of another user returns not found (authorization)

## Writing New Tests

When adding new features, follow this pattern:

```csharp
public class NewFeatureTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NewFeatureTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Feature_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var request = new { /* test data */ };

        // Act
        var response = await _client.PostAsJsonAsync("/api/endpoint", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Test Patterns

**Testing Authentication:**
```csharp
// Register and get token
var registerRequest = new { username = "testuser", password = "Test123!" };
var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
var authData = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
var token = authData.Token;

// Use token in subsequent requests
_client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);
```

**Testing Authorization:**
```csharp
// Create resource as one user
var user1Resource = await CreateResourceAsUser1();

// Try to access as different user
var user2Token = await GetUser2Token();
_client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", user2Token);

var response = await _client.GetAsync($"/api/endpoint/{user1Resource.Id}");
Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // Should return 404
```

See @TESTING.md for complete testing documentation.
