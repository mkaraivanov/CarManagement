# Feature Design Documentation

**REQUIRED: All new major features MUST have a design document in `docs/features/` BEFORE implementation starts.**

## What Requires a Design Doc

- Features spanning multiple files (3+ files)
- Features requiring database schema changes
- Features involving new technologies/libraries
- Features with multiple implementation phases
- Any feature estimated at 2+ days of work

## Process

1. **Before coding:** Copy `docs/features/_TEMPLATE.md` to `docs/features/feature-name.md`
2. **Fill in all sections:** Context, technology decisions, API design, implementation phases, etc.
3. **Commit the design doc** before starting implementation
4. **Reference the doc** in commit messages during implementation
5. **Update status** as phases complete: 🔵 Planning → 🟡 In Progress → ⚠️ Backend Only → ✅ Complete
6. **Keep it current** - update the doc when requirements change

## Benefits

- Clearer thinking before coding
- Better time estimates
- Documentation for future reference
- AI assistants can follow the plan accurately
- Easier to pick up work after breaks

## Before Starting New Features

```bash
# Create feature design document
cp docs/features/_TEMPLATE.md docs/features/my-feature-name.md
# Fill in all sections before coding
# Commit the design doc first
git add docs/features/my-feature-name.md
git commit -m "Add design doc for [feature name]"
```

See @docs/README.md for complete guidelines and templates.
