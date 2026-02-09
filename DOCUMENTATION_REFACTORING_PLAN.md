# Documentation Refactoring Plan

## Goal
Restructure documentation to use `.claude/rules/` modular pattern for better organization, reduced context size, and path-specific rule application.

## Proposed Structure

```
.claude/
├── CLAUDE.md                           # Concise overview + imports (keep under 100 lines)
├── CLAUDE.local.md                     # Personal preferences (gitignored)
└── rules/
    ├── backend/
    │   ├── architecture.md             # Paths: backend/**/*.cs
    │   ├── api-conventions.md          # Paths: backend/Controllers/**/*.cs
    │   ├── services.md                 # Paths: backend/Services/**/*.cs
    │   ├── database.md                 # Paths: backend/Data/**/*.cs, backend/Models/**/*.cs
    │   └── testing.md                  # Paths: backend/**/*.cs, Backend.Tests/**/*.cs
    ├── frontend/
    │   ├── web/
    │   │   ├── react-patterns.md       # Paths: web-frontend/src/**/*.{jsx,tsx}
    │   │   ├── api-integration.md      # Paths: web-frontend/src/services/**/*.js
    │   │   └── testing.md              # Paths: web-frontend/**/*.test.{js,jsx}
    │   └── mobile/
    │       └── react-native.md         # Paths: mobile-frontend/**/*.{js,jsx,tsx}
    ├── workflows/
    │   ├── ai-sub-agent.md             # AI sub-agent workflow (Plan → Implement → QA → Review)
    │   ├── git.md                      # Git workflow and best practices
    │   └── feature-development.md      # Feature design doc process
    ├── testing.md                      # General testing requirements
    ├── security.md                     # Security best practices
    └── commands.md                     # Common development commands

# Keep in root (reference docs, not rules)
TROUBLESHOOTING.md                      # Debugging guide
INCOMPLETE_FEATURES.md                  # Feature tracker
backend/API.md                          # API reference
docs/                                   # Feature design docs
```

## Benefits

### 1. **Path-Specific Loading**
- Backend rules only load when working on C# files
- Web frontend rules only load when working on React files
- Mobile rules only load when working on React Native files
- Reduces context size and improves relevance

### 2. **Better Organization**
- Each file focused on one topic
- Easier to find and update specific rules
- Team members can own specific rule files

### 3. **Reduced Context Size**
- Current CLAUDE.md: ~500+ lines (all loaded)
- Proposed main CLAUDE.md: ~50-100 lines
- Topic-specific rules: only loaded when relevant

### 4. **Improved Maintainability**
- Changes to backend patterns don't require updating frontend sections
- Can update database rules without touching API conventions
- Each file is self-contained and focused

## Migration Steps

### Phase 1: Create Structure
1. Create `.claude/` directory
2. Create subdirectories: `rules/backend/`, `rules/frontend/web/`, `rules/frontend/mobile/`, `rules/workflows/`

### Phase 2: Split CLAUDE.md Content

**Extract to `.claude/rules/backend/architecture.md`:**
```yaml
---
paths:
  - "backend/**/*.cs"
  - "backend/Models/**/*.cs"
  - "backend/Services/**/*.cs"
  - "backend/Controllers/**/*.cs"
  - "backend/Data/**/*.cs"
---

# Backend Architecture

## Layered Pattern
Controllers/ → Services/ → Data/Models

## Service Registration
All services registered as Scoped in Program.cs
...
```

**Extract to `.claude/rules/backend/api-conventions.md`:**
```yaml
---
paths:
  - "backend/Controllers/**/*.cs"
  - "backend/DTOs/**/*.cs"
---

# API Endpoint Conventions

## Request/Response Pattern
- Use DTOs for all request/response
- Add [Authorize] attribute
- Return appropriate status codes
...
```

**Extract to `.claude/rules/frontend/web/react-patterns.md`:**
```yaml
---
paths:
  - "web-frontend/src/**/*.jsx"
  - "web-frontend/src/**/*.tsx"
  - "web-frontend/src/components/**/*"
---

# React Patterns

## Component Structure
- Use functional components with hooks
- Extract reusable logic to custom hooks
...
```

