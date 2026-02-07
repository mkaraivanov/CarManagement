---
name: build-all
description: Builds all projects in the solution (backend + frontend)
disable-model-invocation: true
argument-hint: ""
---

Builds the entire CarManagement solution by executing the build script.

## What This Does

1. **Backend (.NET)**: Builds ASP.NET Core 9.0 in Release mode
2. **Frontend (React)**: Builds production-ready React + Vite application

## Build Outputs

- Backend: `backend/bin/Release/net9.0/`
- Frontend: `web-frontend/dist/`

## Execution

Run the build script using the Bash tool:

```bash
bash /Users/martin.karaivanov/Projects/CarManagement/.claude/skills/build-all.sh
```

The script will:
- Use colored output to show build progress
- Build both projects sequentially
- Report success/failure for each build
- Exit with code 0 on success, 1 on failure

## Application URLs

After building, start the applications with:

**Backend:**
```bash
cd backend && dotnet run
```
→ http://localhost:5239/api

**Frontend:**
```bash
cd web-frontend && npm run dev
```
→ http://localhost:5173

## Notes

- The script uses `set -e` to exit immediately on any build failure
- Mobile frontend build is commented out but can be enabled if needed
- All paths are automatically resolved relative to the project root
- Build creates production-ready artifacts but doesn't start the servers
