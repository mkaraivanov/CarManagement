#!/bin/bash
# Claude Skill: Run All Tests
# Description: Runs backend and frontend tests for the CarManagement solution
# Usage: /test-all

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Get the project root directory
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Running All Tests${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Track overall success
TEST_SUCCESS=true

# 1. Run Backend Tests
echo -e "${YELLOW}[1/2] Running Backend Tests (.NET xUnit)...${NC}"
cd "$PROJECT_ROOT/backend"
if dotnet test --verbosity normal; then
    echo -e "${GREEN}✓ Backend tests passed${NC}"
    echo ""
else
    echo -e "${RED}✗ Backend tests failed${NC}"
    TEST_SUCCESS=false
fi

# 2. Run Frontend Tests
echo -e "${YELLOW}[2/2] Running Frontend Tests (Vitest)...${NC}"
cd "$PROJECT_ROOT/web-frontend"
if npm test -- --run; then
    echo -e "${GREEN}✓ Frontend tests passed${NC}"
    echo ""
else
    echo -e "${RED}✗ Frontend tests failed${NC}"
    TEST_SUCCESS=false
fi

# Summary
echo -e "${BLUE}========================================${NC}"
if [ "$TEST_SUCCESS" = true ]; then
    echo -e "${GREEN}✓ All tests passed!${NC}"
    echo -e "${BLUE}========================================${NC}"
    exit 0
else
    echo -e "${RED}✗ Some tests failed${NC}"
    echo -e "${BLUE}========================================${NC}"
    exit 1
fi
