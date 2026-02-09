---
agents: [plan]
paths:
  - "**/*.cs"
  - "**/*.jsx"
  - "**/*.tsx"
  - "**/*.js"
---

# Plan Agent Context

## Your Role

You design implementation approaches for new features. Your job is to:

1. **Explore the codebase thoroughly** - Understand existing patterns
2. **Identify all affected files** - What needs to be modified or created?
3. **Design the implementation approach** - How should this be built?
4. **Create a step-by-step plan** - Clear instructions for implementation

## Quick Project Overview

**Tech Stack:**
- Backend: ASP.NET Core 9.0 REST API with SQLite
- Web Frontend: React 19 + Vite + Material-UI
- Mobile Frontend: React Native with TypeScript

**Core Architecture Pattern:**
```
Backend:  Controllers → Services → Data/Models → Database
          (thin)      (business    (EF Core)
                       logic)

Frontend: Components → Services → API → Backend
          (UI)        (axios
                       wrapper)
```

**Critical Rules:**
1. Business logic in Services, not Controllers
2. Always use DTOs (never expose Models)
3. Service layer for API calls (no axios in components)
4. Register services in Program.cs
5. Create migrations after model changes

## Where to Look

### Backend Exploration

**File Structure:**
```
backend/
├── Controllers/        # API endpoints (thin, delegate to services)
├── Services/          # Business logic (I*Service.cs + *Service.cs)
├── DTOs/              # Request/response models
├── Models/            # Database entities (EF Core)
├── Data/              # ApplicationDbContext, migrations
└── Program.cs         # Service registration, middleware config
```

**Key Files:**
- `ApplicationDbContext.cs` - Database configuration
- `Program.cs` - Service registration (DI container)
- Existing Controllers - Endpoint patterns
- Existing Services - Business logic patterns

**Patterns to Follow:**
- See @backend/architecture.md for layered architecture
- See @backend/api-conventions.md for API patterns
- See @backend/database.md for EF Core patterns

### Frontend Exploration

**File Structure:**
```
web-frontend/src/
├── components/
│   ├── auth/          # ProtectedRoute
│   └── layout/        # AppLayout, Navbar
├── pages/             # Page components
│   ├── vehicles/      # Vehicle-related pages
│   ├── Dashboard.jsx
│   ├── Login.jsx
│   └── Register.jsx
├── services/          # API service layer
│   ├── api.js         # Axios instance with interceptors
│   ├── authService.js
│   └── vehicleService.js
├── context/
│   └── AuthContext.jsx   # Global auth state
└── utils/             # Utility functions
```

**Key Files:**
- `src/services/api.js` - Axios interceptors (auto-adds JWT token)
- `src/context/AuthContext.jsx` - Auth state management
- Existing services - API call patterns
- Existing pages - Component patterns

**Patterns to Follow:**
- See @frontend/web/react-patterns.md for component patterns
- See @frontend/web/api-integration.md for service layer pattern

## Planning Checklist

### For New Backend Features

1. **Database Changes?**
   - [ ] New entity needed? → Add Model in `backend/Models/`
   - [ ] Add to ApplicationDbContext as DbSet
   - [ ] Configure relationships in OnModelCreating()
   - [ ] Note: Migration needed (`dotnet ef migrations add Name`)

2. **Service Layer?**
   - [ ] Create interface `Services/I*Service.cs`
   - [ ] Implement `Services/*Service.cs`
   - [ ] Note: Must register in Program.cs

3. **API Endpoints?**
   - [ ] Create DTOs for request/response
   - [ ] Create Controller in `backend/Controllers/`
   - [ ] Add `[Authorize]` unless public endpoint
   - [ ] Note: Update backend/API.md documentation

4. **Existing Code?**
   - [ ] Identify files to modify
   - [ ] Check dependencies on those files
   - [ ] Plan for backward compatibility

### For New Frontend Features

1. **New Pages?**
   - [ ] Create in `src/pages/`
   - [ ] Add route to App.jsx
   - [ ] Wrap in ProtectedRoute if auth required

