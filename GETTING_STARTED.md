# Getting Started with Car Management Application

## 🎉 What's Been Built

A complete full-stack car management application with:

### ✅ Backend (ASP.NET Core 9.0 + SQLite)
- **Authentication**: JWT-based auth with BCrypt password hashing
- **Vehicle Management**: Full CRUD operations for vehicles
- **Service Records**: Track maintenance and service history
- **Fuel Records**: Track refueling with automatic fuel efficiency calculations
- **Auto-calculations**:
  - Fuel efficiency based on previous refueling
  - Automatic vehicle mileage updates
  - Total cost calculations

### ✅ Frontend (React 19 + Material-UI + Vite)
- **Modern UI**: Professional Material-UI components
- **Authentication**: Login & registration pages with JWT token management
- **Dashboard**: Summary cards showing vehicle count, service records, and fuel records
- **Vehicle Management**:
  - List all vehicles with status indicators
  - Add/Edit vehicle forms with full validation
  - Vehicle details page with tabs for service & fuel records
- **Responsive Design**: Works on desktop, tablet, and mobile
- **Protected Routes**: Automatic redirect to login for unauthenticated users

---

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK installed
- Node.js 18+ and npm installed

### Start the Backend
```bash
cd backend
dotnet run
```
Backend will be available at: **http://localhost:5239**

### Start the Frontend
```bash
cd web-frontend
npm run dev
```
Frontend will be available at: **http://localhost:5173**

---

## 📖 Using the Application

### 1. Register a New Account
1. Open http://localhost:5173 in your browser
2. Click "Sign up" on the login page
3. Fill in username, email, and password (min 6 characters)
4. You'll be automatically logged in and redirected to the dashboard

### 2. Add Your First Vehicle
1. Click "Add Vehicle" button on the dashboard or navigate to "Vehicles"
2. Fill in the vehicle details:
   - **Required**: Make, Model, Year, License Plate, Current Mileage
   - **Optional**: VIN, Purchase Date, Color
3. Click "Add Vehicle"

### 3. View Vehicle Details
1. Go to "Vehicles" page
2. Click the eye icon or vehicle name to view details
3. The details page shows:
   - Full vehicle information
   - Service history tab (empty initially)
   - Fuel records tab (empty initially)

### 4. Edit or Delete Vehicles
- Click the edit icon in the vehicle list or "Edit" button on details page
- Click the delete icon to remove a vehicle (with confirmation)

---

## 🔐 API Authentication

All API requests (except login/register) require a JWT token:

```bash
# Example: Get current user
curl http://localhost:5239/api/users/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

The frontend automatically includes the token in all requests via Axios interceptors.

---

## 📂 Project Structure

### Backend
```
backend/
├── Controllers/        # API endpoints
├── Models/            # Database entities
├── Services/          # Business logic
├── DTOs/              # Data transfer objects
├── Data/              # EF Core DbContext
├── carmanagement.db   # SQLite database (created on first run)
└── API.md             # Complete API documentation
```

### Frontend
```
web-frontend/
├── src/
│   ├── components/    # Reusable components
│   │   ├── auth/      # ProtectedRoute
│   │   └── layout/    # AppLayout, Navbar
│   ├── pages/         # Main pages
│   │   ├── vehicles/  # Vehicle pages
│   │   ├── Login.jsx
│   │   ├── Register.jsx
│   │   └── Dashboard.jsx
│   ├── services/      # API service layer
│   ├── context/       # AuthContext
│   └── utils/         # Utility functions
└── .env               # Environment variables
```

---

## 🎨 Features Walkthrough

### Dashboard
- **Summary Cards**: Shows total vehicles, service records, and fuel records
- **Quick Actions**: Direct access to add vehicles, log services, and add fuel records

### Vehicle Management
- **Table View**: All vehicles displayed in a clean, sortable table
- **Status Indicators**: Color-coded chips (Active, Sold, Inactive)
- **Actions**: View, edit, and delete vehicles directly from the list

### Vehicle Details
- **Comprehensive Info**: All vehicle details in a clean card layout
- **Tabbed Interface**:
  - Service History: Shows all maintenance records
  - Fuel Records: Shows refueling history with fuel efficiency
- **Quick Actions**: Edit or delete vehicle from the details page

---

## 🧪 Testing the Application

### Test the Full Flow
1. **Register** → Create account
2. **Add Vehicle** → Toyota Camry 2022
3. **View Details** → See the vehicle information
4. **Edit Vehicle** → Update mileage
5. **Delete Vehicle** → Remove the vehicle

### Backend API Testing
See [backend/API.md](backend/API.md) for complete API documentation and curl examples.

Example:
```bash
# Register
curl -X POST http://localhost:5239/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"password123"}'

