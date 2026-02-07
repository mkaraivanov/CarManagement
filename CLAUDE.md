# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CarManagement is a full-stack vehicle management application with three main components:
- **Backend**: ASP.NET Core 9.0 REST API with SQLite database
- **Web Frontend**: React 19 + Vite + Material-UI
- **Mobile Frontend**: React Native (basic setup, in progress)

## Documentation Structure

This project uses structured documentation to maintain clarity and organization:

### Core Documentation
- **[`CLAUDE.md`](CLAUDE.md)** (this file) - AI assistant guide: architecture, patterns, conventions
- **[`WORKFLOWS.md`](WORKFLOWS.md)** - Development workflows and best practices
- **[`TESTING.md`](TESTING.md)** - Testing strategies and procedures
- **[`TROUBLESHOOTING.md`](TROUBLESHOOTING.md)** - Common issues and solutions
- **[`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md)** - Feature completion tracker

### API & Feature Documentation
- **[`backend/API.md`](backend/API.md)** - Complete API reference with request/response examples
- **[`docs/`](docs/)** - Feature design documents and architecture decisions
  - **[`docs/features/`](docs/features/)** - Detailed feature implementation plans
  - **[`docs/architecture/`](docs/architecture/)** - System architecture and design decisions
  - **[`docs/adr/`](docs/adr/)** - Architecture Decision Records (ADRs)

### Quick Reference
- **New features?** → Start with [`WORKFLOWS.md`](WORKFLOWS.md) → Feature Design Documentation
- **Tests failing?** → Check [`TESTING.md`](TESTING.md)
- **Errors?** → See [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md)
- **Backend API without UI?** → Document in [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md)

**See [`docs/README.md`](docs/README.md) for complete documentation guidelines.**

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
- Backend allows requests from configured origins (configured in Program.cs)
- Default: `http://localhost:5173` and `http://localhost:5175`
- **Important**: If frontend runs on a different port, update CORS policy in `Program.cs`
- See [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) → CORS Errors for detailed solutions

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

**FIRST: Create a feature design doc in `docs/features/` if this is a significant change!**

**See [`WORKFLOWS.md`](WORKFLOWS.md) → Backend Development Workflow for complete steps.**

**Quick checklist:**
1. Create Model in `backend/Models/`
2. Add DbSet to `ApplicationDbContext.cs`
3. Configure relationships in `OnModelCreating()`
4. Create migration: `dotnet ef migrations add AddEntityName`
5. Apply migration: `dotnet ef database update`
6. Create DTOs in `backend/DTOs/`
7. Create Service interface and implementation
8. **Register Service in `Program.cs`** (commonly forgotten!)
9. Create Controller in `backend/Controllers/`
10. Update `backend/API.md`

### When Adding New API Endpoints

**FIRST: Ensure there's a feature design doc in `docs/features/` for this endpoint!**

**See [`WORKFLOWS.md`](WORKFLOWS.md) → When Adding New API Endpoints for complete workflow.**

**Key points:**
- Use DTOs for request/response (never expose Models directly)
- Add `[Authorize]` attribute unless endpoint should be public
- Return appropriate status codes (200, 201, 400, 404, 500)
- Update `backend/API.md` with documentation
- Mark as "⚠️ No Frontend Yet" in API.md if no UI exists
- Add to [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md) if backend-only

### Frontend API Integration

**See [`WORKFLOWS.md`](WORKFLOWS.md) → Frontend Development Workflow for complete patterns.**

**Key pattern:**
1. Create service method in `src/services/` (never call axios directly from components)
2. Use shared `api` instance from `src/services/api.js`
3. Handle errors with try-catch in components
4. Show user feedback with Material-UI Snackbar/Alert

### Database Seeding

- Car makes and models are seeded automatically in `ApplicationDbContext.cs`
- Seed data includes 10 makes and 60+ models
- To modify seed data, edit `SeedCarMakesAndModels()` method and create migration

## Development Best Practices

This project is a solo development effort with AI assistance. These practices help maintain code quality and keep the AI effective.

### Feature Completion Definition

**CRITICAL RULE: A feature is NOT considered complete/implemented until it has BOTH backend AND frontend components.**

**See [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md) for tracking incomplete features.**

**Key points:**
- Backend-only APIs without UI = **incomplete**
- Only features with working end-to-end integration = **complete**
- Document incomplete features in [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md)
- Mark backend-only endpoints as "⚠️ No Frontend Yet" in `backend/API.md`

### Development Workflows

**See [`WORKFLOWS.md`](WORKFLOWS.md) for complete workflows including:**
- Feature Design Documentation process
- Git workflow and branching strategy
- Backend development workflow (entities, migrations, services)
- Frontend development workflow (components, services, API integration)
- Before Committing - AI Checklist
- Database & migration best practices

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

## Testing & Debugging

**See [`TESTING.md`](TESTING.md) for complete testing documentation including:**
- Running automated backend tests (xUnit integration tests)
- Manual testing procedures (full flow, API with curl)
- Writing new tests (patterns and examples)
- Test coverage details (13 tests, all passing)

**See [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) for debugging help:**
- Port conflicts
- Database issues
- Authentication problems
- CORS errors
- Build and compilation errors
- API communication issues

**Quick test command:**
```bash
cd backend/Backend.Tests && dotnet test
```

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

## Incomplete Features Tracker

**See [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md) for complete tracking of features that have backend implementation but no frontend UI.**

**Process for managing incomplete features:**
- When implementing backend-only APIs, add to [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md)
- Mark endpoints as "⚠️ No Frontend Yet" in [`backend/API.md`](backend/API.md)
- Remove from incomplete list only when fully functional end-to-end (backend + frontend + testing)

**Planned future enhancements:**
- Service Records UI
- Fuel Records UI
- Insurance management
- Expense analytics dashboard
- Reminders and notifications

See [`INCOMPLETE_FEATURES.md`](INCOMPLETE_FEATURES.md) for detailed status and implementation plans.
