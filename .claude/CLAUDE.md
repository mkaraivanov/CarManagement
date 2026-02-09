# CarManagement Project

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CarManagement is a full-stack vehicle management application with three main components:
- **Backend**: ASP.NET Core 9.0 REST API with SQLite database
- **Web Frontend**: React 19 + Vite + Material-UI
- **Mobile Frontend**: React Native with TypeScript

## Documentation

### Core Documentation
- @WORKFLOWS.md - Development workflows and best practices
- @TESTING.md - Testing strategies and procedures
- @TROUBLESHOOTING.md - Common issues and solutions
- @INCOMPLETE_FEATURES.md - Feature completion tracker

### API & Feature Documentation
- @backend/API.md - Complete API reference with request/response examples
- @docs/README.md - Complete documentation guidelines

### Quick Reference
- **New features?** → See @.claude/rules/workflows/ai-sub-agent.md
- **Tests failing?** → See @.claude/rules/testing.md
- **Errors?** → See @TROUBLESHOOTING.md
- **Backend API without UI?** → Document in @INCOMPLETE_FEATURES.md

## 🚨 CRITICAL: AI Sub-Agent Workflow

**Before implementing ANY new feature, you MUST use the AI Sub-Agent Workflow!**

See @.claude/rules/workflows/ai-sub-agent.md for complete details.

**The 5-Phase Process:**
1. PLAN (background) → Spawn Plan sub-agent
2. USER REVIEW → Get approval & complexity decision
3. IMPLEMENT (background) → Spawn senior/regular-software-engineer
4. QA VALIDATION (background) → **AUTOMATICALLY** spawn qa-engineer
5. CODE REVIEW (background) → **AUTOMATICALLY** spawn code-reviewer

**Phases 4 & 5 are AUTOMATIC** - spawn immediately after implementation, don't wait for user.

## 🚨 CRITICAL: Testing Requirements

**Always run tests before committing!**

```bash
# Backend + Frontend tests (all must pass)
cd backend && dotnet test && cd ../web-frontend && npm test -- --run
```

**Expected:** ALL tests passing (backend + frontend) ✅

See @.claude/rules/testing.md for details.

## Quick Commands

See @.claude/rules/commands.md for all development commands.

**Start Development:**
```bash
cd backend && dotnet run              # Backend: http://localhost:5239
cd web-frontend && npm run dev        # Frontend: http://localhost:5173
cd mobile-frontend/CarManagementMobile && npm start  # Mobile
```

## Feature Completion Definition

**CRITICAL: A feature is NOT complete until it has BOTH backend AND frontend components.**

- Backend-only APIs = incomplete
- Frontend-only UIs = incomplete
- Track incomplete features in @INCOMPLETE_FEATURES.md

## Code Quality Standards

**Follow Existing Patterns:**
- Backend: Controllers → Services → Data/Models
- Frontend: Components → Services → API
- Never expose Models directly (use DTOs)
- Use service layer for all API calls (never axios directly)

**Common Mistakes to Avoid:**
- ❌ Forgetting to create migrations after model changes
- ❌ Not registering services in `Program.cs`
- ❌ Putting business logic in Controllers
- ❌ Calling axios directly from components
- ❌ Not adding `[Authorize]` to protected endpoints

## Development Notes

### Adding Copilot/AI Instructions

Copilot instructions are in `.github/copilot-instructions.md`. This project uses:
- Service layer pattern (Controllers delegate to Services)
- JWT auth for all endpoints except login/register
- Auto-calculations in backend services
- AuthContext and protected routes on frontend

### Mobile Development

React Native setup with TypeScript. Features include:
- Authentication, vehicles, service records, fuel records
- User management
- iOS requires CocoaPods: `bundle install && bundle exec pod install`
