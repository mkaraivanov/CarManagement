# Car Management Application

A full-stack application for managing vehicles, service records, and fuel consumption tracking.

**📖 [Getting Started Guide](GETTING_STARTED.md)** - Complete walkthrough and setup instructions

## 🎯 Features

- ✅ **User Authentication** - Register, login, JWT tokens with BCrypt hashing
- ✅ **Vehicle Management** - Add, edit, delete, and view multiple vehicles
- ✅ **Dashboard** - Overview with summary cards and quick actions
- ✅ **Service History API** - Backend endpoints for maintenance tracking
- ✅ **Fuel Tracking API** - Backend endpoints with auto fuel efficiency calculations
- ✅ **Material-UI Interface** - Professional, responsive design
- ✅ **Protected Routes** - Automatic login redirect for unauthenticated users
- 🚧 **Service Records UI** - Coming in Phase 5
- 🚧 **Fuel Records UI** - Coming in Phase 6

## 🛠 Tech Stack

**Backend:**
- ASP.NET Core 9.0 Web API
- Entity Framework Core 9.0
- SQLite Database
- JWT Authentication + BCrypt
- RESTful API design

**Frontend:**
- React 19 + Vite
- Material-UI (MUI)
- React Router v7
- Axios

**Mobile:**
- React Native (in progress)

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Node.js 20+

### Backend Setup

```bash
cd backend
dotnet build
dotnet run
```
Backend runs on `http://localhost:5239`

### Frontend Setup

```bash
cd web-frontend
npm install
npm run dev
```
Frontend runs on `http://localhost:5173`

## 📁 Project Structure

```
├── backend/              # ASP.NET Core API
│   ├── Controllers/      # API endpoints
│   ├── Models/          # Database entities
│   ├── Services/        # Business logic
│   ├── DTOs/            # Data transfer objects
│   └── Data/            # EF Core DbContext
├── web-frontend/        # React web app
├── mobile-frontend/     # React Native app
└── .claude/            # Project memory & plans
```

## 📚 API Documentation

See [backend/API.md](backend/API.md) for detailed API documentation with examples.

**Available Endpoints:**

**Authentication:**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login user
- `GET /api/users/me` - Get current user

**Vehicles:**
- `GET /api/vehicles` - List user's vehicles
- `GET /api/vehicles/{id}` - Get vehicle details
- `POST /api/vehicles` - Create vehicle
- `PUT /api/vehicles/{id}` - Update vehicle
- `DELETE /api/vehicles/{id}` - Delete vehicle
- `PATCH /api/vehicles/{id}/mileage` - Update mileage

**Service Records:**
- `GET /api/vehicles/{vehicleId}/services` - List service records
- `GET /api/services/{id}` - Get service details
- `POST /api/vehicles/{vehicleId}/services` - Create service record
- `PUT /api/services/{id}` - Update service record
- `DELETE /api/services/{id}` - Delete service record

**Fuel Records:**
- `GET /api/vehicles/{vehicleId}/fuel-records` - List fuel records
- `GET /api/fuel-records/{id}` - Get fuel record details
- `POST /api/vehicles/{vehicleId}/fuel-records` - Create fuel record
- `PUT /api/fuel-records/{id}` - Update fuel record
- `DELETE /api/fuel-records/{id}` - Delete fuel record
- `GET /api/vehicles/{vehicleId}/fuel-efficiency` - Get efficiency stats

## 🗄 Database

SQLite database: `backend/carmanagement.db`

**Tables:**
- Users (authentication)
- Vehicles (vehicle info)
- ServiceRecords (maintenance history)
- FuelRecords (refueling logs)

## ✅ Development Status

- ✅ **Phase 1**: Backend foundation & authentication - **COMPLETE**
- ✅ **Phase 2**: Vehicle API - **COMPLETE**
- ✅ **Phase 3**: Service & Fuel APIs - **COMPLETE**
- ✅ **Phase 4**: Frontend UI (Vehicle Management) - **COMPLETE**
- ⏳ **Phase 5**: Service Records UI
- ⏳ **Phase 6**: Fuel Records UI

## 🔐 Configuration

Edit `backend/appsettings.json`:
- Database: `Data Source=carmanagement.db`
- JWT settings (⚠️ change secret for production!)

## 📝 License

MIT