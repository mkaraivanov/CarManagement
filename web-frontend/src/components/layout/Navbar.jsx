import { AppBar, Toolbar, Typography, Button, IconButton, Box } from '@mui/material';
import { DirectionsCar, Logout } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import NotificationCenter from '../maintenance/NotificationCenter';
import OverdueBadge from '../maintenance/OverdueBadge';

const Navbar = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <AppBar position="fixed">
      <Toolbar>
        <IconButton
          size="large"
          edge="start"
          color="inherit"
          sx={{ mr: 2 }}
          onClick={() => navigate('/dashboard')}
        >
          <DirectionsCar />
        </IconButton>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Car Management
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Button color="inherit" onClick={() => navigate('/dashboard')}>
            Dashboard
          </Button>
          <Button color="inherit" onClick={() => navigate('/vehicles')}>
            Vehicles
          </Button>
          <Button color="inherit" onClick={() => navigate('/maintenance')}>
            Maintenance
          </Button>
          <OverdueBadge />
          <NotificationCenter />
          <Typography variant="body2" sx={{ mx: 2 }}>
            {user?.username}
          </Typography>
          <IconButton color="inherit" onClick={handleLogout}>
            <Logout />
          </IconButton>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
