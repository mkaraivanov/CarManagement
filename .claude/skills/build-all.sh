#!/bin/bash
# Claude Skill: Build All Projects
# Description: Builds backend and frontend projects in the CarManagement solution
# Usage: /build-all

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Get the project root directory (parent of .claude)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Building CarManagement Solution${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Track overall success
BUILD_SUCCESS=true

# 1. Build Backend (ASP.NET Core)
echo -e "${YELLOW}[1/2] Building Backend (.NET)...${NC}"
cd "$PROJECT_ROOT/backend"
if dotnet build --configuration Release; then
    echo -e "${GREEN}✓ Backend build succeeded${NC}"
    echo ""
else
    echo -e "${RED}✗ Backend build failed${NC}"
    BUILD_SUCCESS=false
fi

# 2. Build Web Frontend (React + Vite)
echo -e "${YELLOW}[2/3] Building Web Frontend (React)...${NC}"
cd "$PROJECT_ROOT/web-frontend"
if npm run build; then
    echo -e "${GREEN}✓ Frontend build succeeded${NC}"
    echo ""
else
    echo -e "${RED}✗ Frontend build failed${NC}"
    BUILD_SUCCESS=false
fi

# 3. Verify Mobile Frontend Dependencies (React Native)
echo -e "${YELLOW}[3/3] Verifying Mobile Frontend (React Native)...${NC}"
cd "$PROJECT_ROOT/mobile-frontend/CarManagementMobile"
if [ ! -d "node_modules" ]; then
    echo -e "${BLUE}Installing mobile dependencies...${NC}"
    if npm install; then
        echo -e "${GREEN}✓ Mobile dependencies installed${NC}"
        echo ""
    else
        echo -e "${RED}✗ Mobile dependency installation failed${NC}"
        BUILD_SUCCESS=false
    fi
else
    echo -e "${GREEN}✓ Mobile dependencies verified${NC}"
    echo -e "${BLUE}Note: Mobile apps build through native tools (Xcode/Android Studio)${NC}"
    echo ""
fi

# Summary
echo -e "${BLUE}========================================${NC}"
if [ "$BUILD_SUCCESS" = true ]; then
    echo -e "${GREEN}✓ All builds completed successfully!${NC}"
    echo ""
    echo -e "Build outputs:"
    echo -e "  Backend:      ${PROJECT_ROOT}/backend/bin/Release/net9.0/"
    echo -e "  Web Frontend: ${PROJECT_ROOT}/web-frontend/dist/"
    echo ""
    echo -e "${YELLOW}To run the applications:${NC}"
    echo -e "  Backend:      cd backend && dotnet run"
    echo -e "                → http://localhost:5239/api"
    echo -e "  Web Frontend: cd web-frontend && npm run dev"
    echo -e "                → http://localhost:5173"
    echo -e "  Mobile:       cd mobile-frontend/CarManagementMobile && npm start"
    echo -e "                Then run: npm run ios OR npm run android"
    echo -e "${BLUE}========================================${NC}"
    exit 0
else
    echo -e "${RED}✗ One or more builds failed${NC}"
    echo -e "${BLUE}========================================${NC}"
    exit 1
fi
