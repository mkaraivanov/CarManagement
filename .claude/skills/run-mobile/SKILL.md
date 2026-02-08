---
name: run-mobile
description: Runs the mobile app on iOS or Android simulator
disable-model-invocation: true
argument-hint: "[ios|android]"
---

Runs the CarManagement mobile app on iOS simulator or Android emulator.

## What This Does

1. **Starts Metro Bundler**: Launches the JavaScript bundler in the background
2. **Verifies Dependencies**: Checks if CocoaPods (iOS) dependencies are installed
3. **Builds & Launches App**: Compiles and runs the app on the specified platform

## Usage

```bash
/run-mobile [platform]
```

**Platform options:**
- `ios` - Run on iOS simulator (macOS only, default)
- `android` - Run on Android emulator

## Execution

The script will be executed using:

```bash
bash /Users/martin.karaivanov/Projects/CarManagement/.claude/skills/run-mobile.sh [platform]
```

## Prerequisites

### iOS
- macOS with Xcode installed
- iOS Simulator
- CocoaPods installed

### Android
- Android Studio with Android SDK
- Android Emulator running or device connected

## Backend Requirement

⚠️ **Important**: The backend API must be running before starting the mobile app!

```bash
cd backend && dotnet run
```

Backend should be available at `http://localhost:5239`

## Notes

- First build takes 3-5 minutes (compiles native code)
- Subsequent builds are faster (~2-3 minutes)
- Metro bundler runs in the background
- Hot reload is enabled - code changes auto-refresh
- The script handles CocoaPods installation automatically for iOS
