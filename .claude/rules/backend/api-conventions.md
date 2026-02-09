---
paths:
  - "backend/Controllers/**/*.cs"
  - "backend/DTOs/**/*.cs"
---

# Backend API Conventions

## When Adding New Entities

**⚠️ STEP 0: Use the AI Sub-Agent Workflow!**
- See @.claude/rules/workflows/ai-sub-agent.md

**THEN: Create a feature design doc in `docs/features/` if this is a significant change!**

**Quick checklist:**
1. Create Model in `backend/Models/`
2. Add DbSet to `ApplicationDbContext.cs`
3. Configure relationships in `OnModelCreating()`
4. Create migration: `dotnet ef migrations add AddEntityName`
5. Apply migration: `dotnet ef database update`
6. Create DTOs in `backend/DTOs/`
7. Create Service interface and implementation
8. **Register Service in `Program.cs`** (commonly forgotten!)
9. Create Controller in `backend/Controllers/`
10. Update `backend/API.md`

## When Adding New API Endpoints

**⚠️ STEP 0: Use the AI Sub-Agent Workflow!**
- See @.claude/rules/workflows/ai-sub-agent.md

**THEN: Ensure there's a feature design doc in `docs/features/` for this endpoint!**

**Key points:**
- Use DTOs for request/response (never expose Models directly)
- Add `[Authorize]` attribute unless endpoint should be public
- Return appropriate status codes (200, 201, 400, 404, 500)
- Update `backend/API.md` with documentation
- Mark as "⚠️ No Frontend Yet" in API.md if no UI exists
- Add to @INCOMPLETE_FEATURES.md if backend-only

## Controller Pattern

- Inherit from `ControllerBase`
- Add `[ApiController]` and `[Route]` attributes
- Add `[Authorize]` unless public endpoint
- Inject services via constructor
- Keep controllers thin (delegate to services)

## DTO Pattern

- Create separate Request and Response DTOs
- Never expose Models directly
- Use Data Annotations for validation
- Keep DTOs in `backend/DTOs/`

## Service Pattern

- Create interface in `Services/I*Service.cs`
- Implement in `Services/*Service.cs`
- **Register as Scoped in `Program.cs`** (commonly forgotten!)
- Business logic belongs in services, not controllers

## Critical Rules

- **Business logic belongs in Services**, not Controllers
- **Never expose Models directly** - always use DTOs
- **Always create migrations** after model changes
- **Always register new services** in `Program.cs` DI container
- **Add `[Authorize]` attribute** to all endpoints except login/register
- **Return proper HTTP status codes**
- **Validate inputs** using Data Annotations in DTOs

## Common AI Mistakes

- ❌ Forgetting to create migration after model changes
- ❌ Not registering new services in `Program.cs` DI container
- ❌ Putting business logic in Controllers instead of Services
- ❌ Exposing Models directly instead of using DTOs
- ❌ Modifying existing migrations (always create new ones)
- ❌ Not adding `[Authorize]` to protected endpoints

## API Documentation

See @backend/API.md for complete API reference.

**Key Endpoints:**
- `POST /api/auth/register` - Create account (returns JWT + user)
- `POST /api/auth/login` - Login (returns JWT + user)
- `GET /api/users/me` - Get current user (requires auth)
- `GET /api/vehicles` - List user's vehicles (requires auth)
- `POST /api/vehicles` - Create vehicle (requires auth)
- `GET /api/vehicles/{id}/fuel-efficiency` - Get fuel stats

**Authentication Header:**
```bash
Authorization: Bearer <jwt_token>
```
