# Documentation Structure

This directory contains design documents, architecture decisions, and feature specifications for the CarManagement project.

## Directory Organization

```
docs/
├── README.md              # This file - documentation guide
├── features/              # Feature design documents
│   ├── _TEMPLATE.md      # Template for new feature docs
│   └── vehicle-registration-ocr.md
├── architecture/          # Architecture and design decisions
│   └── [architecture docs]
└── adr/                  # Architecture Decision Records (ADR)
    └── [ADR files]
```

## When to Create Documentation

### Feature Design Documents (`docs/features/`)

**REQUIRED for:**
- New major features that span multiple files
- Features requiring database schema changes
- Features with multiple implementation phases
- Features involving new technology/libraries
- Any feature estimated at 2+ days of work

**Create BEFORE implementation:**
1. Copy `_TEMPLATE.md` to a new file: `feature-name.md`
2. Fill in all sections (context, design, phases, etc.)
3. Review and get approval if working in a team
4. Reference the doc in commit messages during implementation
5. Update status as you complete phases

**Benefits:**
- Clearer thinking before coding
- Better estimation of work
- Documentation for future maintainers
- Easier to review and validate approach
- AI assistants (like Claude) can follow the plan accurately

### Architecture Decision Records (`docs/adr/`)

**REQUIRED for:**
- Choosing between technologies (e.g., Tesseract vs Azure OCR)
- Selecting architectural patterns (e.g., service layer, repository pattern)
- Database design decisions (e.g., SQLite vs PostgreSQL)
- Security decisions (e.g., JWT vs session cookies)

**Format:**
```
001-title-of-decision.md
002-another-decision.md
```

**Template:**
```markdown
# ADR-XXX: [Decision Title]

**Status:** Proposed / Accepted / Deprecated / Superseded by ADR-YYY

**Date:** YYYY-MM-DD

**Context:**
What is the issue we're facing?

**Decision:**
What did we decide to do?

**Consequences:**
What are the trade-offs?
- Positive: ...
- Negative: ...
- Risks: ...
```

### Architecture Documents (`docs/architecture/`)

**REQUIRED for:**
- System-wide architectural patterns
- Database schema overview
- API design principles
- Authentication/authorization flows
- Deployment architecture

**When to create:**
- At project start (initial architecture)
- When making major architectural changes
- When onboarding new team members

## Feature Documentation Workflow

### 1. Before Starting a Feature

```bash
# 1. Create feature branch (MANDATORY - see WORKFLOWS.md)
git checkout main
git pull origin main
git checkout -b feature/my-new-feature

# 2. Copy template
cp docs/features/_TEMPLATE.md docs/features/my-new-feature.md

# 3. Edit the new file with your feature design
# Fill in all sections: Context, Technology Decisions, Implementation Phases, etc.

# 4. Commit the design doc to your feature branch
git add docs/features/my-new-feature.md
git commit -m "Add design doc for [feature name]"
git push origin feature/my-new-feature
```

### 2. During Implementation

- Work on your feature branch (never on `main`)
- Update the design doc as you discover new requirements
- Check off completed tasks in the Implementation Phases section
- Document any deviations from the original plan
- Commit frequently: `git push origin feature/my-new-feature`

### 3. After Completion

- Update status to ✅ Complete
- Add any "Lessons Learned" section if needed
- Update related docs (API.md, CLAUDE.md, README.md)
- Merge feature branch to main (via PR or local merge)
- Delete feature branch after merging

## Integration with CLAUDE.md

**CLAUDE.md** is the AI assistant's guide to:
- Development workflows and best practices
- **MANDATORY feature branch workflow** (all work on feature branches, never on `main`)
- Coding standards and patterns
- Common mistakes to avoid
- Git workflow and commit practices

**Feature docs** are detailed implementation plans for specific features.

**Relationship:**
- CLAUDE.md references this docs structure and mandates feature branch workflow
- Feature docs follow patterns described in CLAUDE.md
- CLAUDE.md stays focused on "how to work" (workflows)
- Feature docs focus on "what to build" (specifications)
- Both emphasize working on feature branches, never directly on `main`

## Best Practices

### For Solo Developers (Current Setup)

Even working solo, feature docs help you:
- Think through the design before coding
- Remember why you made certain decisions 6 months later
- Provide context for AI assistants like Claude
- Avoid scope creep by defining what's in/out of scope

### Writing Good Feature Docs

**Do:**
- ✅ Write in clear, simple language
- ✅ Include code examples and API schemas
- ✅ Define success criteria upfront
- ✅ Break down into phases with clear verification steps
- ✅ Update the doc as implementation progresses

**Don't:**
- ❌ Write docs and never update them (stale docs are worse than no docs)
- ❌ Make docs too abstract or high-level (be specific)
- ❌ Skip the "why" (always explain reasoning for decisions)
- ❌ Forget to mark incomplete features clearly

## Review Checklist

Before considering a feature "complete," verify:

- [ ] Feature design doc exists and is up-to-date
- [ ] All implementation phases marked complete
- [ ] Backend API implemented and tested
- [ ] Frontend UI implemented and tested
- [ ] Integration tests passing
- [ ] `backend/API.md` updated with new endpoints
- [ ] `CLAUDE.md` updated if patterns changed
- [ ] Status changed from "⚠️ Backend Only" to "✅ Complete"

---

**Remember:** Good documentation is an investment in your future productivity. Take the time to write it well!
