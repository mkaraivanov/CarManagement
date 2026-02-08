import { useState } from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  IconButton,
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import {
  DirectionsCar,
  Logout,
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  Build,
  Close,
  People,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import NotificationCenter from '../maintenance/NotificationCenter';
import OverdueBadge from '../maintenance/OverdueBadge';

const Navbar = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [mobileOpen, setMobileOpen] = useState(false);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const menuItems = [
    { label: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
    { label: 'Vehicles', icon: <DirectionsCar />, path: '/vehicles' },
    { label: 'Maintenance', icon: <Build />, path: '/maintenance' },
    { label: 'Users', icon: <People />, path: '/users' },
  ];

  const drawer = (
    <Box sx={{ width: 280 }}>
      <Box
        sx={{
          p: 2,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <Typography variant="h6">Car Management</Typography>
        <IconButton onClick={() => setMobileOpen(false)}>
          <Close />
        </IconButton>
      </Box>
      <List>
        {menuItems.map((item) => (
          <ListItemButton
            key={item.path}
            onClick={() => {
              navigate(item.path);
              setMobileOpen(false);
            }}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText primary={item.label} />
          </ListItemButton>
        ))}
      </List>
    </Box>
  );

  return (
    <>
      <AppBar position="fixed">
        <Toolbar>
          {isMobile && (
            <IconButton
              color="inherit"
              edge="start"
              onClick={() => setMobileOpen(true)}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}
          <IconButton
            size="large"
            edge={isMobile ? false : 'start'}
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
            <Box sx={{ display: { xs: 'none', md: 'flex' }, gap: 2 }}>
              <Button color="inherit" onClick={() => navigate('/dashboard')}>
                Dashboard
              </Button>
              <Button color="inherit" onClick={() => navigate('/vehicles')}>
                Vehicles
              </Button>
              <Button color="inherit" onClick={() => navigate('/maintenance')}>
                Maintenance
              </Button>
              <Button color="inherit" onClick={() => navigate('/users')}>
                Users
              </Button>
            </Box>
            <OverdueBadge />
            <NotificationCenter />
            <Typography variant="body2" sx={{ mx: 2, display: { xs: 'none', sm: 'block' } }}>
              {user?.username}
            </Typography>
            <IconButton color="inherit" onClick={handleLogout}>
              <Logout />
            </IconButton>
          </Box>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={() => setMobileOpen(false)}
        ModalProps={{ keepMounted: true }}
      >
        {drawer}
      </Drawer>
    </>
  );
};

export default Navbar;
