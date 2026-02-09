---
paths:
  - "web-frontend/**/*.test.{js,jsx,tsx}"
  - "web-frontend/**/*.spec.{js,jsx,tsx}"
---

# Frontend Testing

## Running Frontend Tests

```bash
cd web-frontend
npm test                                       # Run all frontend tests
npm run test:ui                               # Run with visual UI
npm run test:coverage                         # Run with coverage report
```

**Expected Results:**
- Frontend: 11 tests passing ✅

## Test Infrastructure

- **Vitest** testing framework
- **React Testing Library** for component testing
- Tests run in Node environment

## Current Test Coverage

**ExtractedDataReview Component Tests:**
- Component rendering and data display
- Confidence level handling
- User interactions

## Writing New Tests

Create `.test.jsx` file alongside component:

```javascript
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import MyComponent from './MyComponent';

describe('MyComponent', () => {
  it('renders correctly', () => {
    render(<MyComponent />);
    expect(screen.getByText('Expected Text')).toBeInTheDocument();
  });

  it('handles user interaction', async () => {
    const { user } = render(<MyComponent />);
    await user.click(screen.getByRole('button'));
    expect(screen.getByText('Result')).toBeInTheDocument();
  });
});
```

See @TESTING.md for complete testing documentation.
