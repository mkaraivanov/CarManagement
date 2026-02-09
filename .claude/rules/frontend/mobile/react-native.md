---
paths:
  - "mobile-frontend/**/*.{js,jsx,tsx,ts}"
  - "mobile-frontend/CarManagementMobile/**/*"
---

# Mobile Frontend (React Native)

## Mobile Development Setup

Mobile frontend is React Native with TypeScript.

**Features:**
- Authentication (login, register)
- Vehicles management
- Service records
- Fuel records
- Maintenance tracking
- User management

## Development Commands

```bash
cd mobile-frontend/CarManagementMobile
npm start                    # Start Metro bundler
npm run android             # Run on Android emulator
npm run ios                 # Run on iOS simulator

# iOS CocoaPods setup (first time or after dependency changes)
bundle install              # Install Ruby bundler for CocoaPods
bundle exec pod install     # Install iOS native dependencies
```

## Mobile Testing

```bash
cd mobile-frontend/CarManagementMobile

# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with coverage
npm test -- --coverage
```

## Mobile Patterns

- Start Metro bundler first: `npm start`
- Run platform-specific commands in separate terminal
- iOS requires CocoaPods setup
- Test hot reload after changes

## Test Coverage

**User Service Tests:**
- CRUD operations for users
- Error handling

**User List Screen Tests:**
- Rendering states
- User identification
- Statistics display
- User interactions
- Self-delete prevention

**User Form Screen Tests:**
- Form rendering and validation
- User interactions
- Error handling

See @TESTING.md for complete mobile testing documentation.
