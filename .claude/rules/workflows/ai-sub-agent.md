# AI Sub-Agent Workflow

**🚨 CRITICAL: Before implementing ANY new feature, you MUST use the AI Sub-Agent Workflow!**

## When to Use This Workflow

**ALWAYS use for:**
- ✅ Adding new features (any size)
- ✅ Adding new API endpoints
- ✅ Adding/modifying database entities
- ✅ Significant refactoring
- ✅ Bug fixes affecting multiple files
- ✅ Any work requiring architectural decisions

**Only skip for:**
- ❌ Single-line typo fixes
- ❌ Documentation-only updates
- ❌ Obvious one-line bug fixes

## The 5-Phase Process

```
1. PLAN (background)     → Spawn Plan sub-agent to design approach
2. USER REVIEW           → Present plan, get approval & complexity decision
3. IMPLEMENT (background) → Spawn senior-software-engineer OR regular-software-engineer
4. QA VALIDATION (background) → AUTOMATICALLY spawn qa-engineer (mandatory)
5. CODE REVIEW (background)   → AUTOMATICALLY spawn code-reviewer (mandatory)
```

## 🚨 CRITICAL: Phases 4 & 5 are AUTOMATIC

**After implementation (Phase 3) completes:**
- **IMMEDIATELY spawn qa-engineer** (don't wait for user)
- After QA approves → **IMMEDIATELY spawn code-reviewer** (don't wait for user)
- These are NOT optional - they MUST be executed for every feature/fix
- Only skip for: documentation-only, typos, config formatting

## Why This Matters

- Runs in background (you can do other work)
- Provides structured review points
- Ensures quality through dedicated QA validation and code review
- Separates planning from implementation
- **QA and Code Review phases are AUTOMATIC** - no need to request them

## Workflow Details

### Phase 1: Planning (Plan Sub-Agent)

The planning sub-agent runs in the background to:
- Thoroughly explore the codebase using search tools
- Understand existing patterns and architecture
- Identify all files that need to be created or modified
- Design the implementation approach
- Create a step-by-step implementation plan

**Output:** A detailed implementation plan for user review.

### Phase 2: User Review

After the plan is ready, the user:
- Reviews the proposed approach
- Provides feedback or requests changes
- Approves the plan to proceed
- Decides: simple task (regular-software-engineer) or complex task (senior-software-engineer)

**Decision criteria:**
| Criteria | regular-software-engineer | senior-software-engineer |
|----------|---------------------|-----------------|
| Scope | Single file or few files | Multiple files, cross-cutting |
| Complexity | Straightforward changes | Architectural decisions needed |
| Risk | Low risk of regressions | Higher risk, needs careful handling |
| Patterns | Following existing patterns exactly | May need new patterns or abstractions |

### Phase 3: Implementation

**For Complex Tasks (senior-software-engineer):**
- Implements production-quality code
- Makes architectural decisions when needed
- Handles edge cases and error conditions
- Ensures proper testing coverage

**For Simple Tasks (regular-software-engineer):**
- Quick, focused implementation
- Follows established patterns exactly
- Makes incremental, targeted changes

Both run in background.

**⚠️ IMPORTANT**: After implementation completes, Claude must AUTOMATICALLY spawn qa-engineer (Phase 4).

### Phase 4: QA Validation (qa-engineer)

**Runs automatically after implementation.**

**What the QA engineer does:**
- Verifies test coverage
- Identifies missing tests
- Runs all relevant tests
- Assesses regression risk
- Validates error handling
- Thinks like a user

**Why QA comes first:**
- Catches functional issues early
- Ensures tests pass before code review
- Code reviewer doesn't waste time reviewing broken code

**⚠️ IMPORTANT**: After QA approves, Claude must AUTOMATICALLY spawn code-reviewer (Phase 5).

### Phase 5: Code Review (code-reviewer)

**Runs automatically after QA approval.**

**What the code reviewer does:**
- Reviews code quality and readability
- Checks adherence to project patterns
- Identifies code smells
- Suggests improvements
- Provides constructive feedback

**Why code review comes after QA:**
- No point reviewing code that doesn't work
- QA validated functionality first
- Focus purely on code quality

## Example Usage

```
User: "Add endpoint to get vehicle statistics"

1. Claude spawns Plan sub-agent (background)
2. User reviews plan → "Use senior-software-engineer"
3. Claude spawns senior-software-engineer (background)
4. Implementation completes
5. Claude AUTOMATICALLY spawns qa-engineer ⭐
6. QA approves
7. Claude AUTOMATICALLY spawns code-reviewer ⭐
8. Feature complete
```

See @WORKFLOWS.md for complete workflow documentation.
