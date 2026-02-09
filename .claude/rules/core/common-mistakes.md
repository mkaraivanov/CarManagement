---
paths:
  - "**/*"
---

# Common Mistakes Consolidated

This document consolidates all common mistakes across backend and frontend to help you avoid them.

## Backend Common Mistakes ❌

### Architecture Violations
- ❌ **Business logic in Controllers** - Belongs in Services
- ❌ **Models exposed directly in responses** - Use DTOs instead
- ❌ **Controllers calling DbContext directly** - Use Services
- ❌ **Service not registered in Program.cs** - Commonly forgotten!

### API Issues
- ❌ **Missing `[Authorize]` attribute** - On protected endpoints
- ❌ **Wrong HTTP status codes** - 200 for errors, 500 for validation, etc.
- ❌ **Technical errors exposed to users** - Return user-friendly messages
- ❌ **No input validation** - Use Data Annotations in DTOs

### Database Issues
- ❌ **Model changed but no migration created** - Always create migration
- ❌ **Migration modified after commit** - Create new migration instead
- ❌ **Relationships not configured** - Configure in OnModelCreating()
- ❌ **Missing DbSet for new entities** - Add to ApplicationDbContext
- ❌ **Skipping `dotnet ef database update`** - Migration not applied

### Security Issues
- ❌ **Passwords not hashed** - Use BCrypt
- ❌ **SQL injection vulnerabilities** - Use parameterized queries (EF Core does this)
- ❌ **Sensitive data in error messages** - Stack traces, connection strings
- ❌ **Missing authorization checks** - Users accessing others' data

### Service Layer Issues
- ❌ **Returning Models instead of DTOs** - Always return DTOs
- ❌ **Not using async/await** - All database operations should be async
- ❌ **Catching exceptions silently** - Log errors, return meaningful responses

## Frontend Common Mistakes ❌

### Architecture Violations
- ❌ **axios called directly from components** - Use service layer
- ❌ **Service returns full axios response** - Return `response.data` only
- ❌ **Business logic in components** - Extract to services/utils
- ❌ **No ProtectedRoute on authenticated pages** - Unauthorized access possible

### User Experience Issues
- ❌ **No loading states** - During async operations
- ❌ **Technical errors shown to users** - "Network Error", stack traces
- ❌ **No error handling for API calls** - App crashes on failure
- ❌ **Form doesn't show validation errors** - User doesn't know what's wrong

### Code Quality Issues
- ❌ **Unused imports/variables** - Clean up
- ❌ **console.log() statements left in code** - Remove before commit
- ❌ **Inline styles instead of Material-UI** - Use sx prop or styled
- ❌ **Duplicate code not refactored** - Extract to reusable components/hooks

### State Management Issues
- ❌ **Not using React Hook Form** - For complex forms
- ❌ **Form validation in submit handler** - Use Yup schema
- ❌ **No form reset after submit** - Forms retain old data

### API Integration Issues
- ❌ **Creating new axios instances** - Use shared `api` instance
- ❌ **Not handling 401 responses** - Token expired, should redirect
- ❌ **Hardcoded API URLs** - Use environment variables

## Git & Workflow Mistakes ❌

### Commit Issues
- ❌ **Committing broken code** - Always test before commit
- ❌ **Not running tests before push** - Could break CI/CD
- ❌ **Vague commit messages** - "fix", "update", "changes"
- ❌ **Committing secrets/API keys** - Use .env files, add to .gitignore

### Migration Issues
- ❌ **Committing without migration** - Model changed but no migration
- ❌ **Not applying migration locally** - DB out of sync
- ❌ **Pushing migration without testing** - Could fail on other machines

### Documentation Issues
- ❌ **API.md not updated** - After adding endpoints
- ❌ **Not marking incomplete features** - In INCOMPLETE_FEATURES.md
- ❌ **No design doc for major features** - docs/features/

## Testing Mistakes ❌

### Backend Testing
- ❌ **Not using TestWebApplicationFactory** - Direct DbContext usage
- ❌ **Not testing authorization** - Users accessing others' data
- ❌ **Not testing validation** - Invalid inputs accepted
- ❌ **Hardcoded test data** - Makes tests brittle

### Frontend Testing
- ❌ **Not using React Testing Library** - Direct DOM manipulation
- ❌ **Testing implementation details** - Instead of user behavior
- ❌ **Not testing error states** - Happy path only
- ❌ **Not testing loading states** - Assumes instant responses

### General Testing
- ❌ **Not running tests before commit** - Breaking existing functionality
- ❌ **Skipping test creation** - For new features
- ❌ **Not updating tests** - After changing functionality

## Configuration Mistakes ❌

### Backend Configuration
- ❌ **JWT secret too short** - Must be 32+ characters
- ❌ **Wrong connection string** - Database file not found
- ❌ **CORS not configured** - Frontend can't access API
- ❌ **Wrong CORS origins** - Frontend on different port

### Frontend Configuration
- ❌ **Wrong VITE_API_URL** - Can't reach backend
- ❌ **Missing .env file** - App uses defaults
- ❌ **.env committed to git** - Secrets exposed

## How to Avoid These Mistakes

### Before Coding
1. Read existing similar code
2. Understand project patterns
3. Check if feature design doc exists
4. Use AI Sub-Agent Workflow for new features

### During Coding
1. Follow existing patterns exactly
2. Write clean, readable code
3. Handle errors gracefully
4. Test as you go

### Before Committing
1. Run all tests (backend + frontend)
2. Check migrations created/applied
3. Check services registered
4. Remove console.log() statements
5. Review your changes

### After Committing
1. Push to remote if tests pass
2. Update documentation if needed
3. Mark incomplete features

## Quick Checklist

Before committing, verify:
- [ ] If database models changed: migration created AND applied
- [ ] If new service created: registered in `Program.cs` DI container
- [ ] Backend compiles: `cd backend && dotnet build`
- [ ] Frontend lints: `cd web-frontend && npm run lint`
- [ ] All tests pass: `cd backend && dotnet test && cd ../web-frontend && npm test -- --run`
- [ ] backend/API.md updated if endpoints changed
- [ ] No console errors in browser developer tools
- [ ] No exceptions in backend console output
- [ ] No console.log() left in code
- [ ] No technical errors exposed to users
