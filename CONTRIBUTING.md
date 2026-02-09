# Contributing to CarManagement

Thank you for your interest in contributing to CarManagement! This guide will help you get started.

## 🚨 CRITICAL: Git Workflow

**ALL contributions MUST use feature branches - direct commits to `main` are not allowed.**

### Quick Start

```bash
# 1. Fork and clone the repository (if external contributor)
git clone https://github.com/YOUR_USERNAME/CarManagement.git
cd CarManagement

# 2. Always start from an updated main branch
git checkout main
git pull origin main

# 3. Create a feature branch for your work
git checkout -b feature/your-feature-name

# 4. Make your changes, commit frequently
git add .
git commit -m "Clear, descriptive commit message"

# 5. Push your feature branch
git push origin feature/your-feature-name

# 6. Create a Pull Request on GitHub
# - From: your-username:feature/your-feature-name
# - To: mkaraivanov:main
```

## Branch Naming Conventions

Use descriptive branch names with appropriate prefixes:

- `feature/` - New features (e.g., `feature/add-fuel-ui`, `feature/vehicle-export`)
- `fix/` - Bug fixes (e.g., `fix/auth-token-expiry`, `fix/mileage-calculation`)
- `refactor/` - Code refactoring (e.g., `refactor/service-layer`)
- `docs/` - Documentation updates (e.g., `docs/update-api-guide`)
- `test/` - Test additions/fixes (e.g., `test/add-vehicle-tests`)

## Development Workflow

### For New Features

1. **Create feature branch**: `git checkout -b feature/your-feature`
2. **Create design doc** (if significant): Copy `docs/features/_TEMPLATE.md`
3. **Implement changes**: Follow existing patterns and conventions
4. **Write tests**: Ensure new code has test coverage
5. **Run tests**: All tests must pass
6. **Update documentation**: Keep API.md and other docs current
7. **Push branch**: `git push origin feature/your-feature`
8. **Create Pull Request**: Submit for review

### For Bug Fixes

1. **Create fix branch**: `git checkout -b fix/issue-description`
2. **Write failing test**: Reproduce the bug
3. **Fix the bug**: Make minimal changes to fix the issue
4. **Verify test passes**: Ensure the fix works
5. **Run all tests**: Ensure no regressions
6. **Push branch**: `git push origin fix/issue-description`
7. **Create Pull Request**: Reference the issue number

## Code Quality Standards

### Testing Requirements

**All changes must include tests and all tests must pass before merging.**

```bash
# Run backend tests
cd backend
dotnet test

# Run frontend tests
cd web-frontend
npm test -- --run

# Both should show all tests passing
```

Current test coverage:
- Backend: 18 tests (xUnit)
- Frontend: 11 tests (Vitest + React Testing Library)

### Code Review Checklist

Before submitting a PR, ensure:

- [ ] Created and working on a feature branch (not `main`)
- [ ] All tests pass (backend + frontend)
- [ ] Code follows existing patterns and conventions
- [ ] New features have test coverage
- [ ] Documentation updated (API.md, README, etc.)
- [ ] No console errors or warnings
- [ ] Commit messages are clear and descriptive
- [ ] Branch can be merged cleanly (no conflicts)

## Architecture Overview

**Backend (ASP.NET Core):**
- Controllers → Services → Data/Models pattern
- DTOs for all API requests/responses
- Entity Framework Core with SQLite
- JWT authentication with BCrypt

**Frontend (React):**
- Components → Services → API pattern
- Material-UI for UI components
- React Context for auth state
- React Hook Form + Yup for validation

See [`CLAUDE.md`](CLAUDE.md) for complete architecture documentation.

## Common Patterns

### Backend

**Adding a new entity:**
1. Create Model in `backend/Models/`
2. Add DbSet to `ApplicationDbContext.cs`
3. Create migration: `dotnet ef migrations add AddEntityName`
4. Apply migration: `dotnet ef database update`
5. Create DTOs in `backend/DTOs/`
6. Create Service interface and implementation
7. Register Service in `Program.cs` DI container
8. Create Controller in `backend/Controllers/`
9. Update `backend/API.md`

**Adding a new endpoint:**
1. Add method to service interface + implementation
2. Add controller action with proper HTTP verb
3. Add `[Authorize]` unless endpoint should be public
4. Use DTOs for request/response
5. Return appropriate status codes
6. Update `backend/API.md`

### Frontend

**Adding a new feature:**
1. Create service method in `src/services/`
2. Use shared `api` instance from `src/services/api.js`
3. Handle errors with try-catch
4. Show user feedback with Material-UI Snackbar/Alert
5. Protected pages wrapped in `<ProtectedRoute>`

## Documentation

Update documentation when making changes:

- **API changes**: Update `backend/API.md`
- **New features**: Add to `docs/features/` (use `_TEMPLATE.md`)
- **Architecture changes**: Update `CLAUDE.md`
- **Workflow changes**: Update `WORKFLOWS.md`

## Getting Help

- Check [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) for common issues
- Review [`WORKFLOWS.md`](WORKFLOWS.md) for development workflows
- See [`TESTING.md`](TESTING.md) for testing guidelines
- Read [`CLAUDE.md`](CLAUDE.md) for architecture details

## Pull Request Process

1. **Create PR**: From your feature branch to `main`
2. **Write description**: Explain what and why
3. **Link issues**: Reference any related issues
4. **Request review**: Wait for maintainer review
5. **Address feedback**: Make requested changes
6. **Merge**: Maintainer will merge after approval
7. **Clean up**: Delete your feature branch after merge

## Code of Conduct

- Be respectful and professional
- Focus on constructive feedback
- Help others learn and grow
- Keep discussions on topic

## Questions?

Open an issue for questions or clarifications. We're here to help!

---

**Remember: Always use feature branches. Never commit directly to `main`.** ✅
