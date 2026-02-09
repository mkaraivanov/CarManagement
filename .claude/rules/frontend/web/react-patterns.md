---
agents: [plan, senior-software-engineer, regular-software-engineer, code-reviewer]
paths:
  - "web-frontend/src/**/*.jsx"
  - "web-frontend/src/**/*.tsx"
  - "web-frontend/src/components/**/*"
  - "web-frontend/src/pages/**/*"
---

# Web Frontend React Patterns

## Frontend Architecture

**Structure:**
```
src/
├── components/
│   ├── auth/          # ProtectedRoute component
│   └── layout/        # AppLayout, Navbar
├── pages/            # Page components
│   ├── vehicles/     # Vehicle-related pages (List, Details, VehicleForm)
│   ├── Dashboard.jsx
│   ├── Login.jsx
│   └── Register.jsx
├── services/         # API service layer
│   ├── api.js        # Axios instance with interceptors
│   ├── authService.js
│   └── vehicleService.js
├── context/
│   └── AuthContext.jsx   # Global auth state
└── utils/            # Utility functions
```

## Authentication Pattern

- `AuthContext` provides: `user`, `login`, `register`, `logout`, `isAuthenticated`, `loading`
- `ProtectedRoute` wrapper checks `isAuthenticated` before rendering routes
- Token stored in localStorage (key: `'token'`)
- On app mount, AuthContext fetches current user if token exists

## State Management

- React Context for auth state (no Redux/Zustand)
- Component-level state with useState for UI state
- React Hook Form + Yup for form validation

## Component Patterns

- Use functional components with hooks
- Extract reusable logic to custom hooks
- Keep components focused and small (single responsibility)
- Protected pages wrapped in `<ProtectedRoute>`

## Form Validation

- Forms must use React Hook Form + Yup for validation
- Show validation errors inline
- Disable submit button during submission

## Loading & Error States

- Show loading states for async operations
- Display user-friendly error messages using Material-UI Snackbar/Alert
- Clear error messages for users (not raw error objects)

## Common Mistakes

- ❌ Calling axios directly from components (use service layer)
- ❌ Not showing loading states during async operations
- ❌ Exposing technical errors to users
- ❌ Not using React Hook Form for complex forms
- ❌ Breaking the service layer pattern

## Code Organization

- Extract reusable logic into utility functions or custom hooks
- Keep components focused and small (single responsibility)
- Remove unused imports and dead code
- Group related files together
