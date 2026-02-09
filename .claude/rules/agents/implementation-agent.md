---
agents: [senior-software-engineer, regular-software-engineer]
paths:
  - "**/*.cs"
  - "**/*.jsx"
  - "**/*.tsx"
  - "**/*.js"
---

# Implementation Agent Context

## Your Role

You implement features following the approved plan. Focus on:

1. **Follow the plan** - Implement exactly what was designed
2. **Follow project patterns** - Match existing code style
3. **Write clean code** - Readable, maintainable
4. **Handle errors** - Graceful error handling
5. **No testing** - QA agent will validate tests (Phase 4)

## Critical Patterns to Follow

### Backend Architecture

**Layered Pattern:**
```
Controllers/        # API endpoints (thin, just delegate)
├── Services/       # Business logic (injected via DI)
│   ├── I*Service.cs    # Interfaces
│   └── *Service.cs     # Implementations
├── DTOs/          # Data transfer objects
├── Models/        # Database entities (EF Core)
└── Data/          # ApplicationDbContext, migrations
```

**Critical Rules:**
1. **Business logic in Services**, not Controllers
2. **Never expose Models** - always use DTOs
3. **Register services in Program.cs** (commonly forgotten!)
4. **Create migrations** after model changes
5. **Add [Authorize]** to protected endpoints

### Frontend Architecture

**Structure:**
```
src/
├── components/     # Reusable components
├── pages/         # Page components
├── services/      # API service layer (axios wrappers)
├── context/       # Global state (AuthContext)
└── utils/         # Utility functions
```

**Critical Rules:**
1. **Service layer for API calls** - never axios directly in components
2. **Services return response.data** - not full axios response
3. **Show loading states** - during async operations
4. **Handle errors gracefully** - user-friendly messages
5. **Use React Hook Form + Yup** - for complex forms

## Common Mistakes to Avoid

### Backend ❌

- ❌ Business logic in Controllers (belongs in Services)
- ❌ Models exposed directly (use DTOs)
- ❌ Forgetting to create migration after model changes
- ❌ Not registering services in Program.cs
- ❌ Missing [Authorize] on protected endpoints
- ❌ Modifying existing migrations (create new ones)

### Frontend ❌

- ❌ Calling axios directly from components (use service layer)
- ❌ Service returns full axios response (return data only)
- ❌ No loading states during async operations
- ❌ Technical errors shown to users
- ❌ Not using React Hook Form for complex forms

## Implementation Patterns

### Backend: Adding New Entity

**1. Create Model**
```csharp
// backend/Models/Vehicle.cs
public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string VIN { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
```

**2. Add to ApplicationDbContext**
```csharp
// backend/Data/ApplicationDbContext.cs
public DbSet<Vehicle> Vehicles { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure relationships
    modelBuilder.Entity<User>()
        .HasMany(u => u.Vehicles)
        .WithOne(v => v.User)
        .HasForeignKey(v => v.UserId)
        .OnDelete(DeleteBehavior.Cascade);
}
```

**3. Create Migration**
```bash
cd backend
dotnet ef migrations add AddVehiclesTable
dotnet ef database update
```

**4. Create DTOs**
```csharp
// backend/DTOs/CreateVehicleRequest.cs
public class CreateVehicleRequest
{
    [Required]
    public string Make { get; set; }

    [Required]
    public string Model { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }
}

// backend/DTOs/VehicleResponse.cs
public class VehicleResponse
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}
```

**5. Create Service**
```csharp
// backend/Services/IVehicleService.cs
public interface IVehicleService
{
    Task<VehicleResponse> CreateAsync(CreateVehicleRequest request, int userId);
    Task<IEnumerable<VehicleResponse>> GetAllAsync(int userId);
}

// backend/Services/VehicleService.cs
public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request, int userId)
    {
        var vehicle = new Vehicle
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            UserId = userId
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return new VehicleResponse
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year
        };
    }
}
```

**6. Register Service in Program.cs**
```csharp
// backend/Program.cs
builder.Services.AddScoped<IVehicleService, VehicleService>();
```

**7. Create Controller**
```csharp
// backend/Controllers/VehicleController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(CreateVehicleRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var vehicle = await _vehicleService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var vehicles = await _vehicleService.GetAllAsync(userId);
        return Ok(vehicles);
    }
}
```

### Frontend: Adding API Integration

**1. Create Service**
```javascript
// src/services/vehicleService.js
import api from './api';

export const vehicleService = {
  async getVehicles() {
    const response = await api.get('/vehicles');
    return response.data; // Return data only
  },

  async createVehicle(vehicleData) {
    const response = await api.post('/vehicles', vehicleData);
    return response.data;
  },

  async updateVehicle(id, vehicleData) {
    const response = await api.put(`/vehicles/${id}`, vehicleData);
    return response.data;
  },

  async deleteVehicle(id) {
    await api.delete(`/vehicles/${id}`);
  }
};
```

