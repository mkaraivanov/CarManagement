import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import BackButton from './BackButton';

// Mock useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

const renderWithRouter = (component) => {
  return render(<BrowserRouter>{component}</BrowserRouter>);
};

describe('BackButton', () => {
  beforeEach(() => {
    mockNavigate.mockClear();
  });

  it('renders with default props', () => {
    renderWithRouter(<BackButton />);
    const button = screen.getByRole('button', { name: /back/i });
    expect(button).toBeInTheDocument();
  });

  it('renders with custom label', () => {
    renderWithRouter(<BackButton label="Go Back" />);
    const button = screen.getByRole('button', { name: /go back/i });
    expect(button).toBeInTheDocument();
  });

  it('navigates back using default behavior (-1)', async () => {
    const user = userEvent.setup();
    renderWithRouter(<BackButton />);
    const button = screen.getByRole('button', { name: /back/i });

    await user.click(button);
    expect(mockNavigate).toHaveBeenCalledWith(-1);
  });

  it('navigates to custom route when to prop is a string', async () => {
    const user = userEvent.setup();
    renderWithRouter(<BackButton to="/vehicles" />);
    const button = screen.getByRole('button', { name: /back/i });

    await user.click(button);
    expect(mockNavigate).toHaveBeenCalledWith('/vehicles');
  });

  it('respects disabled prop', () => {
    renderWithRouter(<BackButton disabled={true} />);
    const button = screen.getByRole('button', { name: /back/i });
    expect(button).toBeDisabled();
  });

  it('supports different Material-UI variants', () => {
    const { rerender } = renderWithRouter(<BackButton variant="contained" />);
    let button = screen.getByRole('button', { name: /back/i });
    expect(button).toHaveClass('MuiButton-contained');

    rerender(
      <BrowserRouter>
        <BackButton variant="text" />
      </BrowserRouter>
    );
    button = screen.getByRole('button', { name: /back/i });
    expect(button).toHaveClass('MuiButton-text');
  });

  it('calls custom onClick handler when provided', async () => {
    const user = userEvent.setup();
    const customHandler = vi.fn();
    renderWithRouter(<BackButton onClick={customHandler} />);
    const button = screen.getByRole('button', { name: /back/i });

    await user.click(button);
    expect(customHandler).toHaveBeenCalled();
  });

  it('supports fullWidth prop', () => {
    renderWithRouter(<BackButton fullWidth={true} />);
    const button = screen.getByRole('button', { name: /back/i });
    expect(button).toHaveClass('MuiButton-fullWidth');
  });

  it('passes through additional props to Material-UI Button', () => {
    renderWithRouter(<BackButton className="custom-class" />);
    const button = screen.getByRole('button', { name: /back/i });
    expect(button).toHaveClass('custom-class');
  });

  it('renders with default ArrowBack icon', () => {
    renderWithRouter(<BackButton />);
    const button = screen.getByRole('button', { name: /back/i });
    const icon = button.querySelector('svg');
    expect(icon).toBeInTheDocument();
  });
});