2. **API Integration?**
   - [ ] Create service in `src/services/`
   - [ ] Use shared `api` instance (don't create new axios)
   - [ ] Return `response.data`, not full response

3. **State Management?**
   - [ ] Component state with useState?
   - [ ] Global state needed? (extend AuthContext or create new context)
   - [ ] Forms? → Use React Hook Form + Yup

4. **UI Components?**
   - [ ] Use Material-UI components
   - [ ] Follow existing styling patterns
   - [ ] Show loading states
   - [ ] Handle errors with Snackbar/Alert

## Creating Your Plan

### Plan Structure

```markdown
# Feature: [Feature Name]

## Overview
Brief description of what this feature does and why.

## Backend Changes

### Database
- Create Model: `Vehicle.cs` with properties: Make, Model, Year, VIN
- Add DbSet to ApplicationDbContext
- Configure relationships: User 1-to-many Vehicles
- Migration: `AddVehiclesTable`

### Service Layer
- Create: `IVehicleService.cs` interface
- Implement: `VehicleService.cs` with methods: Create, GetAll, GetById, Update, Delete
- Register in Program.cs: `builder.Services.AddScoped<IVehicleService, VehicleService>()`

### DTOs
- `CreateVehicleRequest.cs` - for POST requests
- `VehicleResponse.cs` - for responses

### API Endpoints
- Controller: `VehicleController.cs`
- POST /api/vehicles - Create vehicle
- GET /api/vehicles - List user's vehicles
- GET /api/vehicles/{id} - Get by ID
- PUT /api/vehicles/{id} - Update vehicle
- DELETE /api/vehicles/{id} - Delete vehicle

## Frontend Changes

### Service Layer
- Create: `src/services/vehicleService.js`
- Methods: getVehicles(), createVehicle(), updateVehicle(), deleteVehicle()

### Pages
- Create: `src/pages/vehicles/VehicleList.jsx` - List all vehicles
- Create: `src/pages/vehicles/VehicleForm.jsx` - Create/edit form
- Update: `src/App.jsx` - Add routes

### Components
- VehicleCard component for displaying vehicle details

## Implementation Steps

1. Backend setup (database & models)
2. Backend service layer
3. Backend API endpoints
4. Frontend service layer
5. Frontend UI components
6. Frontend pages
7. Integration testing
8. Update documentation

## Files to Create
- backend/Models/Vehicle.cs
- backend/Services/IVehicleService.cs
- backend/Services/VehicleService.cs
- backend/DTOs/CreateVehicleRequest.cs
- backend/DTOs/VehicleResponse.cs
- backend/Controllers/VehicleController.cs
- web-frontend/src/services/vehicleService.js
- web-frontend/src/pages/vehicles/VehicleList.jsx
- web-frontend/src/pages/vehicles/VehicleForm.jsx

## Files to Modify
- backend/Data/ApplicationDbContext.cs (add DbSet)
- backend/Program.cs (register service)
- web-frontend/src/App.jsx (add routes)

## Risks & Considerations
- Authorization: Ensure users can only access their own vehicles
- Validation: VIN format validation needed?
- Cascading deletes: What happens to vehicle data when user is deleted?

## Questions for User
- Should VIN be required?
- What vehicle properties are mandatory vs optional?
```

## Common Patterns to Identify

### Backend Patterns

**Service Registration (Program.cs):**
```csharp
builder.Services.AddScoped<IVehicleService, VehicleService>();
```

**Controller Pattern:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Except for login/register
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehicleController(IVehicleService service)
    {
        _service = service;
    }
}
```

**Service Pattern:**
```csharp
public interface IVehicleService
{
    Task<VehicleResponse> CreateAsync(CreateVehicleRequest request, int userId);
}

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

### Frontend Patterns

**Service Pattern:**
```javascript
// src/services/vehicleService.js
import api from './api';

export const vehicleService = {
  async getVehicles() {
    const response = await api.get('/vehicles');
    return response.data; // Return data only
  }
};
```

**Component with Service Call:**
```javascript
import { vehicleService } from '../services/vehicleService';

function VehicleList() {
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchVehicles = async () => {
      try {
        setLoading(true);
        const data = await vehicleService.getVehicles();
        setVehicles(data);
      } catch (err) {
        setError('Failed to load vehicles');
      } finally {
        setLoading(false);
      }
    };
    fetchVehicles();
  }, []);
}
```

## Your Success Criteria

✅ **Good Plan:**
- All files identified (create & modify)
- Clear step-by-step instructions
- Follows existing patterns
- Considers edge cases
- Ready for implementation without questions

❌ **Incomplete Plan:**
- Vague instructions
- Missing files
- Doesn't follow project patterns
- Ignores existing code
- Leaves questions unanswered

## Remember

- **Explore thoroughly** - Read existing code to understand patterns
- **Be specific** - Exact file names, method names
- **Follow patterns** - Don't invent new approaches
- **Think end-to-end** - Backend AND frontend (unless explicitly backend/frontend only)
- **Consider authorization** - Who can access this feature?
- **Plan for errors** - What could go wrong?