**Extract to `.claude/rules/workflows/ai-sub-agent.md`:**
```markdown
# AI Sub-Agent Workflow

## When to Use
✅ Adding new features
✅ New API endpoints
✅ Database entity changes
...

## 5-Phase Process
1. PLAN (background)
2. USER REVIEW
3. IMPLEMENT (background)
4. QA VALIDATION (background) - AUTOMATIC
5. CODE REVIEW (background) - AUTOMATIC
```

### Phase 3: Create Concise Main CLAUDE.md
```markdown
# CarManagement Project

Vehicle management application with:
- Backend: ASP.NET Core 9.0 + SQLite
- Web: React 19 + Vite + Material-UI
- Mobile: React Native + TypeScript

## Documentation
- @WORKFLOWS.md - Development workflows
- @TESTING.md - Testing requirements
- @TROUBLESHOOTING.md - Common issues
- @INCOMPLETE_FEATURES.md - Feature tracker
- @backend/API.md - API reference

## Quick Commands
See @.claude/rules/commands.md

## Critical Rules
⚠️ **ALWAYS use AI Sub-Agent Workflow for new features**
See @.claude/rules/workflows/ai-sub-agent.md

⚠️ **Run all tests before committing**
See @.claude/rules/testing.md
```

### Phase 4: Move Existing Docs
- Move relevant sections from WORKFLOWS.md → `.claude/rules/workflows/*.md`
- Move relevant sections from TESTING.md → `.claude/rules/testing.md`
- Keep TROUBLESHOOTING.md as reference (not rules)
- Keep INCOMPLETE_FEATURES.md as tracker

### Phase 5: Test & Validate
1. Start Claude Code session
2. Use `/memory` command to verify files are loaded
3. Test that path-specific rules load correctly
4. Verify imports work as expected

## Example Path-Specific Rule

```markdown
---
paths:
  - "backend/Controllers/**/*.cs"
  - "backend/Services/**/*.cs"
---

# Backend API Development

## When Adding New Endpoints

1. Create DTOs in backend/DTOs/
2. Create Service interface and implementation
3. **Register Service in Program.cs** (commonly forgotten!)
4. Create Controller
5. Update backend/API.md

## Required Patterns

### Controllers
- Inherit from ControllerBase
- Add [ApiController] and [Route] attributes
- Add [Authorize] unless public endpoint
- Inject services via constructor

### Services
- Create interface in Services/I*Service.cs
- Implement in Services/*Service.cs
- Register as Scoped in Program.cs
- Keep business logic here (not in controllers)

### Error Handling
- Return appropriate status codes
- Use ProblemDetails for errors
- Validate inputs in DTOs
```

## File Size Guidelines

Based on the article:
- **Main CLAUDE.md**: 50-100 lines (overview + imports)
- **Topic-specific rules**: 50-200 lines each
- **Be specific**: "Use 2-space indentation" not "Format code properly"
- **Use structure**: Headings, bullet points, clear organization

## Priority Files to Create

1. **High Priority** (create first):
   - `.claude/CLAUDE.md` (main overview)
   - `.claude/rules/workflows/ai-sub-agent.md` (critical workflow)
   - `.claude/rules/backend/api-conventions.md` (most common task)
   - `.claude/rules/frontend/web/react-patterns.md` (most common task)
   - `.claude/rules/testing.md` (critical requirement)

2. **Medium Priority**:
   - `.claude/rules/backend/architecture.md`
   - `.claude/rules/backend/database.md`
   - `.claude/rules/frontend/web/api-integration.md`
   - `.claude/rules/security.md`

3. **Lower Priority**:
   - `.claude/rules/commands.md`
   - `.claude/rules/frontend/mobile/react-native.md`
   - `.claude/rules/workflows/git.md`

## Rollback Plan

If issues arise:
1. Keep original CLAUDE.md as CLAUDE.md.backup
2. Test new structure in parallel before deleting old files
3. Use `/memory` command to verify what's loaded
4. Can revert by removing `.claude/` directory

## Success Criteria

- ✅ Main CLAUDE.md under 100 lines
- ✅ Each rule file focused on single topic
- ✅ Path-specific rules use YAML frontmatter
- ✅ No duplication between files
- ✅ All critical workflows documented
- ✅ Easy to find and update specific rules
- ✅ Reduced context size for Claude
