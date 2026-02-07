# Car Management API Documentation

Complete REST API for managing vehicles, service records, and fuel consumption.

## Base URL
`http://localhost:5239`

## Authentication

All protected endpoints require a JWT token in the Authorization header:
```
Authorization: Bearer <token>
```

---

## 🔐 Authentication Endpoints

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "string",
  "email": "string",
  "password": "string" (min 6 characters)
}
```

**Response 200:**
```json
{
  "token": "jwt_token",
  "user": {
    "id": "guid",
    "username": "string",
    "email": "string",
    "createdAt": "datetime"
  }
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "string",
  "password": "string"
}
```

**Response 200:** Same as register

### Get Current User
```http
GET /api/users/me
Authorization: Bearer <token>
```

---

## 🚗 Vehicle Endpoints

### List User's Vehicles
```http
GET /api/vehicles
Authorization: Bearer <token>
```

**Response 200:** Array of vehicles

### Get Vehicle by ID
```http
GET /api/vehicles/{id}
Authorization: Bearer <token>
```

### Create Vehicle
```http
POST /api/vehicles
Authorization: Bearer <token>
Content-Type: application/json

{
  "make": "string",
  "model": "string",
  "year": number (1900-2100),
  "licensePlate": "string",
  "currentMileage": number,
  "vin": "string (optional)",
  "purchaseDate": "datetime (optional)",
  "color": "string (optional)",
  "status": "Active" | "Sold" | "Inactive"
}
```

### Update Vehicle
```http
PUT /api/vehicles/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  // All fields optional - only send what you want to update
  "make": "string",
  "model": "string",
  "currentMileage": number,
  ...
}
```

### Delete Vehicle
```http
DELETE /api/vehicles/{id}
Authorization: Bearer <token>
```

**Response 204:** No Content

### Update Vehicle Mileage
```http
PATCH /api/vehicles/{id}/mileage
Authorization: Bearer <token>
Content-Type: application/json

{
  "mileage": number
}
```

---

## 🔧 Service Record Endpoints

### List Vehicle's Service Records
```http
GET /api/vehicles/{vehicleId}/services
Authorization: Bearer <token>
```

**Response 200:** Array of service records (ordered by date DESC)

### Get Service Record by ID
```http
GET /api/services/{id}
Authorization: Bearer <token>
```

### Create Service Record
```http
POST /api/vehicles/{vehicleId}/services
Authorization: Bearer <token>
Content-Type: application/json

{
  "serviceDate": "datetime",
  "mileageAtService": number,
  "serviceType": "OilChange" | "TireRotation" | "BrakeService" | "Inspection" | "General" | "Other",
  "serviceCenter": "string",
  "description": "string",
  "cost": number,
  "nextServiceDue": "datetime (optional)",
  "nextServiceMileage": number (optional),
  "receiptUrl": "string (optional)"
}
```

**✨ Auto-Updates:** Vehicle mileage is automatically updated if service mileage > current mileage

### Update Service Record
```http
PUT /api/services/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  // All fields optional
  "serviceDate": "datetime",
  "cost": number,
  ...
}
```

### Delete Service Record
```http
DELETE /api/services/{id}
Authorization: Bearer <token>
```

---

## ⛽ Fuel Record Endpoints

### List Vehicle's Fuel Records
```http
GET /api/vehicles/{vehicleId}/fuel-records
Authorization: Bearer <token>
```

**Response 200:** Array of fuel records (ordered by date DESC)

### Get Fuel Record by ID
```http
GET /api/fuel-records/{id}
Authorization: Bearer <token>
```

### Create Fuel Record
```http
POST /api/vehicles/{vehicleId}/fuel-records
Authorization: Bearer <token>
Content-Type: application/json

