# Troubleshooting Guide

This document provides solutions to common issues encountered during development and deployment of the CarManagement application.

## Table of Contents
- [Port Conflicts](#port-conflicts)
- [Database Issues](#database-issues)
- [Authentication Issues](#authentication-issues)
- [CORS Errors](#cors-errors)
- [Build and Compilation Errors](#build-and-compilation-errors)
- [Frontend Issues](#frontend-issues)
- [Mobile Development Issues](#mobile-development-issues)
- [API Communication Issues](#api-communication-issues)

## Port Conflicts

### Symptom
Error message: "Address already in use" or "Port XXXX is already in use"

### Solution

**Kill backend process (port 5239):**
```bash
lsof -ti:5239 | xargs kill -9
```

**Kill frontend process (port 5173):**
```bash
lsof -ti:5173 | xargs kill -9
```

**Find what's using a specific port:**
```bash
lsof -i :5239
# or
netstat -vanp tcp | grep 5239
```

## Database Issues

### Database Corruption

**Symptoms:**
- SQLite errors in backend console
- "Database is locked" errors
- Malformed database errors

**Solution:**
```bash
# Delete and recreate database
rm backend/carmanagement.db
cd backend && dotnet ef database update
```

### Migration Errors

**Symptom:** Migration fails to apply or causes errors

**Solution:**
```bash
# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Remove the problematic migration
dotnet ef migrations remove

# Fix the issue in your models/context

# Create a new migration
dotnet ef migrations add FixedMigrationName

# Apply it
dotnet ef database update
```

### Database File Missing

**Symptom:** "Cannot find database file" or "No such table" errors

**Solution:**
```bash
# Create the database
cd backend
dotnet ef database update

# Verify it was created
ls -la carmanagement.db
```

### Database Connection String Issues

**Check configuration:**
```bash
# Verify appsettings.json has correct path
cat backend/appsettings.json | grep ConnectionStrings
```

**Should look like:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=carmanagement.db"
}
```

## Authentication Issues

### JWT Token Issues

**Symptom:** 401 Unauthorized errors even after logging in

**Check:**
1. Token is being sent in requests (check browser Network tab → Headers)
2. Token hasn't expired (default: 1 hour)
3. JWT Secret in `appsettings.json` is at least 32 characters

**Solution:**
```bash
# Clear browser localStorage
# In browser console:
localStorage.clear()
location.reload()
```

### JWT Secret Too Short

**Symptom:** Backend crashes with "IDX10603: The algorithm 'HS256' requires key size of at least '256' bits"

**Solution:**
Edit `backend/appsettings.json`:
```json
"Jwt": {
  "Secret": "your-very-long-secret-key-at-least-32-characters-long",
  "Issuer": "CarManagementAPI",
  "Audience": "CarManagementAPI",
  "ExpirationHours": 1
}
```

### Token Expiration

**Symptom:** Token works initially but then starts returning 401 errors

**Solution:**
- Default token expiration is 1 hour
- User needs to log in again to get a new token
- To extend: change `ExpirationHours` in `appsettings.json`

### 401 Unauthorized for User's Own Resources

**Symptom:** User can't access their own vehicles/records

**Check:**
1. Verify `UserId` is correctly set in JWT claims during login
2. Check controller is getting correct user ID: `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
3. Verify database relationships are correct

**Common mistake:**
```csharp
// Wrong - compares string to int
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userIdInt = int.Parse(userId); // This might fail or return wrong value
```

## CORS Errors

### Symptom
Browser console shows: "Origin http://localhost:XXXX is not allowed by Access-Control-Allow-Origin"

### Cause
Frontend port doesn't match the allowed origins in backend CORS configuration

### Solution

1. **Check which port the frontend is running on** (look at Vite dev server output)
2. **Update backend CORS policy** in [`backend/Program.cs`](backend/Program.cs) around line 62:
   ```csharp
   policy.WithOrigins(
       "http://localhost:5173",
       "http://localhost:5175",  // Add your port here
       "http://localhost:XXXX"   // If using a different port
   )
   ```
3. **Restart the backend**: `cd backend && dotnet run`

### Note
Vite uses port 5173 by default, but will automatically use 5175, 5176, etc. if 5173 is already in use.

### Preflight Requests Failing

**Symptom:** OPTIONS requests return 404 or 405

**Check:**
Ensure CORS middleware is added before routing in `Program.cs`:
```csharp
app.UseCors("AllowSpecificOrigins");  // Must be before UseAuthorization
app.UseAuthorization();
app.MapControllers();
```

## Build and Compilation Errors

### Backend Won't Compile

**Check for:**
- Missing package references
- Syntax errors in C# files
- Missing `using` statements

**Solution:**
```bash
cd backend
dotnet clean
dotnet restore
dotnet build
```

### Frontend Won't Start

**Symptoms:**
- "Module not found" errors
- "Cannot find module 'vite'" or similar

**Solution:**
```bash
cd web-frontend
rm -rf node_modules package-lock.json
npm install
npm run dev
```

### ESLint Errors

**Run linter to see all errors:**
```bash
cd web-frontend
npm run lint
```

**Auto-fix many errors:**
```bash
npm run lint -- --fix
```

## Frontend Issues

### Blank Page After Login

**Check:**
1. Browser console for JavaScript errors
2. Network tab for failed API calls
3. AuthContext is properly wrapping app in `src/main.jsx`

**Common cause:**
Protected routes are checked before AuthContext loads user data

**Solution:**
Ensure `AuthProvider` wraps your router in `main.jsx`:
```jsx
<AuthProvider>
  <RouterProvider router={router} />
</AuthProvider>
```

### API Calls Return Empty Data

**Check:**
1. Backend console for errors
2. Network tab → Response for actual backend response
3. Service layer is returning `response.data`, not full response

**Example service pattern:**
```javascript
export const getVehicles = async () => {
  const response = await api.get('/vehicles');
  return response.data;  // Return just the data
};
```

### React State Not Updating

**Common mistakes:**
- Mutating state directly instead of creating new objects/arrays
- Not using setter functions from `useState`
- Async state updates not finishing before reading

**Solution:**
```javascript
// Wrong
vehicles.push(newVehicle);
setVehicles(vehicles);

// Correct
setVehicles([...vehicles, newVehicle]);
```

### Material-UI Components Not Styled

**Check:**
- `@mui/material` is installed
- Imports are correct: `import { Button } from '@mui/material';`
- No CSS conflicts overriding MUI styles

## Mobile Development Issues

### Metro Bundler Won't Start

**Solution:**
```bash
cd mobile-frontend/CarManagementMobile
# Clear Metro cache
npm start -- --reset-cache
```

### iOS Build Fails

**Common causes:**
- CocoaPods not installed or outdated
- Pods not installed

**Solution:**
```bash
cd mobile-frontend/CarManagementMobile/ios
bundle install
bundle exec pod install

# If still failing, try:
bundle exec pod deintegrate
bundle exec pod install
```

### Android Emulator Connection Issues

**Check:**
1. Emulator is running: `adb devices`
2. Metro bundler is accessible from emulator

**Solution:**
```bash
# Restart ADB
adb kill-server
adb start-server

# Reverse port forwarding for API (if testing with local backend)
adb reverse tcp:5239 tcp:5239
```

## API Communication Issues

### Network Request Failed

**Symptoms:**
- "Network Error" in browser console
- API calls timeout

**Check:**
1. Backend is running: `curl http://localhost:5239/api/users/me`
2. Correct API URL in `.env`: `VITE_API_URL=http://localhost:5239/api`
3. No firewall blocking requests

### 404 Not Found for Valid Endpoints

**Check:**
1. Endpoint exists in controller
2. Route is correctly defined: `[HttpGet]`, `[Route("api/[controller]")]`
3. API URL doesn't have double `/api/api/`

**Common mistake:**
```javascript
// Wrong - if VITE_API_URL already includes /api
const response = await api.get('/api/vehicles');

// Correct
const response = await api.get('/vehicles');
```

### Request Timeout

**Causes:**
- Backend is hung or processing slowly
- Large response size
- Network issues

**Solution:**
```javascript
// Increase timeout in api.js
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000  // 10 seconds instead of default 5s
});
```

### Cannot Read Response Data

**Symptom:** `response.data` is undefined or wrong shape

**Check:**
1. Backend is returning correct DTO
2. Content-Type header is `application/json`
3. Response status is 200 (not 204 No Content)

**Debug:**
```javascript
try {
  const response = await api.get('/vehicles');
  console.log('Full response:', response);
  console.log('Data:', response.data);
  console.log('Status:', response.status);
  return response.data;
} catch (error) {
  console.error('Error details:', error.response);
}
```

## General Debugging Tips

### Enable Verbose Logging

**Backend:**
Edit `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

**Frontend:**
Check browser console with appropriate filtering (Errors, Warnings, Info)

### Check Recent Changes

```bash
# See recent commits
git log --oneline -10

# See changes in current branch
git diff main

# Check if specific file changed recently
git log --follow -- path/to/file
```

### Clean Slate Approach

When all else fails:

```bash
# Backend
cd backend
rm carmanagement.db
dotnet clean
dotnet build
dotnet ef database update
dotnet run

# Frontend (in new terminal)
cd web-frontend
rm -rf node_modules package-lock.json
npm install
npm run dev

# Test with fresh data
# Register new user, create test vehicle
```

## Getting Help

### Information to Gather

When asking for help (or debugging with AI), provide:

1. **Error message** (full text, not paraphrased)
2. **What you were doing** (exact steps to reproduce)
3. **What you expected** vs **what happened**
4. **Relevant code** (controller action, service method, component)
5. **Console output** (backend terminal and browser console)
6. **Environment**:
   - OS and version
   - .NET version: `dotnet --version`
   - Node version: `node --version`
   - npm version: `npm --version`

### Quick Health Check

```bash
# Backend
cd backend && dotnet build && echo "✅ Backend builds"

# Backend tests
cd backend/Backend.Tests && dotnet test && echo "✅ Tests pass"

# Frontend
cd web-frontend && npm run lint && echo "✅ Linting passes"

# Database exists
ls -la backend/carmanagement.db && echo "✅ Database exists"

# Ports available
! lsof -ti:5239 && echo "✅ Port 5239 available" || echo "❌ Port 5239 in use"
! lsof -ti:5173 && echo "✅ Port 5173 available" || echo "❌ Port 5173 in use"
```
