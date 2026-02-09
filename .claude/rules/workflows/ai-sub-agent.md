# AI Sub-Agent Workflow

**🚨 CRITICAL: Before implementing ANY new feature, you MUST use the AI Sub-Agent Workflow!**

## When to Use This Workflow

**ALWAYS use for:**
- ✅ Adding new features (any size)
- ✅ Adding new API endpoints
- ✅ Adding/modifying database entities
- ✅ Significant refactoring
- ✅ Bug fixes affecting multiple files

**Only skip for:**
- ❌ Single-line typo fixes
- ❌ Documentation-only updates
- ❌ Obvious one-line bug fixes

## The 5-Phase Process

```
1. PLAN (background)           → Spawn plan agent
2. USER REVIEW                 → Get approval & complexity decision
3. IMPLEMENT (background)      → Spawn implementation agent
4. QA VALIDATION (background)  → AUTOMATICALLY spawn qa-engineer ⭐
5. CODE REVIEW (background)    → AUTOMATICALLY spawn code-reviewer ⭐
```

## 🚨 CRITICAL: Phases 4 & 5 are AUTOMATIC

**After implementation (Phase 3) completes:**
- **IMMEDIATELY spawn qa-engineer** (don't wait for user)
- After QA approves → **IMMEDIATELY spawn code-reviewer** (don't wait for user)
- These are NOT optional - MUST be executed for every feature/fix
- Only skip for: documentation-only, typos, config formatting

## Agent-Specific Documentation

Each agent has dedicated context documentation:

- **Phase 1 (Planning)**: See @agents/plan-agent.md
- **Phase 3 (Implementation)**: See @agents/implementation-agent.md
- **Phase 4 (QA)**: See @agents/qa-agent.md
- **Phase 5 (Code Review)**: See @agents/code-reviewer-agent.md

## Quick Phase Overview

### Phase 1: Plan Agent
- Explores codebase thoroughly
- Identifies all affected files
- Designs implementation approach
- Creates step-by-step plan

### Phase 2: User Review
User decides:
- Approve or request changes
- Choose: `regular-software-engineer` (simple) or `senior-software-engineer` (complex)

**Decision criteria:**
| Criteria | regular | senior |
|----------|---------|--------|
| Scope | Single/few files | Multiple files |
| Complexity | Straightforward | Architectural decisions |
| Risk | Low | High |

### Phase 3: Implementation Agent
- Follows the approved plan
- Matches existing patterns
- Writes clean, tested code
- Runs in background

**⚠️ After completion → AUTOMATICALLY spawn qa-engineer**

### Phase 4: QA Agent (Automatic)
- Verifies test coverage
- Runs all tests
- Assesses regression risk
- Validates error handling

**⚠️ After approval → AUTOMATICALLY spawn code-reviewer**

### Phase 5: Code Reviewer (Automatic)
- Reviews code quality
- Checks pattern adherence
- Identifies improvements
- Provides constructive feedback

## Example Flow

```
User: "Add endpoint to get vehicle statistics"

1. Spawn plan agent (background) → Designs approach
2. User reviews → "Looks good, use senior-software-engineer"
3. Spawn senior-software-engineer (background) → Implements
4. AUTOMATICALLY spawn qa-engineer → Validates tests ✅
5. AUTOMATICALLY spawn code-reviewer → Reviews code ✅
6. Feature complete!
```

## Why This Works

- **Background execution** - You can do other work
- **Structured review** - Clear checkpoints
- **Quality assurance** - Automatic QA + code review
- **Separation of concerns** - Planning ≠ implementation ≠ testing ≠ review

See @WORKFLOWS.md for complete workflow documentation.