# Login and save token
TOKEN=$(curl -s -X POST http://localhost:5239/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"password123"}' | jq -r '.token')

# Create vehicle
curl -X POST http://localhost:5239/api/vehicles \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"make":"Toyota","model":"Camry","year":2022,"licensePlate":"ABC123","currentMileage":15000}'
```

---

## 🔧 Configuration

### Backend Configuration
Edit `backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=carmanagement.db"
  },
  "Jwt": {
    "Secret": "SuperSecretKey12345SuperSecretKey12345",
    "Issuer": "CarManagementAPI",
    "Audience": "CarManagementClient",
    "ExpirationHours": 1
  }
}
```

### Frontend Configuration
Edit `web-frontend/.env`:
```
VITE_API_URL=http://localhost:5239/api
```

---

## 📦 Dependencies

### Backend
- Microsoft.EntityFrameworkCore (9.0.1)
- Microsoft.EntityFrameworkCore.Sqlite (9.0.1)
- Microsoft.AspNetCore.Authentication.JwtBearer (9.0.0)
- BCrypt.Net-Next (4.0.3)

### Frontend
- React 19
- Material-UI (@mui/material)
- React Router v7
- Axios
- React Hook Form
- Yup
- date-fns
- Recharts

---

## 🎯 Next Steps (Future Enhancements)

### Service Records UI (Phase 2)
- Add service record form
- Service history timeline view
- Service due reminders

### Fuel Records UI (Phase 2)
- Add fuel record form
- Fuel efficiency charts with Recharts
- Monthly fuel expense summaries

### Additional Modules (Phase 3+)
- Insurance management
- Registration & documentation
- Expenses & analytics dashboard
- Reminders & notifications

---

## 🐛 Troubleshooting

### Port Already in Use
If backend fails to start:
```bash
# Kill process on port 5239
lsof -ti:5239 | xargs kill -9
```

If frontend fails to start:
```bash
# Kill process on port 5173
lsof -ti:5173 | xargs kill -9
```

### Database Issues
Delete the database to start fresh:
```bash
rm backend/carmanagement.db
cd backend && dotnet ef database update
```

### Authentication Issues
- Clear browser localStorage to remove old tokens
- Check that JWT secret in appsettings.json is at least 32 characters

---

## 📚 Additional Documentation

- [backend/API.md](backend/API.md) - Complete REST API documentation
- [backend/README.md](backend/README.md) - Backend specific documentation
- [MEMORY.md](.claude/projects/-Users-martin-karaivanov-Projects-CarManagement/memory/MEMORY.md) - Project memory and development notes

---

## ✨ What's Working

✅ Full authentication system (register, login, logout)
✅ Protected routes with automatic redirect
✅ JWT token management with auto-refresh
✅ Vehicle CRUD operations (Create, Read, Update, Delete)
✅ Material-UI professional interface
✅ Responsive design
✅ Backend API with 15+ endpoints
✅ Database with proper relationships
✅ Auto-calculations for fuel efficiency
✅ Error handling and validation

---

**Ready to use!** Open http://localhost:5173 and start managing your vehicles! 🚗
