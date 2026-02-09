# Development Commands

## Backend (ASP.NET Core)

```bash
cd backend
dotnet run                    # Start backend on http://localhost:5239
dotnet build                  # Build the project
dotnet watch run             # Run with hot reload
dotnet ef database update    # Apply pending migrations
dotnet ef migrations add <Name>  # Create new migration
```

## Web Frontend (React + Vite)

```bash
cd web-frontend
npm run dev                  # Start dev server on http://localhost:5173
npm run build                # Production build
npm run lint                 # Run ESLint
npm run preview              # Preview production build
npm test                     # Run frontend tests (Vitest)
npm run test:ui              # Run tests with visual UI
npm run test:coverage        # Run tests with coverage report
```

## Mobile Frontend (React Native)

```bash
cd mobile-frontend/CarManagementMobile
npm start                    # Start Metro bundler
npm run android             # Run on Android emulator
npm run ios                 # Run on iOS simulator

# iOS CocoaPods setup (first time or after dependency changes)
bundle install              # Install Ruby bundler for CocoaPods
bundle exec pod install     # Install iOS native dependencies
```

## Testing Commands

```bash
# Backend Tests (xUnit)
cd backend
dotnet test                                    # Run all backend tests
dotnet test --filter "ClassName~YourTest"     # Run specific test class
dotnet test --logger "console;verbosity=detailed"  # Verbose output

# Frontend Tests (Vitest + React Testing Library)
cd web-frontend
npm test                                       # Run all frontend tests
npm run test:ui                               # Run with visual UI
npm run test:coverage                         # Run with coverage report

# Run ALL tests (backend + frontend)
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

## Database Management

```bash
# Reset database (deletes all data)
rm backend/carmanagement.db
cd backend && dotnet ef database update

# View database
cd backend && sqlite3 carmanagement.db
```
