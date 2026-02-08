---
name: build-all
description: Builds all projects in the solution (backend + web frontend + mobile frontend)
disable-model-invocation: true
argument-hint: ""
---

Builds the entire CarManagement solution by executing the build script.

## What This Does

1. **Backend (.NET)**: Builds ASP.NET Core 9.0 in Release mode
2. **Web Frontend (React)**: Builds production-ready React + Vite application
3. **Mobile Frontend (React Native)**: Verifies dependencies are installed

## Build Outputs

- Backend: `backend/bin/Release/net9.0/`
- Web Frontend: `web-frontend/dist/`
- Mobile Frontend: Dependencies verified (builds through native tools)

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

**Web Frontend:**
```bash
cd web-frontend && npm run dev
```
→ http://localhost:5173

**Mobile Frontend:**
```bash
cd mobile-frontend/CarManagementMobile
npm start                # Start Metro bundler
# In a new terminal:
npm run ios             # Run on iOS simulator (macOS only)
# OR
npm run android         # Run on Android emulator
```

## Notes

- The script uses `set -e` to exit immediately on any build failure
- Mobile frontend doesn't have a traditional "build" step - apps are compiled through Xcode (iOS) or Gradle (Android)
- The script verifies mobile dependencies are installed and ready
- All paths are automatically resolved relative to the project root
- Backend and web frontend builds create production-ready artifacts but don't start the servers