**2. Create Page Component**
```javascript
// src/pages/vehicles/VehicleList.jsx
import { useState, useEffect } from 'react';
import { vehicleService } from '../../services/vehicleService';
import {
  Container,
  Typography,
  CircularProgress,
  Alert,
  Grid
} from '@mui/material';

export default function VehicleList() {
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchVehicles = async () => {
      try {
        setLoading(true);
        setError('');
        const data = await vehicleService.getVehicles();
        setVehicles(data);
      } catch (err) {
        setError('Failed to load vehicles. Please try again.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchVehicles();
  }, []);

  if (loading) {
    return (
      <Container sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error) {
    return (
      <Container sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        My Vehicles
      </Typography>
      <Grid container spacing={2}>
        {vehicles.map(vehicle => (
          <Grid item xs={12} md={6} key={vehicle.id}>
            {/* Vehicle card component */}
          </Grid>
        ))}
      </Grid>
    </Container>
  );
}
```

**3. Create Form Component (React Hook Form + Yup)**
```javascript
// src/pages/vehicles/VehicleForm.jsx
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { TextField, Button, Box } from '@mui/material';
import { vehicleService } from '../../services/vehicleService';

const schema = yup.object({
  make: yup.string().required('Make is required'),
  model: yup.string().required('Model is required'),
  year: yup.number()
    .required('Year is required')
    .min(1900, 'Year must be 1900 or later')
    .max(2100, 'Year must be 2100 or earlier')
});

export default function VehicleForm({ onSuccess }) {
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    resolver: yupResolver(schema)
  });

  const onSubmit = async (data) => {
    try {
      await vehicleService.createVehicle(data);
      onSuccess?.();
    } catch (err) {
      console.error(err);
      // Handle error with Snackbar/Alert
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 2 }}>
      <TextField
        fullWidth
        label="Make"
        {...register('make')}
        error={!!errors.make}
        helperText={errors.make?.message}
        sx={{ mb: 2 }}
      />
      <TextField
        fullWidth
        label="Model"
        {...register('model')}
        error={!!errors.model}
        helperText={errors.model?.message}
        sx={{ mb: 2 }}
      />
      <TextField
        fullWidth
        type="number"
        label="Year"
        {...register('year')}
        error={!!errors.year}
        helperText={errors.year?.message}
        sx={{ mb: 2 }}
      />
      <Button
        type="submit"
        variant="contained"
        disabled={isSubmitting}
        fullWidth
      >
        {isSubmitting ? 'Creating...' : 'Create Vehicle'}
      </Button>
    </Box>
  );
}
```

**4. Add Routes**
```javascript
// src/App.jsx
import VehicleList from './pages/vehicles/VehicleList';
import VehicleForm from './pages/vehicles/VehicleForm';

function App() {
  return (
    <Routes>
      {/* ... other routes ... */}
      <Route path="/vehicles" element={
        <ProtectedRoute>
          <VehicleList />
        </ProtectedRoute>
      } />
      <Route path="/vehicles/new" element={
        <ProtectedRoute>
          <VehicleForm />
        </ProtectedRoute>
      } />
    </Routes>
  );
}
```

## Security Checklist

- [ ] `[Authorize]` attribute on protected endpoints
- [ ] User can only access their own data (check UserId)
- [ ] Input validation with Data Annotations (backend)
- [ ] Form validation with React Hook Form + Yup (frontend)
- [ ] User-friendly error messages (no stack traces)
- [ ] No console.log() with sensitive data

## Configuration Files

**Backend (appsettings.json):**
- Database connection string
- JWT configuration (Secret, Issuer, Audience, ExpirationHours)
- JWT Secret must be 32+ characters

**Frontend (.env):**
- `VITE_API_URL` - Backend API base URL (default: http://localhost:5239/api)

**CORS (Program.cs):**
- Default allowed origins: http://localhost:5173, http://localhost:5175
- Update if frontend runs on different port

## What NOT to Do

- ❌ Don't write tests - QA agent handles this (Phase 4)
- ❌ Don't modify existing migrations - create new ones
- ❌ Don't skip migration creation after model changes
- ❌ Don't expose technical errors to users
- ❌ Don't leave console.log() statements
- ❌ Don't break existing patterns

## Your Success Criteria

✅ **Good Implementation:**
- Follows the plan exactly
- Matches existing code patterns
- Clean, readable code
- Proper error handling
- No security vulnerabilities
- Ready for QA validation

❌ **Poor Implementation:**
- Deviates from plan without explanation
- Invents new patterns
- Messy, hard-to-read code
- Missing error handling
- Security issues

## Remember

- **Follow the plan** - Don't improvise
- **Match existing patterns** - Read similar code first
- **Keep it simple** - Don't over-engineer
- **Handle errors** - User-friendly messages
- **Clean code** - Someone will review this
