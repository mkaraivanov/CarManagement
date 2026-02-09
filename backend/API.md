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

**Response 200:**
```json
{
  "id": "guid",
  "username": "string",
  "email": "string",
  "createdAt": "datetime"
}
```

---

## 👥 User Management Endpoints

### Get All Users
```http
GET /api/users
Authorization: Bearer <token>
```

**Response 200:** Array of users with statistics
```json
[
  {
    "id": "guid",
    "username": "string",
    "email": "string",
    "createdAt": "datetime",
    "statistics": {
      "vehicleCount": 0,
      "serviceRecordCount": 0,
      "fuelRecordCount": 0
    }
  }
]
```

### Get User by ID
```http
GET /api/users/{id}
Authorization: Bearer <token>
```

**Response 200:**
```json
{
  "id": "guid",
  "username": "string",
  "email": "string",
  "createdAt": "datetime",
  "statistics": {
    "vehicleCount": 0,
    "serviceRecordCount": 0,
    "fuelRecordCount": 0
  }
}
```

**Response 404:** User not found

### Update User
```http
PUT /api/users/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "username": "string",  // optional
  "email": "string"      // optional
}
```

**Response 200:**
```json
{
  "id": "guid",
  "username": "string",
  "email": "string",
  "createdAt": "datetime"
}
```

**Response 400:** Username or email already in use
**Response 404:** User not found

**Note:** Password cannot be changed through this endpoint.

### Delete User
```http
DELETE /api/users/{id}
Authorization: Bearer <token>
```

**Response 204:** User deleted successfully
**Response 400:** Cannot delete your own account
**Response 404:** User not found

**Note:** Deleting a user will cascade delete all their vehicles, service records, and fuel records.

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

## 🔔 Preventive Maintenance Endpoints

### Maintenance Templates

#### Get All Templates
```http
GET /api/maintenance-templates
Authorization: Bearer <token>
```
Returns all system templates and user's custom templates.

**Response 200:** Array of templates

#### Get System Templates Only
```http
GET /api/maintenance-templates/system
Authorization: Bearer <token>
```
Returns only pre-defined system templates (20 templates across 6 categories).

#### Get Templates by Category
```http
GET /api/maintenance-templates/category/{category}
Authorization: Bearer <token>
```
**Categories:** Engine, Tires, Brakes, Fluids, Inspection, Equipment

#### Get Template Categories
```http
GET /api/maintenance-templates/categories
Authorization: Bearer <token>
```
Returns list of all template categories.

#### Get Template by ID
```http
GET /api/maintenance-templates/{id}
Authorization: Bearer <token>
```

#### Create Custom Template
```http
POST /api/maintenance-templates
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "string",
  "description": "string (optional)",
  "category": "string",
  "defaultIntervalMonths": number (optional),
  "defaultIntervalKilometers": number (optional),
  "defaultIntervalHours": number (optional),
  "useCompoundRule": boolean
}
```
**useCompoundRule:** `true` = OR logic (whichever first), `false` = AND logic (all conditions)

#### Update Custom Template
```http
PUT /api/maintenance-templates/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  // All fields optional
  "name": "string",
  "defaultIntervalMonths": number,
  ...
}
```
**Note:** Only user-created templates can be updated (not system templates)

#### Delete Custom Template
```http
DELETE /api/maintenance-templates/{id}
Authorization: Bearer <token>
```
**Note:** Only user-created templates can be deleted (not system templates)

---

### Maintenance Schedules

#### Get Schedules for Vehicle
```http
GET /api/maintenance-schedules/vehicle/{vehicleId}
Authorization: Bearer <token>
```
Returns all maintenance schedules for a vehicle with calculated status.

**Response 200:**
```json
[
  {
    "id": "guid",
    "vehicleId": "guid",
    "templateId": "guid",
    "taskName": "Oil Change",
    "category": "Engine",
    "intervalMonths": 6,
    "intervalKilometers": 10000,
    "useCompoundRule": true,
    "lastCompletedDate": "2026-01-15",
    "lastCompletedMileage": 50000,
    "nextDueDate": "2026-07-15",
    "nextDueMileage": 60000,
    "daysUntilDue": 158,
    "kilometersUntilDue": 10000,
    "isOverdue": false,
    "isUpcoming": false,
    "status": "OK",
    "isActive": true
  }
]
```

#### Get Schedule by ID
```http
GET /api/maintenance-schedules/{id}
Authorization: Bearer <token>
```

#### Get Overdue Schedules
```http
GET /api/maintenance-schedules/overdue
Authorization: Bearer <token>
```
Returns all overdue schedules across all user's vehicles.

#### Get Upcoming Schedules
```http
GET /api/maintenance-schedules/upcoming
Authorization: Bearer <token>
```
Returns schedules within reminder threshold (default: 30 days or 1,000 km).