{
  "refuelDate": "datetime",
  "mileage": number,
  "quantity": number (liters/gallons),
  "pricePerUnit": number,
  "fuelType": "Regular" | "Premium" | "Diesel" | "Electric",
  "gasStation": "string (optional)",
  "notes": "string (optional)"
}
```

**✨ Auto-Calculations:**
- `totalCost` = quantity × pricePerUnit
- `fuelEfficiency` = (mileage - previous mileage) / quantity (MPG or km/L)
- Vehicle mileage is automatically updated

### Update Fuel Record
```http
PUT /api/fuel-records/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  // All fields optional
  "mileage": number,
  "quantity": number,
  ...
}
```

### Delete Fuel Record
```http
DELETE /api/fuel-records/{id}
Authorization: Bearer <token>
```

### Get Fuel Efficiency Stats
```http
GET /api/vehicles/{vehicleId}/fuel-efficiency
Authorization: Bearer <token>
```

**Response 200:**
```json
{
  "averageEfficiency": number,
  "totalCost": number,
  "totalQuantity": number,
  "recordCount": number
}
```
*Calculates average over last 10 fuel records*

---

## 🧪 Testing Examples

### Complete User Flow
```bash
# 1. Register
curl -X POST http://localhost:5239/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"john","email":"john@example.com","password":"pass123"}'

# 2. Login (save token)
TOKEN=$(curl -s -X POST http://localhost:5239/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john","password":"pass123"}' | jq -r '.token')

# 3. Create Vehicle
curl -X POST http://localhost:5239/api/vehicles \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"make":"Toyota","model":"Camry","year":2022,"licensePlate":"ABC123","currentMileage":15000}'

# 4. Add Service Record
curl -X POST http://localhost:5239/api/vehicles/{vehicleId}/services \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"serviceDate":"2026-02-01","mileageAtService":15500,"serviceType":"OilChange","serviceCenter":"QuickLube","description":"Regular oil change","cost":45.99}'

# 5. Add Fuel Record
curl -X POST http://localhost:5239/api/vehicles/{vehicleId}/fuel-records \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"refuelDate":"2026-02-05","mileage":15750,"quantity":12.5,"pricePerUnit":3.89,"fuelType":"Regular"}'

# 6. Get Fuel Efficiency
curl http://localhost:5239/api/vehicles/{vehicleId}/fuel-efficiency \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📄 Vehicle Registration Endpoints

⚠️ **WARNING: These endpoints are backend-only. No frontend UI exists yet. Feature is incomplete.**

### Extract Registration Data from Document
```http
POST /api/vehicle-registration/extract
Authorization: Bearer <token>
Content-Type: multipart/form-data

file: <registration_document.jpg|png|pdf>
```

Extracts vehicle data from registration document using OCR.

**Supported formats:** JPG, PNG, PDF (max 10MB)

**Response 200:**
```json
{
  "success": true,
  "message": "Registration data extracted successfully",
  "extractedData": {
    "registrationNumber": {
      "value": "ABC123",
      "confidence": "High"
    },
    "make": {
      "value": "Toyota",
      "confidence": "High"
    },
    "model": {
      "value": "Camry",
      "confidence": "Medium"
    },
    "year": {
      "value": "2022",
      "confidence": "High"
    },
    "vin": {
      "value": "1HGBH41JXMN109186",
      "confidence": "High"
    },
    "ownerName": {
      "value": "John Doe",
      "confidence": "Medium"
    },
    "expiryDate": {
      "value": "2026-12-31",
      "confidence": "Low"
    }
  },
  "rawText": "full extracted OCR text..."
}
```

**Response 400 (validation errors):**
```json
{
  "success": false,
  "message": "No file provided",
  "errors": ["Please upload a registration document image or PDF"]
}
```

### Upload Registration Document for Vehicle
```http
POST /api/vehicle-registration/upload/{vehicleId}
Authorization: Bearer <token>
Content-Type: multipart/form-data

file: <registration_document.jpg|png|pdf>
```

Uploads and stores a registration document for an existing vehicle.

**Response 200:**
```json
{
  "success": true,
  "message": "Registration document uploaded successfully",
  "documentUrl": "uploads/user-id/registrations/filename.jpg",
  "publicUrl": "/uploads/user-id/registrations/filename.jpg"
}
```

**cURL Example:**
```bash
# Extract registration data
curl -X POST http://localhost:5239/api/vehicle-registration/extract \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@registration.jpg"

# Upload registration document
curl -X POST http://localhost:5239/api/vehicle-registration/upload/{vehicleId} \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@registration.pdf"
```

---

## 📋 Status Codes

- `200 OK` - Success
- `201 Created` - Resource created
- `204 No Content` - Success (no body)
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Missing/invalid token
- `404 Not Found` - Resource not found

---

## 🔒 Security

- Passwords hashed with BCrypt
- JWT tokens expire in 1 hour
- Users can only access their own data
- All endpoints except auth require authentication
