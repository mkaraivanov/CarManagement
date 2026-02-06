
## Execution Guidelines
## CarManagement AI Agent Instructions

### Project Architecture
- **Backend**: ASP.NET Core 9.0 (C#), SQLite, JWT auth, organized by Controllers (API endpoints), Models (entities), Services (business logic), DTOs (data transfer objects), Data (DbContext).
- **Frontend (Web)**: React 19, Vite, Material-UI, with src/components, src/pages, src/services (API layer), src/context (AuthContext), and .env for API URL config.
- **Frontend (Mobile)**: React Native, Metro, Android/iOS support, see CarManagementMobile/README.md for build/run steps.

### Developer Workflows
- **Backend**: Run with `dotnet run` in backend/. Use VS Code task "Run ASP.NET Core backend" for background execution. API docs: backend/API.md. Config: backend/appsettings.json.
- **Web Frontend**: Run with `npm run dev` in web-frontend/. API URL set in web-frontend/.env. Build with `npm run build`. Lint with `npm run lint`.
- **Mobile Frontend**: Start Metro with `npm start`, then run platform-specific commands (`npm run android`/`npm run ios`). For iOS, use `bundle install` and `bundle exec pod install` for CocoaPods.

### Key Conventions & Patterns
- **Authentication**: JWT required for all API calls except login/register. Token managed in frontend via Axios interceptors.
- **Data Flow**: Backend exposes REST endpoints for vehicles, service records, fuel records. Frontends consume via API service layer.
- **Auto-calculations**: Backend auto-updates vehicle mileage and fuel efficiency on relevant record creation.
- **Protected Routes**: Web frontend uses AuthContext and ProtectedRoute for access control.
- **Status Indicators**: Vehicle status (Active, Sold, Inactive) shown as color-coded chips in UI.

### Integration Points
- **API Base URL**: http://localhost:5239 (configurable in .env)
- **Database**: SQLite file (carmanagement.db) auto-created on backend first run.
- **Environment Configs**: backend/appsettings.json, web-frontend/.env

### Examples
- **Register/Login**: See backend/API.md and GETTING_STARTED.md for curl and UI flows.
- **Add Vehicle**: POST /api/vehicles with required fields; see API.md for schema.
- **Testing**: Use curl examples in GETTING_STARTED.md and API.md for end-to-end flow.

### References
- [GETTING_STARTED.md](../GETTING_STARTED.md): Full-stack overview, quickstart, workflows
- [backend/API.md](../backend/API.md): API reference and examples
- [backend/appsettings.json](../backend/appsettings.json): Backend config
- [web-frontend/.env](../web-frontend/.env): Frontend API config
- [CarManagementMobile/README.md](../mobile-frontend/CarManagementMobile/README.md): Mobile build/run steps
