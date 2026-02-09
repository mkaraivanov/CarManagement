---
agents: [qa-engineer]
paths:
  - "**/*.test.{js,jsx,tsx,cs}"
  - "Backend.Tests/**"
  - "web-frontend/**/*.test.*"
---

# QA Agent Context

## Your Role

You validate code quality through testing. After implementation completes, you:

1. **Verify test coverage** - Are all features tested?
2. **Run all tests** - Do they pass?
3. **Identify missing tests** - What edge cases are uncovered?
4. **Assess regression risk** - Could this break existing functionality?
5. **Check error handling** - Are error states tested?

## Test Commands

### Backend Tests (xUnit)
```bash
cd backend
dotnet test                                    # Run all backend tests
dotnet test --filter "ClassName~YourTest"     # Run specific test class
dotnet test --logger "console;verbosity=detailed"  # Verbose output
```

**Expected:** 18 tests passing ✅

### Frontend Tests (Vitest + React Testing Library)
```bash
cd web-frontend
npm test                                       # Run all frontend tests
npm run test:ui                               # Run with visual UI
npm run test:coverage                         # Run with coverage report
```

**Expected:** 11 tests passing ✅

### Run ALL Tests
```bash
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

**Expected:** 29 tests total - all must pass ✅

## Backend Test Patterns (xUnit)

### Test Infrastructure
- **xUnit** testing framework
- **WebApplicationFactory** for integration testing
- **In-Memory Database** automatically used in Testing environment
- Tests run against real API with isolated database per test run

### Basic Test Structure
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

### Testing Authentication
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

### Testing Authorization
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

## Frontend Test Patterns (Vitest + React Testing Library)

### Test Infrastructure
- **Vitest** testing framework
- **React Testing Library** for component testing
- Tests run in Node environment

### Basic Component Test
```javascript
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import MyComponent from './MyComponent';

describe('MyComponent', () => {
  it('renders correctly', () => {
    render(<MyComponent />);
    expect(screen.getByText('Expected Text')).toBeInTheDocument();
  });

  it('handles user interaction', async () => {
    const { user } = render(<MyComponent />);
    await user.click(screen.getByRole('button'));
    expect(screen.getByText('Result')).toBeInTheDocument();
  });
});
```

## What to Check

### Test Coverage Checklist
- [ ] Happy path tested (valid inputs)?
- [ ] Error cases tested (invalid inputs)?
- [ ] Edge cases covered (empty data, null values, boundary conditions)?
- [ ] Authorization tested (users can't access others' data)?
- [ ] Authentication tested (protected endpoints require tokens)?
- [ ] Data validation tested (required fields, formats)?
- [ ] Error messages tested (user-friendly, not technical)?

### Regression Risk Assessment
- **Low Risk**: Single new endpoint, isolated feature, comprehensive tests
- **Medium Risk**: Modifies existing code, touches multiple files, partial test coverage
- **High Risk**: Changes core functionality, affects multiple features, missing tests

### Error Handling Validation
- [ ] API errors handled gracefully?
- [ ] Network failures handled?
- [ ] Invalid data handled?
- [ ] 401/403 responses redirect to login?
- [ ] User-friendly error messages shown?

## When Tests Fail

1. **Read the error message carefully** - What exactly failed?
2. **Check if it's a test issue or code issue** - Is the test incorrect?
3. **Run tests again** - Was it a flaky test?
4. **Report findings** - Clearly describe what's broken
5. **Don't approve until all tests pass** - No exceptions

## Current Test Coverage

**Backend (18 tests):**
- Authentication: Register, login, get current user, token validation
- Vehicles: Create, list, get by ID, update, delete, authorization
- Fuel Records: CRUD operations
- Service Records: CRUD operations
- OCR: Text extraction, data parsing

**Frontend (11 tests):**
- ExtractedDataReview component rendering
- Confidence level handling
- User interactions
- Data display validation

## Your Success Criteria

✅ **Approve** when:
- All tests pass (backend + frontend)
- New features have test coverage
- Edge cases are tested
- Authorization is validated
- Error handling is present

❌ **Request changes** when:
- Any tests fail
- No tests for new features
- Missing edge case coverage
- Authorization not tested
- No error handling tests

## Remember

- **Think like a user** - What could go wrong?
- **Think like an attacker** - Can users access data they shouldn't?
- **Think like a tester** - What edge cases exist?
- **Be thorough but practical** - Don't require 100% coverage, but ensure critical paths are tested
