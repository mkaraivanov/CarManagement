# Claude Skills for CarManagement

This directory contains custom Claude Code skills for the CarManagement project.

## Available Skills

### `/build-all`
**Description**: Builds all projects in the solution (backend + frontend)

**What it does**:
- Builds the ASP.NET Core backend in Release mode
- Builds the React frontend for production
- Shows colored output for build status
- Reports build locations

**Usage**:
```bash
/build-all
```

**Output locations**:
- Backend: `backend/bin/Release/net9.0/`
- Frontend: `web-frontend/dist/`

---

### `/test-all`
**Description**: Runs all tests in the solution

**What it does**:
- Runs backend tests (xUnit) with normal verbosity
- Runs frontend tests (Vitest) in run mode
- Shows pass/fail status for each test suite
- Exits with error code if any tests fail

**Usage**:
```bash
/test-all
```

**Expected results**:
- Backend: 18 tests passing
- Frontend: 11 tests passing
- **Total: 29 tests**

---

## How to Use Skills

1. **In Claude Code CLI**: Simply type the skill name with a forward slash:
   ```
   /build-all
   /test-all
   ```

2. **Manually**: You can also run the scripts directly:
   ```bash
   ./.claude/skills/build-all.sh
   ./.claude/skills/test-all.sh
   ```

## Adding New Skills

To add a new skill:

1. Create a new `.sh` file in this directory
2. Make it executable: `chmod +x skill-name.sh`
3. Add a header comment with:
   - `# Claude Skill: [Name]`
   - `# Description: [What it does]`
   - `# Usage: /skill-name`
4. Add it to this README

## Skill Best Practices

- Use `set -e` to exit on errors
- Provide colored output for better readability
- Show progress indicators for multi-step operations
- Return meaningful exit codes (0 = success, 1 = failure)
- Include helpful summary messages
- Make paths relative to project root

## Color Codes

Skills use ANSI color codes for output:
- **Blue**: Headers and info
- **Yellow**: Progress/in-progress
- **Green**: Success
- **Red**: Errors/failures
- **NC (No Color)**: Reset

## Testing Your Skills

Before committing a new skill, test it:

1. Run it manually: `./.claude/skills/your-skill.sh`
2. Verify it works from different directories
3. Check error handling with intentional failures
4. Ensure it provides clear output
