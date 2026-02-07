---
name: test-all
description: Runs all tests in the solution (backend + frontend)
disable-model-invocation: true
argument-hint: ""
---

Runs the complete test suite for the CarManagement solution by executing the test script.

## What This Does

1. **Backend Tests (xUnit)**: Runs 18 backend unit/integration tests
2. **Frontend Tests (Vitest)**: Runs 11 frontend component tests

## Expected Results

- **Total**: 29 tests should pass
- **Exit code**: Returns 0 if all tests pass, 1 if any fail

## Execution

Run the test script using the Bash tool:

```bash
bash /Users/martin.karaivanov/Projects/CarManagement/.claude/skills/test-all.sh
```

The script will:
- Use colored output to show test progress
- Run backend tests with `dotnet test --verbosity normal`
- Run frontend tests with `npm test -- --run`
- Report pass/fail status for each test suite
- Exit with code 0 only if both test suites pass

## Notes

- The script uses `set -e` to exit immediately on test failures
- Tests are run sequentially (backend first, then frontend)
- All paths are automatically resolved relative to the project root
- This should be run before committing changes (as per TESTING.md)
