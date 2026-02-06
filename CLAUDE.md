# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CarManagement is a full-stack vehicle management application with three main components:
- **Backend**: ASP.NET Core 9.0 REST API with SQLite database
- **Web Frontend**: React 19 + Vite + Material-UI
- **Mobile Frontend**: React Native (basic setup, in progress)

## Development Commands

### Backend (ASP.NET Core)
```bash
cd backend
dotnet run                    # Start backend on http://localhost:5239
dotnet build                  # Build the project
dotnet watch run             # Run with hot reload
dotnet ef database update    # Apply pending migrations
dotnet ef migrations add <Name>  # Create new migration
```

### Web Frontend (React + Vite)
```bash
cd web-frontend
npm run dev                  # Start dev server on http://localhost:5173
npm run build                # Production build
npm run lint                 # Run ESLint
npm run preview              # Preview production build
```

### Mobile Frontend (React Native)
```bash
cd mobile-frontend/CarManagementMobile
npm start                    # Start Metro bundler
npm run android             # Run on Android emulator
npm run ios                 # Run on iOS simulator

# iOS CocoaPods setup (first time or after dependency changes)
bundle install              # Install Ruby bundler for CocoaPods
bundle exec pod install     # Install iOS native dependencies
```

### Database Management
```bash
# Reset database (deletes all data)
rm backend/carmanagement.db
cd backend && dotnet ef database update

# View database
cd backend && sqlite3 carmanagement.db
```

## Architecture

### Backend Architecture (ASP.NET Core)

**Layered Pattern:**
```
Controllers/        # API endpoints, handle HTTP requests
├── Services/       # Business logic layer (injected via DI)
│   ├── I*Service.cs    # Service interfaces
│   └── *Service.cs     # Service implementations
├── DTOs/          # Data transfer objects (request/response models)
├── Models/        # Database entities (EF Core)
└── Data/          # ApplicationDbContext and migrations
```

**Service Registration (Program.cs):**
- All services registered as `Scoped` (one instance per request)
- Services: `IUserService`, `IVehicleService`, `IServiceRecordService`, `IFuelRecordService`
- DbContext configured with SQLite connection string from `appsettings.json`
- JWT authentication middleware configured with token validation

**Database Relationships:**
- `User` 1-to-many `Vehicle` (cascade delete)
- `Vehicle` 1-to-many `ServiceRecord` (cascade delete)
- `Vehicle` 1-to-many `FuelRecord` (cascade delete)
- `CarMake` 1-to-many `CarModel`

**Auto-Calculations:**
- Fuel efficiency calculated automatically when creating fuel records (based on previous record)
- Vehicle mileage automatically updated when creating service/fuel records
- Total costs computed in backend before returning to frontend

**Authentication Flow:**
- JWT tokens generated on login/register with 1-hour expiration (configurable in `appsettings.json`)
- Passwords hashed with BCrypt before storage
- All endpoints except `/api/auth/register` and `/api/auth/login` require `[Authorize]` attribute
- Token validated on each request via JWT Bearer middleware

### Frontend Architecture (React)

**Structure:**
```
src/
├── components/
│   ├── auth/          # ProtectedRoute component
│   └── layout/        # AppLayout, Navbar
├── pages/            # Page components
│   ├── vehicles/     # Vehicle-related pages (List, Details, VehicleForm)
│   ├── Dashboard.jsx
│   ├── Login.jsx
│   └── Register.jsx
├── services/         # API service layer
│   ├── api.js        # Axios instance with interceptors
│   ├── authService.js
│   └── vehicleService.js
├── context/
│   └── AuthContext.jsx   # Global auth state
└── utils/            # Utility functions
```

