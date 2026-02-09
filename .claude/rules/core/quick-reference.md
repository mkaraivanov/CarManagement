---
paths:
  - "**/*"
---

# Quick Reference

## Tech Stack

- **Backend**: ASP.NET Core 9.0 REST API with SQLite database
- **Web Frontend**: React 19 + Vite + Material-UI
- **Mobile Frontend**: React Native with TypeScript
- **Testing**: xUnit (backend) + Vitest (frontend)
- **Auth**: JWT Bearer tokens (1-hour expiration)

## Core Architecture Pattern

```
Backend:  Controllers → Services → Data/Models → Database
          (thin)      (business    (EF Core)
                       logic)

Frontend: Components → Services → API → Backend
          (UI)        (axios
                       wrapper)
```

## Critical Rules (Top 5)

1. **Business logic in Services**, not Controllers
2. **Always use DTOs** - never expose Models directly
3. **Service layer for API calls** - no axios directly in components
4. **Register services in Program.cs** - commonly forgotten!
5. **Create migrations** after model changes

## File Structure

### Backend
```
backend/
├── Controllers/        # API endpoints (thin, delegate to services)
├── Services/          # Business logic (I*Service.cs + *Service.cs)
├── DTOs/              # Request/response models
├── Models/            # Database entities (EF Core)
├── Data/              # ApplicationDbContext, migrations
└── Program.cs         # Service registration, middleware config
```

### Frontend
```
web-frontend/src/
├── components/        # Reusable components
├── pages/            # Page components
├── services/         # API service layer (axios wrappers)
├── context/          # Global state (AuthContext)
└── utils/            # Utility functions
```

## Common Commands

### Development
```bash
# Backend
cd backend && dotnet run              # http://localhost:5239

# Frontend
cd web-frontend && npm run dev        # http://localhost:5173
```

### Testing (Always run before committing!)
```bash
# Backend tests (18 tests)
cd backend && dotnet test

# Frontend tests (11 tests)
cd web-frontend && npm test -- --run

# All tests (29 tests)
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

### Database
```bash
# Create migration
cd backend && dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Reset database
rm backend/carmanagement.db && dotnet ef database update
```

## Key Patterns

### Backend Service Registration
```csharp
// Program.cs
builder.Services.AddScoped<IVehicleService, VehicleService>();
```

### Backend Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // All endpoints except login/register
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehicleController(IVehicleService service)
    {
        _service = service; // Injected via DI
    }
}
```

### Frontend Service
```javascript
// src/services/vehicleService.js
import api from './api';

export const vehicleService = {
  async getVehicles() {
    const response = await api.get('/vehicles');
    return response.data; // Return data, not full response
  }
};
```

### Frontend Component with Service
```javascript
import { vehicleService } from '../services/vehicleService';

function VehicleList() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const result = await vehicleService.getVehicles();
        setData(result);
      } catch (err) {
        // Handle error with Snackbar/Alert
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);
}
```

## Authentication

**Token Storage:** localStorage (key: `'token'`)
**Auto-include:** Axios interceptor adds token to all requests
**Auto-logout:** 401 responses clear token and redirect to login

## Database Relationships

- `User` 1-to-many `Vehicle` (cascade delete)
- `Vehicle` 1-to-many `ServiceRecord` (cascade delete)
- `Vehicle` 1-to-many `FuelRecord` (cascade delete)
- `CarMake` 1-to-many `CarModel`

## Configuration

**Backend:** `backend/appsettings.json`
- Database connection string
- JWT secret (must be 32+ characters)
- JWT expiration (default: 1 hour)

**Frontend:** `web-frontend/.env`
- `VITE_API_URL` (default: http://localhost:5239/api)

**CORS:** `backend/Program.cs`
- Default origins: http://localhost:5173, http://localhost:5175
- Update if frontend runs on different port

## Documentation

- **Full API Reference**: @backend/API.md
- **Backend Architecture**: @backend/architecture.md
- **Frontend Patterns**: @frontend/web/react-patterns.md
- **Testing Guide**: @TESTING.md
- **Troubleshooting**: @TROUBLESHOOTING.md
- **Incomplete Features**: @INCOMPLETE_FEATURES.md
