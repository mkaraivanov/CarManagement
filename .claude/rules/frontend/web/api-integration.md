---
paths:
  - "web-frontend/src/services/**/*.js"
  - "web-frontend/src/services/**/*.ts"
---

# Frontend API Integration

## API Communication Pattern

- Axios instance in `src/services/api.js` configured with base URL from `.env`
- **Request Interceptor**: Automatically adds JWT token from localStorage to all requests
- **Response Interceptor**: Catches 401 responses, clears token, redirects to login
- All API calls go through service layer (don't call axios directly from components)

## Configuration

**Frontend (`web-frontend/.env`):**
- `VITE_API_URL`: Backend API base URL (default: `http://localhost:5239/api`)

## Frontend API Integration Workflow

**⚠️ STEP 0: Use the AI Sub-Agent Workflow!**
- See @.claude/rules/workflows/ai-sub-agent.md

**Key pattern:**
1. Create service method in `src/services/` (never call axios directly from components)
2. Use shared `api` instance from `src/services/api.js`
3. Handle errors with try-catch in components
4. Show user feedback with Material-UI Snackbar/Alert

## Service Layer Pattern

### Creating Service Methods

```javascript
// src/services/vehicleService.js
import api from './api';

export const vehicleService = {
  async getVehicles() {
    const response = await api.get('/vehicles');
    return response.data; // Return data, not full response
  },

  async createVehicle(vehicleData) {
    const response = await api.post('/vehicles', vehicleData);
    return response.data;
  }
};
```

### Using Services in Components

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

  // Render component...
}
```

## Critical Rules

- Service files return `response.data`, not the full axios response
- JWT token handled automatically by axios interceptors in `api.js`
- Use try-catch blocks for API calls in components
- **Never call axios directly from components** - always use service layer

## Common Mistakes

- ❌ Calling axios directly from components (use service layer)
- ❌ Not handling loading states
- ❌ Not handling errors gracefully
- ❌ Returning full axios response instead of just data
