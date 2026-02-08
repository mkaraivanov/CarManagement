#!/bin/bash
# Claude Skill: Run Mobile App
# Description: Runs the mobile app on iOS or Android
# Usage: /run-mobile [ios|android]

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Get the project root directory (two levels up from .claude/skills)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
MOBILE_DIR="$PROJECT_ROOT/mobile-frontend/CarManagementMobile"

# Default platform is iOS
PLATFORM="${1:-ios}"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Running CarManagement Mobile App${NC}"
echo -e "${BLUE}  Platform: ${PLATFORM}${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Check if backend is running
if ! curl -s http://localhost:5239/api/vehicles > /dev/null 2>&1; then
    echo -e "${YELLOW}⚠️  Warning: Backend API doesn't appear to be running${NC}"
    echo -e "${YELLOW}   The app needs the backend at http://localhost:5239${NC}"
    echo -e "${YELLOW}   Start it with: cd backend && dotnet run${NC}"
    echo ""
    read -p "Continue anyway? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

cd "$MOBILE_DIR"

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo -e "${YELLOW}Installing npm dependencies...${NC}"
    npm install
    echo -e "${GREEN}✓ Dependencies installed${NC}"
    echo ""
fi

# For iOS, check if CocoaPods dependencies are installed
if [ "$PLATFORM" = "ios" ]; then
    if [ ! -d "ios/Pods" ]; then
        echo -e "${YELLOW}Installing iOS CocoaPods dependencies...${NC}"
        cd ios
        pod install
        cd ..
        echo -e "${GREEN}✓ CocoaPods installed${NC}"
        echo ""
    fi
fi

# Check if Metro bundler is already running
if lsof -Pi :8081 -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo -e "${GREEN}✓ Metro bundler already running on port 8081${NC}"
    echo ""
else
    echo -e "${YELLOW}Starting Metro bundler in background...${NC}"
    npm start > /dev/null 2>&1 &
    METRO_PID=$!
    echo -e "${GREEN}✓ Metro bundler started (PID: $METRO_PID)${NC}"
    echo ""

    # Wait a few seconds for Metro to start
    echo -e "${BLUE}Waiting for Metro to initialize...${NC}"
    sleep 3
fi

# Run the app on the specified platform
echo -e "${YELLOW}Building and launching ${PLATFORM} app...${NC}"
echo -e "${BLUE}This may take a few minutes on first build...${NC}"
echo ""

if [ "$PLATFORM" = "ios" ]; then
    npx react-native run-ios
elif [ "$PLATFORM" = "android" ]; then
    npx react-native run-android
else
    echo -e "${RED}✗ Invalid platform: $PLATFORM${NC}"
    echo -e "${YELLOW}Usage: $0 [ios|android]${NC}"
    exit 1
fi

echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}✓ Mobile app launched successfully!${NC}"
echo ""
echo -e "${YELLOW}Useful commands:${NC}"
echo -e "  Press 'r' in the app to reload"
echo -e "  Press 'd' to open developer menu"
echo -e "  Shake device to open developer menu"
echo ""
echo -e "${YELLOW}To stop Metro bundler:${NC}"
echo -e "  lsof -ti:8081 | xargs kill -9"
echo -e "${BLUE}========================================${NC}"
