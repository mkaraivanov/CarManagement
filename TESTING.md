# Testing Guide

This document covers automated testing and manual testing procedures for the CarManagement project.

## Table of Contents
- [Automated Testing](#automated-testing)
- [Manual Testing](#manual-testing)
- [Writing New Tests](#writing-new-tests)

## Automated Testing

### Backend Integration Tests

**Test Project Location:** `backend/Backend.Tests/`

**Running Tests:**
```bash
cd backend/Backend.Tests
dotnet test                    # Run all tests
dotnet test --verbosity normal # Run with detailed output
dotnet test --filter "FullyQualifiedName~Authentication"  # Run specific test class
```

### Test Infrastructure

- **xUnit** testing framework
- **WebApplicationFactory** for integration testing
- **In-Memory Database** automatically used in Testing environment
- Tests run against real API with isolated database per test run

### Current Test Coverage (13 tests - All Passing ✅)

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

### Important Testing Notes

- Each test gets a fresh in-memory database (isolation)
- Test environment automatically set via `TestWebApplicationFactory`
- No need to manually configure database - handled by Program.cs
- Authorization tests verify users can only access their own data
- Backend returns 404 (not 403) for unauthorized access (security best practice)

### Before Committing Code

```bash
# Always run tests before pushing
cd backend/Backend.Tests && dotnet test
# All tests must pass
```

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

### Test Patterns

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

**Testing Validation:**
```csharp
// Invalid request
var invalidRequest = new { /* missing required fields */ };
var response = await _client.PostAsJsonAsync("/api/endpoint", invalidRequest);

Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
var errorResponse = await response.Content.ReadAsStringAsync();
Assert.Contains("validation error message", errorResponse);
```

## Manual Testing

### Full Flow Test

1. Start backend: `cd backend && dotnet run`
2. Start frontend: `cd web-frontend && npm run dev`
3. Open `http://localhost:5173`
4. Register new account
5. Add a vehicle
6. View vehicle details

### API Testing with curl

See `GETTING_STARTED.md` for complete curl examples including:
- Register/login flow
- Creating vehicles with authentication
- Fetching user data

**Quick Test:**
```bash
# Register a user
curl -X POST http://localhost:5239/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test123!","email":"test@example.com"}'

# Save the token from the response, then:
TOKEN="your_jwt_token_here"

# Create a vehicle
curl -X POST http://localhost:5239/api/vehicles \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "make":"Toyota",
    "model":"Camry",
    "year":2020,
    "vin":"1HGBH41JXMN109186",
    "licensePlate":"ABC123",
    "purchaseDate":"2020-01-15",
    "purchasePrice":25000
  }'

# Get vehicles
curl -X GET http://localhost:5239/api/vehicles \
  -H "Authorization: Bearer $TOKEN"
```

### Testing Checklist

Before releasing/merging a feature:

- [ ] All automated tests pass (`dotnet test`)
- [ ] Manual end-to-end test completed successfully
- [ ] Test with different user accounts (authorization)
- [ ] Test validation errors (invalid inputs)
- [ ] Test edge cases (empty data, long strings, special characters)
- [ ] Test authentication (with/without token, expired token)
- [ ] Check browser console for errors
- [ ] Check backend console for exceptions
- [ ] Test on clean database (reset and recreate)
- [ ] Verify CORS works (frontend can call backend)

### Frontend Testing

Currently no automated frontend tests. Manual testing approach:

1. **Visual Testing**: Check UI renders correctly in browser
2. **Form Validation**: Try submitting invalid data
3. **Error Handling**: Check error messages display properly
4. **Loading States**: Verify spinners/loading indicators work
5. **Navigation**: Test all links and routing
6. **Responsive Design**: Test on different screen sizes (browser DevTools)

### Mobile Testing

**Test Project Location:** `mobile-frontend/CarManagementMobile/__tests__/`

**Running Tests:**
```bash
cd mobile-frontend/CarManagementMobile

# Run all tests
npm test

# Run tests in watch mode (for development)
npm test -- --watch

# Run tests with coverage
npm test -- --coverage

# Run specific test file
npm test -- userService.test.ts
```

### Mobile Test Infrastructure

- **Jest** testing framework (configured for React Native)
- **react-test-renderer** for component testing
- **Mock** implementations for services, navigation, and context
- Tests run in Node environment (no device/emulator needed)

### Current Mobile Test Coverage

**User Service Tests:**
- getAll() - Fetches all users with statistics
- getById(id) - Fetches specific user
- update(id, data) - Updates user information
- delete(id) - Deletes user account
- Error handling for each method

**User List Screen Tests:**
- Rendering: loading states, user list display, empty states
- User identification: "You" badge for current user
- Statistics display: vehicle, service, and fuel counts
- User interactions: edit navigation, delete confirmation
- Self-delete prevention: current user cannot delete themselves
- Date formatting and edge cases

**User Form Screen Tests:**
- Form rendering: loading, field population, info banner
- Form validation: required fields, email format
- User interactions: save, cancel, navigation
- Error handling during save/load operations

### Manual Mobile Testing

```bash
cd mobile-frontend/CarManagementMobile

# Test on Android
npm run android
# App should launch on emulator/device

# Test on iOS (macOS only)
npm run ios
# App should launch on simulator

# Test hot reload
# Make a change to a component and save
# App should update without full restart
```

**Mobile Test Checklist:**
- [ ] Authentication flow (login, register, logout)
- [ ] Vehicle management (list, add, edit, delete)
- [ ] Service records (list, add, view)
- [ ] Fuel records (list, add, view)
- [ ] User management (list, edit, delete)
- [ ] Navigation between screens
- [ ] Error handling and alerts
- [ ] Loading states
- [ ] Pull-to-refresh functionality
- [ ] Form validation

## Performance Testing

### Backend Performance

**Database Query Performance:**
```bash
# Enable detailed logging in appsettings.json
"Logging": {
  "LogLevel": {
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}

# Watch console for slow queries (>100ms)
```

**Memory Usage:**
- Monitor backend process memory with Activity Monitor (macOS) or Task Manager (Windows)
- Watch for memory leaks during extended testing sessions
- Backend should stay under 200MB for typical use

### Frontend Performance

- Use React DevTools Profiler to identify slow components
- Check Network tab for slow API calls
- Monitor bundle size: `cd web-frontend && npm run build` (should be <1MB for main bundle)

## Test Data

### Sample Test Data

Located in `backend/TestData/`:
- Documentation for sample data scenarios
- May include sample files for testing file uploads

### Creating Test Users

```bash
# Quick script to create multiple test users
for i in {1..5}; do
  curl -X POST http://localhost:5239/api/auth/register \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"user$i\",\"password\":\"Test123!\",\"email\":\"user$i@example.com\"}"
done
```

### Resetting Test Database

```bash
# Delete database and recreate
rm backend/carmanagement.db
cd backend && dotnet ef database update

# Or backup and restore
cp backend/carmanagement.db.backup backend/carmanagement.db
```
