# Testing Requirements

**CRITICAL: Always run tests after making changes and before committing!**

## When to Run Tests

1. **After writing new code** - Verify it works correctly
2. **After fixing bugs** - Ensure the fix works and no regressions
3. **After refactoring** - Confirm behavior hasn't changed
4. **Before committing** - All tests must pass
5. **Before pushing** - Final verification

## Running All Tests

```bash
# Backend Tests (xUnit)
cd backend
dotnet test

# Frontend Tests (Vitest + React Testing Library)
cd web-frontend
npm test

# Run ALL tests (backend + frontend)
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

## Current Test Coverage

- Backend: 18 tests (authentication, vehicles, fuel, service records, OCR)
- Frontend: 11 tests (ExtractedDataReview component, confidence handling)
- **Total: 29 tests** - all must pass ✅

## Writing Tests for New Features

- Backend: Create test class in `Backend.Tests/` using xUnit
- Frontend: Create `.test.jsx` file alongside component using Vitest + React Testing Library

See @TESTING.md for detailed testing guidelines and patterns.
See @.claude/rules/backend/testing.md for backend test patterns.
See @.claude/rules/frontend/web/testing.md for frontend test patterns.
