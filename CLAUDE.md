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

## Testing the Application

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
