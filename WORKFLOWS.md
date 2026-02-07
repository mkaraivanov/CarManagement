# Development Workflows

This document outlines development workflows, best practices, and patterns for working on the CarManagement project.

## Table of Contents
- [Feature Design Documentation](#feature-design-documentation)
- [Feature Completion Definition](#feature-completion-definition)
- [Git Workflow](#git-workflow)
- [Backend Development Workflow](#backend-development-workflow)
- [Frontend Development Workflow](#frontend-development-workflow)
- [Before Committing - AI Checklist](#before-committing---ai-checklist)
- [Database & Migration Best Practices](#database--migration-best-practices)

## Feature Design Documentation

**REQUIRED: All new major features MUST have a design document in `docs/features/` BEFORE implementation starts.**

**What requires a design doc:**
- Features spanning multiple files (3+ files)
- Features requiring database schema changes
- Features involving new technologies/libraries
- Features with multiple implementation phases
- Any feature estimated at 2+ days of work

**Process:**
1. **Before coding:** Copy `docs/features/_TEMPLATE.md` to `docs/features/feature-name.md`
2. **Fill in all sections:** Context, technology decisions, API design, implementation phases, etc.
3. **Commit the design doc** before starting implementation
4. **Reference the doc** in commit messages during implementation
5. **Update status** as phases complete: 🔵 Planning → 🟡 In Progress → ⚠️ Backend Only → ✅ Complete
6. **Keep it current** - update the doc when requirements change

**Benefits:**
- Clearer thinking before coding
- Better time estimates
- Documentation for future reference
- AI assistants can follow the plan accurately
- Easier to pick up work after breaks

**See [`docs/README.md`](docs/README.md) for complete guidelines and templates.**

**Before Starting New Features:**
```bash
# Create feature design document
cp docs/features/_TEMPLATE.md docs/features/my-feature-name.md
# Fill in all sections before coding
# Commit the design doc first
git add docs/features/my-feature-name.md
git commit -m "Add design doc for [feature name]"
```

## Feature Completion Definition

**CRITICAL RULE: A feature is NOT considered complete/implemented until it has BOTH backend AND frontend components.**

- Backend-only APIs without UI are considered **incomplete features**
- Frontend-only UIs without backend support are considered **incomplete features**
- Only features with **working end-to-end integration** (backend + frontend + testing) are considered complete

**Why this matters:**
- Prevents accumulation of unused backend code
- Ensures features are actually usable by end users
- Maintains consistency between API and UI development
- Helps AI assistants understand what's truly "done" vs "in progress"

**When documenting features:**
- Incomplete features must be listed in `INCOMPLETE_FEATURES.md`
- Only remove from incomplete list when both backend and frontend are implemented and tested
- Update API.md when backend is done, but mark endpoints as "⚠️ No Frontend Yet"

## Git Workflow

**Branching Strategy:**
- Create feature branches for significant changes: `feature/add-fuel-ui`, `fix/auth-bug`, `refactor/service-layer`
- Keeps `main` branch clean and makes it easier to abandon unsuccessful experiments
- Can push to `main` directly for small fixes; use branches for larger features
- Delete branches after merging or abandoning

**Commit Practices:**
- Commit when code works and is tested (not broken code)
- Clear commit messages describing what and why
- Test end-to-end before pushing

## Backend Development Workflow

### When Adding/Modifying Database Entities (CRITICAL WORKFLOW)

This is a common area where AI makes mistakes. Follow these steps in order:

1. Create or update Model in `backend/Models/` with EF Core annotations
2. Update `ApplicationDbContext.cs`:
   - Add `DbSet<EntityName>` property if new entity
   - Configure relationships in `OnModelCreating()` if needed
3. **Create migration**: `dotnet ef migrations add DescriptiveName`
4. **Apply migration**: `dotnet ef database update`
5. Create DTOs in `backend/DTOs/` (request and response DTOs)
6. Create Service interface in `backend/Services/IEntityService.cs`
7. Implement Service in `backend/Services/EntityService.cs`
8. **Register service in `Program.cs`** (AI commonly forgets this step!)
   ```csharp
   builder.Services.AddScoped<IEntityService, EntityService>();
   ```
9. Create Controller in `backend/Controllers/`
10. Test with curl or frontend
11. Update `backend/API.md` with new endpoints

### When Adding New API Endpoints

**FIRST: Ensure there's a feature design doc in `docs/features/` for this endpoint!**

Then follow these steps:

1. Add method to appropriate service interface + implementation
2. Add controller action with proper HTTP verb attribute (`[HttpGet]`, `[HttpPost]`, etc.)
3. Add `[Authorize]` attribute unless endpoint should be public
4. Use DTOs for request/response (never expose Models directly)
5. Return appropriate status codes (200, 201, 400, 404, 500)
6. Update `backend/API.md` with endpoint documentation
7. If no frontend exists yet, mark endpoint as "⚠️ No Frontend Yet" in API.md
8. Update feature design doc with implementation status

### Critical Backend Rules

- **Business logic belongs in Services**, not Controllers (Controllers should be thin routing layers)
- **Never expose Models directly** - always use DTOs for API requests/responses
- **Always create migrations** after model changes (don't skip this!)
- **Always register new services** in `Program.cs` DI container
- **Add `[Authorize]` attribute** to all endpoints except login/register
- **Return proper HTTP status codes**: 200 (OK), 201 (Created), 400 (Bad Request), 404 (Not Found), 500 (Server Error)
- **Validate inputs** using Data Annotations in DTOs

### Common AI Mistakes on Backend

- ❌ Forgetting to create migration after model changes
- ❌ Not registering new services in `Program.cs` DI container
- ❌ Putting business logic in Controllers instead of Services
- ❌ Exposing Models directly instead of using DTOs
- ❌ Modifying existing migrations (always create new ones)
- ❌ Not adding `[Authorize]` to protected endpoints

## Frontend Development Workflow

### When Adding Features

1. API calls must go through service layer (`src/services/`), **never call axios directly in components**
2. Use `AuthContext` for authentication state
3. Forms must use React Hook Form + Yup for validation
4. Show loading states for async operations
5. Display user-friendly error messages using Material-UI Snackbar/Alert

### Frontend API Integration

1. Create service method in appropriate service file (e.g., `vehicleService.js`)
2. Use the shared `api` instance from `src/services/api.js`
3. Handle errors with try-catch in components
4. Show user feedback with Material-UI Snackbar/Alert components

### Frontend Patterns to Follow

- Protected pages wrapped in `<ProtectedRoute>`
- Service files return `response.data`, not the full axios response
- JWT token handled automatically by axios interceptors in `api.js`
- Use try-catch blocks for API calls in components
- Clear error messages for users (not raw error objects)

### Common AI Mistakes on Frontend

- ❌ Calling axios directly from components (use service layer)
- ❌ Not showing loading states during async operations
- ❌ Exposing technical errors to users
- ❌ Not using React Hook Form for complex forms
- ❌ Breaking the service layer pattern

## Before Committing - AI Checklist

Run through this checklist before every commit:

### Must Verify
- [ ] If database models changed: migration created AND applied
- [ ] If new service created: registered in `Program.cs` DI container
- [ ] Backend compiles: `cd backend && dotnet build`
- [ ] Frontend lints: `cd web-frontend && npm run lint`
- [ ] Tested end-to-end (both UI and API working together)
- [ ] `backend/API.md` updated if endpoints changed
- [ ] No console errors in browser developer tools
- [ ] No exceptions in backend console output

### Common Checks
- Backend running on correct port (5239)
- Frontend running on correct port (5173)
- CORS configured for frontend URL
- JWT token being sent in requests (check Network tab)
- Database file exists and has tables

### Testing
```bash
# Always run tests before pushing
cd backend/Backend.Tests && dotnet test
# All tests must pass
```

## Database & Migration Best Practices

### Migration Workflow

- **Never modify existing migrations** that have been committed - create new ones instead
- Test migrations locally before committing
- Backup database before major schema changes: `cp backend/carmanagement.db backend/carmanagement.db.backup`
- If migration fails, rollback and fix:
  ```bash
  dotnet ef database update PreviousMigrationName
  dotnet ef migrations remove
  # Fix the issue, then create new migration
  dotnet ef migrations add FixedMigrationName
  dotnet ef database update
  ```

### Database Maintenance

- SQLite database file: `backend/carmanagement.db`
- Connection string in `backend/appsettings.json`
- To reset database completely: delete file and run `dotnet ef database update`

### Database Seeding

- Car makes and models are seeded automatically in `ApplicationDbContext.cs`
- Seed data includes 10 makes and 60+ models
- To modify seed data, edit `SeedCarMakesAndModels()` method and create migration

## Code Quality & Consistency

### Follow Existing Patterns

- Match the architectural patterns already in the codebase
- Controllers → Services → Data/Models on backend
- Components → Services → API on frontend
- Keep DTOs in sync between requests and responses
- Use existing naming conventions (PascalCase for C#, camelCase for JavaScript)

### Code Organization

- Extract reusable logic into utility functions or custom hooks
- Keep components focused and small (single responsibility)
- Remove unused imports and dead code
- Group related files together

### API Documentation

- Update `backend/API.md` whenever adding or changing endpoints
- Include request/response examples
- Document required vs optional fields
- Note authorization requirements
