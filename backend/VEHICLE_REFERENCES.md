# Vehicle Makes and Models - Dropdown Reference Data

## Overview
The application now includes a complete database of car makes and models to power dropdown selections in the frontend. This ensures data consistency and provides a better user experience.

## Database Tables

### CarMakes
- **Id**: int (Primary Key)
- **Name**: string (Make name, e.g., "Toyota", "Honda")

### CarModels
- **Id**: int (Primary Key)
- **MakeId**: int (Foreign Key to CarMakes)
- **Name**: string (Model name, e.g., "Camry", "Civic")

## Seed Data

The database includes 10 popular car brands with 61 total models:

1. **Toyota** (7 models): Camry, Corolla, RAV4, Highlander, Prius, Tacoma, 4Runner
2. **Honda** (6 models): Civic, Accord, CR-V, Pilot, Odyssey, HR-V
3. **Ford** (6 models): F-150, Mustang, Explorer, Escape, Edge, Bronco
4. **Chevrolet** (6 models): Silverado, Equinox, Malibu, Traverse, Tahoe, Camaro
5. **BMW** (6 models): 3 Series, 5 Series, X3, X5, X7, M3
6. **Mercedes-Benz** (6 models): C-Class, E-Class, GLC, GLE, S-Class, GLA
7. **Audi** (6 models): A4, A6, Q5, Q7, A3, Q3
8. **Volkswagen** (6 models): Jetta, Passat, Tiguan, Atlas, Golf, Taos
9. **Nissan** (6 models): Altima, Sentra, Rogue, Pathfinder, Murano, Frontier
10. **Hyundai** (6 models): Elantra, Sonata, Tucson, Santa Fe, Palisade, Kona

## API Endpoints

### 1. Get All Makes with Models
```http
GET /api/vehiclereferences/makes
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Toyota",
    "models": [
      {"id": 1, "makeId": 1, "name": "Camry"},
      {"id": 2, "makeId": 1, "name": "Corolla"},
      ...
    ]
  },
  ...
]
```

**Use case:** Populate cascading dropdowns where selecting a make filters the model dropdown.

---

### 2. Get Make Names Only (Lightweight)
```http
GET /api/vehiclereferences/makes/names
```

**Response:**
```json
["Audi", "BMW", "Chevrolet", "Ford", "Honda", "Hyundai", "Mercedes-Benz", "Nissan", "Toyota", "Volkswagen"]
```

**Use case:** Simple dropdown for make selection only.

---

### 3. Get Models for Specific Make
```http
GET /api/vehiclereferences/makes/{makeId}/models
```

**Example:**
```http
GET /api/vehiclereferences/makes/1/models
```

**Response:**
```json
[
  {"id": 1, "makeId": 1, "name": "Camry"},
  {"id": 2, "makeId": 1, "name": "Corolla"},
  {"id": 3, "makeId": 1, "name": "RAV4"},
  ...
]
```

**Use case:** Load models dynamically after user selects a make.

## Frontend Integration Example

### Using Cascading Dropdowns

```javascript
// 1. Load all makes on component mount
const [makes, setMakes] = useState([]);
const [models, setModels] = useState([]);

useEffect(() => {
  fetch('http://localhost:5239/api/vehiclereferences/makes')
    .then(res => res.json())
    .then(data => setMakes(data));
}, []);

// 2. When user selects a make, load its models
const handleMakeChange = (makeId) => {
  const selectedMake = makes.find(m => m.id === makeId);
  setModels(selectedMake?.models || []);
};

// 3. Render dropdowns
<select onChange={(e) => handleMakeChange(parseInt(e.target.value))}>
  <option value="">Select Make</option>
  {makes.map(make => (
    <option key={make.id} value={make.id}>{make.name}</option>
  ))}
</select>

<select>
  <option value="">Select Model</option>
  {models.map(model => (
    <option key={model.id} value={model.name}>{model.name}</option>
  ))}
</select>
```

### Alternative: Lightweight Approach

```javascript
// Load only make names
fetch('http://localhost:5239/api/vehiclereferences/makes/names')
  .then(res => res.json())
  .then(names => setMakeNames(names));

// Then load models when make is selected
const handleMakeChange = (makeName) => {
  const make = makes.find(m => m.name === makeName);
  if (make) {
    fetch(`http://localhost:5239/api/vehiclereferences/makes/${make.id}/models`)
      .then(res => res.json())
      .then(data => setModels(data));
  }
};
```

## Creating Vehicles with Reference Data

When creating a vehicle, you still use the string values for make and model (not IDs), but they should match the reference data:

```bash
POST /api/vehicles
{
  "make": "Toyota",      # Must match a CarMake.Name
  "model": "Camry",      # Must match a CarModel.Name
  "year": 2023,
  "licensePlate": "ABC123",
  "currentMileage": 15000,
  "color": "Silver"
}
```

## Testing

```bash
# Get all makes with models
curl http://localhost:5239/api/vehiclereferences/makes

# Get just make names
curl http://localhost:5239/api/vehiclereferences/makes/names

# Get Toyota models (makeId=1)
curl http://localhost:5239/api/vehiclereferences/makes/1/models

# Get Honda models (makeId=2)
curl http://localhost:5239/api/vehiclereferences/makes/2/models
```

## Adding More Makes/Models

To add more makes and models, update the seed data in [ApplicationDbContext.cs](Data/ApplicationDbContext.cs) in the `SeedCarMakesAndModels` method, then create and apply a new migration:

```bash
dotnet ef migrations add AddMoreCarModels
dotnet ef database update
```

## Notes

- The Vehicle table still uses strings for Make and Model (not foreign keys) for flexibility
- The reference data is meant to guide users but doesn't enforce strict constraints
- All makes and models are alphabetically sorted in API responses
- No authentication required for reference data endpoints (public access)
