---
agents: [plan, senior-software-engineer, regular-software-engineer]
paths:
  - "backend/**/*.cs"
  - "backend/Models/**/*.cs"
  - "backend/Services/**/*.cs"
  - "backend/Controllers/**/*.cs"
  - "backend/Data/**/*.cs"
---

# Backend Architecture

## Layered Pattern

```
Controllers/        # API endpoints, handle HTTP requests
├── Services/       # Business logic layer (injected via DI)
│   ├── I*Service.cs    # Service interfaces
│   └── *Service.cs     # Service implementations
├── DTOs/          # Data transfer objects (request/response models)
├── Models/        # Database entities (EF Core)
└── Data/          # ApplicationDbContext and migrations
```

## Service Registration (Program.cs)

- All services registered as `Scoped` (one instance per request)
- Services: `IUserService`, `IVehicleService`, `IServiceRecordService`, `IFuelRecordService`
- DbContext configured with SQLite connection string from `appsettings.json`
- JWT authentication middleware configured with token validation

## Database Relationships

- `User` 1-to-many `Vehicle` (cascade delete)
- `Vehicle` 1-to-many `ServiceRecord` (cascade delete)
- `Vehicle` 1-to-many `FuelRecord` (cascade delete)
- `CarMake` 1-to-many `CarModel`

## Auto-Calculations

- Fuel efficiency calculated automatically when creating fuel records (based on previous record)
- Vehicle mileage automatically updated when creating service/fuel records
- Total costs computed in backend before returning to frontend

## Authentication Flow

- JWT tokens generated on login/register with 1-hour expiration (configurable in `appsettings.json`)
- Passwords hashed with BCrypt before storage
- All endpoints except `/api/auth/register` and `/api/auth/login` require `[Authorize]` attribute
- Token validated on each request via JWT Bearer middleware

## Configuration Files

**Backend (`backend/appsettings.json`):**
- Database connection string (SQLite file path)
- JWT configuration (Secret, Issuer, Audience, ExpirationHours)
- **Important**: JWT Secret must be at least 32 characters

**CORS:**
- Backend allows requests from configured origins (configured in Program.cs)
- Default: `http://localhost:5173` and `http://localhost:5175`
- **Important**: If frontend runs on a different port, update CORS policy in `Program.cs`
- See @TROUBLESHOOTING.md → CORS Errors for detailed solutions

## Database Seeding

- Car makes and models are seeded automatically in `ApplicationDbContext.cs`
- Seed data includes 10 makes and 60+ models
- To modify seed data, edit `SeedCarMakesAndModels()` method and create migration
