import { createTheme } from '@mui/material/styles';

// Automotive-themed color palette
const colors = {
  primary: {
    main: '#1565c0',      // Deep automotive blue
    light: '#5e92f3',
    dark: '#003c8f',
    contrastText: '#ffffff',
  },
  secondary: {
    main: '#ff6f00',      // Vibrant orange (replaces pink)
    light: '#ffa040',
    dark: '#c43e00',
    contrastText: '#ffffff',
  },
  success: {
    main: '#2e7d32',      // Forest green
    light: '#60ad5e',
    dark: '#005005',
  },
  warning: {
    main: '#ed6c02',      // Amber
    light: '#ff9800',
    dark: '#e65100',
  },
  error: {
    main: '#d32f2f',
    light: '#ef5350',
    dark: '#c62828',
  },
  info: {
    main: '#0288d1',
    light: '#03a9f4',
    dark: '#01579b',
  },
  grey: {
    50: '#fafafa',
    100: '#f5f5f5',
    200: '#eeeeee',
    300: '#e0e0e0',
    400: '#bdbdbd',
    500: '#9e9e9e',
    600: '#757575',
    700: '#616161',
    800: '#424242',
    900: '#212121',
  },
  background: {
    default: '#f5f7fa',   // Soft gray instead of white
    paper: '#ffffff',
  },
};

// Typography scale
const typography = {
  fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  h1: {
    fontSize: '2.5rem',
    fontWeight: 700,
    lineHeight: 1.2,
  },
  h2: {
    fontSize: '2rem',
    fontWeight: 700,
    lineHeight: 1.3,
  },
  h3: {
    fontSize: '1.75rem',
    fontWeight: 600,
    lineHeight: 1.3,
  },
  h4: {
    fontSize: '1.5rem',
    fontWeight: 600,
    lineHeight: 1.4,
  },
  h5: {
    fontSize: '1.25rem',
    fontWeight: 600,
    lineHeight: 1.4,
  },
  h6: {
    fontSize: '1rem',
    fontWeight: 600,
    lineHeight: 1.5,
  },
  button: {
    textTransform: 'none',  // Remove uppercase
    fontWeight: 600,
  },
};

// Softer, progressive shadows
const shadows = [
  'none',
  '0px 2px 4px rgba(0, 0, 0, 0.05)',
  '0px 4px 8px rgba(0, 0, 0, 0.08)',
  '0px 6px 12px rgba(0, 0, 0, 0.1)',
  '0px 8px 16px rgba(0, 0, 0, 0.12)',
  '0px 10px 20px rgba(0, 0, 0, 0.14)',
  '0px 12px 24px rgba(0, 0, 0, 0.16)',
  '0px 14px 28px rgba(0, 0, 0, 0.18)',
  '0px 16px 32px rgba(0, 0, 0, 0.2)',
  '0px 18px 36px rgba(0, 0, 0, 0.22)',
  '0px 20px 40px rgba(0, 0, 0, 0.24)',
  '0px 22px 44px rgba(0, 0, 0, 0.26)',
  '0px 24px 48px rgba(0, 0, 0, 0.28)',
  '0px 26px 52px rgba(0, 0, 0, 0.3)',
  '0px 28px 56px rgba(0, 0, 0, 0.32)',
  '0px 30px 60px rgba(0, 0, 0, 0.34)',
  '0px 32px 64px rgba(0, 0, 0, 0.36)',
  '0px 34px 68px rgba(0, 0, 0, 0.38)',
  '0px 36px 72px rgba(0, 0, 0, 0.4)',
  '0px 38px 76px rgba(0, 0, 0, 0.42)',
  '0px 40px 80px rgba(0, 0, 0, 0.44)',
  '0px 42px 84px rgba(0, 0, 0, 0.46)',
  '0px 44px 88px rgba(0, 0, 0, 0.48)',
  '0px 46px 92px rgba(0, 0, 0, 0.5)',
  '0px 48px 96px rgba(0, 0, 0, 0.52)',
];

// Shape/border radius
const shape = {
  borderRadius: 12,  // Softer, more modern
};

// Component default overrides
const components = {
  MuiButton: {
    styleOverrides: {
      root: {
        borderRadius: 8,
        padding: '10px 20px',
        boxShadow: 'none',
        transition: 'all 0.2s ease-in-out',
        '&:hover': {
          boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.15)',
        },
      },
      contained: {
        '&:hover': {
          transform: 'translateY(-2px)',
        },
      },
    },
  },
  MuiCard: {
    styleOverrides: {
      root: {
        borderRadius: 16,
        boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.08)',
        transition: 'all 0.3s ease-in-out',
        '&:hover': {
          boxShadow: '0px 8px 24px rgba(0, 0, 0, 0.12)',
        },
      },
    },
  },
  MuiChip: {
    styleOverrides: {
      root: {
        borderRadius: 8,
        fontWeight: 500,
      },
    },
  },
  MuiTextField: {
    styleOverrides: {
      root: {
        '& .MuiOutlinedInput-root': {
          borderRadius: 8,
        },
      },
    },
  },
  MuiAppBar: {
    styleOverrides: {
      root: {
        boxShadow: '0px 2px 8px rgba(0, 0, 0, 0.1)',
        background: 'linear-gradient(135deg, #1565c0 0%, #0d47a1 100%)',
      },
    },
  },
};

// Create and export the theme
const theme = createTheme({
  palette: colors,
  typography,
  shadows,
  shape,
  components,
});

export default theme;
