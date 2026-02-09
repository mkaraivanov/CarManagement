# Git Workflow

## Branching Strategy

- Create feature branches for significant changes: `feature/add-fuel-ui`, `fix/auth-bug`, `refactor/service-layer`
- Keeps `main` branch clean and makes it easier to abandon unsuccessful experiments
- Can push to `main` directly for small fixes; use branches for larger features
- Delete branches after merging or abandoning

## Commit Practices

- Commit when code works and is tested (not broken code)
- Clear commit messages describing what and why
- Test end-to-end before pushing

## Before Committing - Checklist

### Must Verify
- [ ] If database models changed: migration created AND applied
- [ ] If new service created: registered in `Program.cs` DI container
- [ ] Backend compiles: `cd backend && dotnet build`
- [ ] Frontend lints: `cd web-frontend && npm run lint`
- [ ] Tested end-to-end (both UI and API working together)
- [ ] `backend/API.md` updated if endpoints changed
- [ ] No console errors in browser developer tools
- [ ] No exceptions in backend console output

### Testing Before Commit

**CRITICAL: Run all tests before committing any changes!**

```bash
# Backend Tests
cd backend && dotnet test

# Frontend Tests
cd web-frontend && npm test -- --run

# Run ALL tests before pushing
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

**Expected Results:**
- Backend: 18 tests passing ✅
- Frontend: 11 tests passing ✅
- **Total: 29 tests** - All must pass before committing!

### Test Workflow

```bash
# 1. Make your changes

# 2. Run affected tests
cd backend && dotnet test  # If you changed backend
cd web-frontend && npm test -- --run  # If you changed frontend

# 3. If all tests pass, commit
git add .
git commit -m "Your message"

# 4. Run ALL tests before pushing
cd backend && dotnet test && cd ../web-frontend && npm test -- --run

# 5. If all tests pass, push
git push origin main
```

See @TESTING.md for complete testing documentation.