#### Create Schedule
```http
POST /api/maintenance-schedules
Authorization: Bearer <token>
Content-Type: application/json

{
  "vehicleId": "guid",
  "templateId": "guid (optional)",
  "taskName": "string",
  "description": "string (optional)",
  "category": "string (optional)",
  "intervalMonths": number (optional),
  "intervalKilometers": number (optional),
  "intervalHours": number (optional),
  "useCompoundRule": boolean,
  "lastCompletedDate": "datetime (optional)",
  "lastCompletedMileage": number (optional)",
  "lastCompletedHours": number (optional)",
  "reminderDaysBefore": 30,
  "reminderKilometersBefore": 1000,
  "reminderHoursBefore": 10
}
```
**✨ Auto-Calculations:** Next due dates calculated automatically based on intervals

#### Update Schedule
```http
PUT /api/maintenance-schedules/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  // All fields optional
  "taskName": "string",
  "intervalMonths": number,
  "isActive": boolean,
  ...
}
```

#### Delete Schedule
```http
DELETE /api/maintenance-schedules/{id}
Authorization: Bearer <token>
```

#### Complete Schedule
```http
POST /api/maintenance-schedules/{id}/complete
Authorization: Bearer <token>
Content-Type: application/json

{
  "completedDate": "datetime",
  "completedMileage": number,
  "completedHours": number (optional),
  "serviceRecordId": "guid (optional)"
}
```
**✨ Auto-Updates:** Recalculates next due dates based on completion data

#### Link Service Record to Schedule
```http
POST /api/maintenance-schedules/{id}/link-service/{serviceRecordId}
Authorization: Bearer <token>
```
Links an existing service record and marks the schedule as completed.

#### Recalculate Next Due Dates
```http
POST /api/maintenance-schedules/{id}/recalculate
Authorization: Bearer <token>
```
Manually recalculate next due dates for a schedule.

#### Recalculate for All Vehicle Schedules
```http
POST /api/maintenance-schedules/vehicle/{vehicleId}/recalculate
Authorization: Bearer <token>
```
Recalculate next due dates for all schedules of a vehicle.

---

### Notifications

#### Get All Notifications
```http
GET /api/notifications?limit=50
Authorization: Bearer <token>
```
Returns user's notifications (default: 50 most recent).

#### Get Unread Notifications
```http
GET /api/notifications/unread
Authorization: Bearer <token>
```

#### Get Notification Count
```http
GET /api/notifications/count
Authorization: Bearer <token>
```

**Response 200:**
```json
{
  "totalCount": 15,
  "unreadCount": 3
}
```

#### Get Notification by ID
```http
GET /api/notifications/{id}
Authorization: Bearer <token>
```

#### Mark Notification as Read
```http
POST /api/notifications/{id}/read
Authorization: Bearer <token>
```

#### Mark All Notifications as Read
```http
POST /api/notifications/read-all
Authorization: Bearer <token>
```

#### Delete Notification
```http
DELETE /api/notifications/{id}
Authorization: Bearer <token>
```

---

### Maintenance Status Codes

- **"OK"** - Maintenance not due yet
- **"Due Soon"** - Within reminder threshold
- **"Overdue"** - Past due date/mileage/hours

### Background Job

A background service runs **daily at midnight UTC** to:
- Check all active schedules
- Create reminders for overdue/upcoming maintenance
- Generate notifications (in-app, email, push)
- Clean up old reminders (90 days) and notifications (30 days)

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

### Preventive Maintenance Flow
```bash
# 1. Get system templates
curl http://localhost:5239/api/maintenance-templates/system \
  -H "Authorization: Bearer $TOKEN"

# 2. Get template categories
curl http://localhost:5239/api/maintenance-templates/categories \
  -H "Authorization: Bearer $TOKEN"

# 3. Create maintenance schedule from template
curl -X POST http://localhost:5239/api/maintenance-schedules \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "{vehicleId}",
    "templateId": "00000000-0000-0000-0000-000000000001",
    "taskName": "Oil Change",
    "intervalMonths": 6,
    "intervalKilometers": 10000,
    "useCompoundRule": true,
    "lastCompletedDate": "2026-01-15",
    "lastCompletedMileage": 50000,
    "reminderDaysBefore": 30,
    "reminderKilometersBefore": 1000
  }'

# 4. Get vehicle's maintenance schedules
curl http://localhost:5239/api/maintenance-schedules/vehicle/{vehicleId} \
  -H "Authorization: Bearer $TOKEN"

# 5. Get overdue schedules
curl http://localhost:5239/api/maintenance-schedules/overdue \
  -H "Authorization: Bearer $TOKEN"

# 6. Get upcoming schedules
curl http://localhost:5239/api/maintenance-schedules/upcoming \
  -H "Authorization: Bearer $TOKEN"

# 7. Complete a maintenance schedule
curl -X POST http://localhost:5239/api/maintenance-schedules/{scheduleId}/complete \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "completedDate": "2026-02-07",
    "completedMileage": 55000,
    "serviceRecordId": "{serviceRecordId}"
  }'

# 8. Get notifications
curl http://localhost:5239/api/notifications \
  -H "Authorization: Bearer $TOKEN"

# 9. Get unread notification count
curl http://localhost:5239/api/notifications/count \
  -H "Authorization: Bearer $TOKEN"

# 10. Mark notification as read
curl -X POST http://localhost:5239/api/notifications/{notificationId}/read \
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
