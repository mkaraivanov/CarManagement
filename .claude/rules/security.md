---
agents: [senior-software-engineer, regular-software-engineer, code-reviewer]
paths:
  - "**/*.cs"
  - "**/*.jsx"
  - "**/*.tsx"
---

# Security Best Practices

## Authentication & Authorization

- JWT secret must be 32+ characters (already configured correctly)
- All endpoints except `/api/auth/register` and `/api/auth/login` need `[Authorize]` attribute
- Validate all user inputs in DTOs (backend) and forms (frontend)
- Never commit secrets, API keys, or production connection strings
- Use different JWT secrets for development and production

## Token Handling

- Tokens stored in localStorage (key: `'token'`)
- Token automatically included in requests via axios interceptor
- 401 responses automatically clear token and redirect to login
- Token expiration: 1 hour (configurable in `appsettings.json`)

## Input Validation

- Use Data Annotations in DTOs for backend validation
- Use React Hook Form + Yup for frontend validation
- Never trust client-side data
- Sanitize all user inputs

## Common Security Mistakes

- ❌ Forgetting `[Authorize]` attribute on protected endpoints
- ❌ Committing secrets or API keys
- ❌ Not validating user inputs
- ❌ Exposing sensitive data in error messages
- ❌ Using weak JWT secrets