**API Communication:**
- Axios instance in `src/services/api.js` configured with base URL from `.env`
- **Request Interceptor**: Automatically adds JWT token from localStorage to all requests
- **Response Interceptor**: Catches 401 responses, clears token, redirects to login
- All API calls go through service layer (don't call axios directly from components)

**Authentication Pattern:**
- `AuthContext` provides: `user`, `login`, `register`, `logout`, `isAuthenticated`, `loading`
- `ProtectedRoute` wrapper checks `isAuthenticated` before rendering routes
- Token stored in localStorage (key: `'token'`)
- On app mount, AuthContext fetches current user if token exists

**State Management:**
- React Context for auth state (no Redux/Zustand)
- Component-level state with useState for UI state
- React Hook Form + Yup for form validation

### Configuration Files

**Backend (`backend/appsettings.json`):**
- Database connection string (SQLite file path)
- JWT configuration (Secret, Issuer, Audience, ExpirationHours)
- **Important**: JWT Secret must be at least 32 characters

**Frontend (`web-frontend/.env`):**
- `VITE_API_URL`: Backend API base URL (default: `http://localhost:5239/api`)

**CORS:**
- Backend allows requests from `http://localhost:5173` only (configured in Program.cs)
- To add additional origins, modify the CORS policy in `Program.cs`

## API Endpoints

See `backend/API.md` for complete documentation with request/response examples.

**Key Endpoints:**
- `POST /api/auth/register` - Create account (returns JWT + user)
- `POST /api/auth/login` - Login (returns JWT + user)
- `GET /api/users/me` - Get current user (requires auth)
- `GET /api/vehicles` - List user's vehicles (requires auth)
- `POST /api/vehicles` - Create vehicle (requires auth)
- `GET /api/vehicles/{id}/fuel-efficiency` - Get fuel stats

**Authentication Header:**
```bash
Authorization: Bearer <jwt_token>
```

## Important Patterns & Conventions

### When Adding New Entities

1. **Create Model** in `backend/Models/` with EF Core annotations
2. **Add DbSet** to `ApplicationDbContext.cs`
3. **Configure Relationships** in `OnModelCreating()` method
4. **Create Migration**: `dotnet ef migrations add AddEntityName`
5. **Apply Migration**: `dotnet ef database update`
6. **Create DTOs** for request/response in `backend/DTOs/`
7. **Create Service Interface** and implementation in `backend/Services/`
8. **Register Service** in `Program.cs` with DI
9. **Create Controller** in `backend/Controllers/`

### When Adding New API Endpoints

1. Add method to appropriate service interface + implementation
2. Add controller action with proper HTTP verb attribute (`[HttpGet]`, `[HttpPost]`, etc.)
3. Add `[Authorize]` attribute unless endpoint should be public
4. Use DTOs for request/response (never expose Models directly)
5. Return appropriate status codes (200, 201, 400, 404, 500)
6. Update `backend/API.md` with endpoint documentation

### Frontend API Integration

1. Create service method in appropriate service file (e.g., `vehicleService.js`)
2. Use the shared `api` instance from `src/services/api.js`
3. Handle errors with try-catch in components
4. Show user feedback with Material-UI Snackbar/Alert components

### Database Seeding

- Car makes and models are seeded automatically in `ApplicationDbContext.cs`
- Seed data includes 10 makes and 60+ models
- To modify seed data, edit `SeedCarMakesAndModels()` method and create migration

## Best Practices & Workflows

This project is a solo development effort with AI assistance. These practices help maintain code quality and keep the AI effective.

### Git Workflow

**Branching Strategy:**
- Create feature branches for significant changes: `feature/add-fuel-ui`, `fix/auth-bug`, `refactor/service-layer`
- Keeps `main` branch clean and makes it easier to abandon unsuccessful experiments
- Can push to `main` directly for small fixes; use branches for larger features
- Delete branches after merging or abandoning

**Commit Practices:**
- Commit when code works and is tested (not broken code)
- Clear commit messages describing what and why
- Test end-to-end before pushing

### Backend Development Workflow

**When Adding/Modifying Database Entities (CRITICAL WORKFLOW):**

This is a common area where AI makes mistakes. Follow these steps in order:

1. Create or update Model in `backend/Models/` with EF Core annotations
2. Update `ApplicationDbContext.cs`:
   - Add `DbSet<EntityName>` property if new entity
   - Configure relationships in `OnModelCreating()` if needed
3. **Create migration**: `dotnet ef migrations add DescriptiveName`
4. **Apply migration**: `dotnet ef database update`
5. Create DTOs in `backend/DTOs/` (request and response DTOs)
6. Create Service interface in `backend/Services/IEntityService.cs`
7. Implement Service in `backend/Services/EntityService.cs`
8. **Register service in `Program.cs`** (AI commonly forgets this step!)
   ```csharp
   builder.Services.AddScoped<IEntityService, EntityService>();
   ```
9. Create Controller in `backend/Controllers/`
10. Test with curl or frontend
11. Update `backend/API.md` with new endpoints

**Critical Backend Rules:**

- **Business logic belongs in Services**, not Controllers (Controllers should be thin routing layers)
- **Never expose Models directly** - always use DTOs for API requests/responses
- **Always create migrations** after model changes (don't skip this!)
- **Always register new services** in `Program.cs` DI container
- **Add `[Authorize]` attribute** to all endpoints except login/register
- **Return proper HTTP status codes**: 200 (OK), 201 (Created), 400 (Bad Request), 404 (Not Found), 500 (Server Error)
- **Validate inputs** using Data Annotations in DTOs

**Common AI Mistakes on Backend:**
- ❌ Forgetting to create migration after model changes
- ❌ Not registering new services in `Program.cs` DI container
- ❌ Putting business logic in Controllers instead of Services
- ❌ Exposing Models directly instead of using DTOs
- ❌ Modifying existing migrations (always create new ones)
- ❌ Not adding `[Authorize]` to protected endpoints

### Frontend Development Workflow

**When Adding Features:**

1. API calls must go through service layer (`src/services/`), **never call axios directly in components**
2. Use `AuthContext` for authentication state
3. Forms must use React Hook Form + Yup for validation
4. Show loading states for async operations
5. Display user-friendly error messages using Material-UI Snackbar/Alert

**Frontend Patterns to Follow:**

- Protected pages wrapped in `<ProtectedRoute>`
- Service files return `response.data`, not the full axios response
- JWT token handled automatically by axios interceptors in `api.js`
- Use try-catch blocks for API calls in components
- Clear error messages for users (not raw error objects)

**Common AI Mistakes on Frontend:**
- ❌ Calling axios directly from components (use service layer)
- ❌ Not showing loading states during async operations
- ❌ Exposing technical errors to users
- ❌ Not using React Hook Form for complex forms
- ❌ Breaking the service layer pattern

### Before Committing - AI Checklist

Run through this checklist before every commit:

**Must Verify:**
- [ ] If database models changed: migration created AND applied
- [ ] If new service created: registered in `Program.cs` DI container
- [ ] Backend compiles: `cd backend && dotnet build`
- [ ] Frontend lints: `cd web-frontend && npm run lint`
- [ ] Tested end-to-end (both UI and API working together)
- [ ] `backend/API.md` updated if endpoints changed
- [ ] No console errors in browser developer tools
- [ ] No exceptions in backend console output

**Common Checks:**
- Backend running on correct port (5239)
- Frontend running on correct port (5173)
- CORS configured for frontend URL
- JWT token being sent in requests (check Network tab)
- Database file exists and has tables

### Database & Migration Best Practices

**Migration Workflow:**
- **Never modify existing migrations** that have been committed - create new ones instead
- Test migrations locally before committing
- Backup database before major schema changes: `cp backend/carmanagement.db backend/carmanagement.db.backup`
- If migration fails, rollback and fix:
  ```bash
  dotnet ef database update PreviousMigrationName
  dotnet ef migrations remove
  # Fix the issue, then create new migration
  dotnet ef migrations add FixedMigrationName
  dotnet ef database update
  ```

**Database Maintenance:**
- SQLite database file: `backend/carmanagement.db`
- Connection string in `backend/appsettings.json`
- To reset database completely: delete file and run `dotnet ef database update`

### Security Best Practices

**Authentication & Authorization:**
- JWT secret must be 32+ characters (already configured correctly)
- All endpoints except `/api/auth/register` and `/api/auth/login` need `[Authorize]` attribute
- Validate all user inputs in DTOs (backend) and forms (frontend)
- Never commit secrets, API keys, or production connection strings
- Use different JWT secrets for development and production

**Token Handling:**
- Tokens stored in localStorage (key: `'token'`)
- Token automatically included in requests via axios interceptor
- 401 responses automatically clear token and redirect to login
- Token expiration: 1 hour (configurable in `appsettings.json`)

### Code Quality & Consistency

**Follow Existing Patterns:**
- Match the architectural patterns already in the codebase
- Controllers → Services → Data/Models on backend
- Components → Services → API on frontend
- Keep DTOs in sync between requests and responses
- Use existing naming conventions (PascalCase for C#, camelCase for JavaScript)

**Code Organization:**
- Extract reusable logic into utility functions or custom hooks
- Keep components focused and small (single responsibility)
- Remove unused imports and dead code
- Group related files together

**API Documentation:**
- Update `backend/API.md` whenever adding or changing endpoints
- Include request/response examples
- Document required vs optional fields
- Note authorization requirements

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

**Test Infrastructure:**
- **xUnit** testing framework
- **WebApplicationFactory** for integration testing
- **In-Memory Database** automatically used in Testing environment
- Tests run against real API with isolated database per test run

**Current Test Coverage (13 tests - All Passing ✅):**

*Authentication Tests (6):*
- Register with valid data returns token and user
- Register with duplicate username returns bad request
- Login with valid credentials returns token
- Login with invalid password returns unauthorized
- Get current user with valid token returns user data
- Get current user without token returns unauthorized

*Vehicle Tests (7):*
- Create vehicle with valid data returns created vehicle
- Create vehicle without token returns unauthorized
- Get vehicles returns only user's vehicles (authorization)
- Get vehicle by ID returns vehicle
- Update vehicle with valid data returns updated vehicle
- Delete vehicle with valid ID returns no content
- Delete vehicle of another user returns not found (authorization)

**Writing New Tests:**

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

**Important Testing Notes:**
- Each test gets a fresh in-memory database (isolation)
- Test environment automatically set via `TestWebApplicationFactory`
- No need to manually configure database - handled by Program.cs
- Authorization tests verify users can only access their own data
- Backend returns 404 (not 403) for unauthorized access (security best practice)

**Before Committing Code:**
```bash
# Always run tests before pushing
cd backend/Backend.Tests && dotnet test
# All tests must pass
```

## Testing the Application Manually

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

### Troubleshooting

**Port conflicts:**
```bash
# Kill backend process (port 5239)
lsof -ti:5239 | xargs kill -9

# Kill frontend process (port 5173)
lsof -ti:5173 | xargs kill -9
```

**Database corruption:**
```bash
# Delete and recreate database
rm backend/carmanagement.db
cd backend && dotnet ef database update
```

**Authentication issues:**
- Clear browser localStorage to remove stale tokens
- Verify JWT Secret in `appsettings.json` is at least 32 characters
- Check token expiration in `appsettings.json` (default: 1 hour)

**401 Unauthorized errors:**
- Verify token is being sent in Authorization header
- Check token hasn't expired
- Ensure user making request owns the resource (vehicles, records)

## Development Notes

### Adding Copilot/AI Instructions

Copilot instructions are in `.github/copilot-instructions.md`. Key points from those instructions:
- Backend uses service layer pattern (Controllers delegate to Services)
- JWT auth required for all endpoints except login/register
- Auto-calculations happen in backend services (fuel efficiency, mileage updates)
- Frontend uses AuthContext and protected routes
- API URL configurable via environment variables

### Mobile Development

Mobile frontend is currently a basic React Native setup. The structure follows standard React Native patterns:
- Start Metro bundler first: `npm start`
- Run platform-specific commands in separate terminal
- iOS requires CocoaPods setup: `bundle install && bundle exec pod install`

### Future Enhancements (Not Yet Implemented)

Based on `README.md`, planned features include:
- Service Records UI (Phase 5)
- Fuel Records UI (Phase 6)
- Insurance management
- Expense analytics dashboard
- Reminders and notifications

When implementing these, follow existing patterns for Vehicle management.
