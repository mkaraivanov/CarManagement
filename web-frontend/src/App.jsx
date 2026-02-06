import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import VehicleList from './pages/vehicles/VehicleList';
import VehicleForm from './pages/vehicles/VehicleForm';
import VehicleDetails from './pages/vehicles/VehicleDetails';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              }
            />
            <Route
              path="/vehicles"
              element={
                <ProtectedRoute>
                  <VehicleList />
                </ProtectedRoute>
              }
            />
            <Route
              path="/vehicles/new"
              element={
                <ProtectedRoute>
                  <VehicleForm />
                </ProtectedRoute>
              }
            />
            <Route
              path="/vehicles/:id"
              element={
                <ProtectedRoute>
                  <VehicleDetails />
                </ProtectedRoute>
              }
            />
            <Route
              path="/vehicles/:id/edit"
              element={
                <ProtectedRoute>
                  <VehicleForm />
                </ProtectedRoute>
              }
            />
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </AuthProvider>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
