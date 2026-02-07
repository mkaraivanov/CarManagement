import { useState, useEffect } from 'react';
import {
  IconButton,
  Badge,
  Menu,
  MenuItem,
  Box,
  Typography,
  Divider,
  Button,
  List,
  ListItem,
  ListItemText,
  Chip,
} from '@mui/material';
import { Notifications, CheckCircle, Delete } from '@mui/icons-material';
import { formatDistanceToNow, parseISO } from 'date-fns';
import notificationService from '../../services/notificationService';

const NotificationCenter = () => {
  const [anchorEl, setAnchorEl] = useState(null);
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchNotificationCount();
    const interval = setInterval(fetchNotificationCount, 60000); // Update every minute
    return () => clearInterval(interval);
  }, []);

  const fetchNotificationCount = async () => {
    try {
      const data = await notificationService.getCount();
      setUnreadCount(data.unreadCount || 0);
    } catch (err) {
      console.error('Error fetching notification count:', err);
    }
  };

  const fetchNotifications = async () => {
    try {
      setLoading(true);
      const data = await notificationService.getAll(20);
      setNotifications(data);
    } catch (err) {
      console.error('Error fetching notifications:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleOpen = async (event) => {
    setAnchorEl(event.currentTarget);
    await fetchNotifications();
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleMarkAsRead = async (notificationId) => {
    try {
      await notificationService.markAsRead(notificationId);
      await fetchNotifications();
      await fetchNotificationCount();
    } catch (err) {
      console.error('Error marking notification as read:', err);
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      await notificationService.markAllAsRead();
      await fetchNotifications();
      await fetchNotificationCount();
    } catch (err) {
      console.error('Error marking all as read:', err);
    }
  };

  const handleDelete = async (notificationId) => {
    try {
      await notificationService.delete(notificationId);
      await fetchNotifications();
      await fetchNotificationCount();
    } catch (err) {
      console.error('Error deleting notification:', err);
    }
  };

  const getNotificationColor = (type) => {
    switch (type) {
      case 'MaintenanceOverdue':
        return 'error';
      case 'MaintenanceDue':
        return 'warning';
      case 'ServiceCompleted':
        return 'success';
      default:
        return 'default';
    }
  };

  const open = Boolean(anchorEl);

  return (
    <>
      <IconButton color="inherit" onClick={handleOpen}>
        <Badge badgeContent={unreadCount} color="error">
          <Notifications />
        </Badge>
      </IconButton>

      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        PaperProps={{
          sx: {
            width: 400,
            maxHeight: 500,
          },
        }}
      >
        <Box sx={{ p: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">Notifications</Typography>
          {unreadCount > 0 && (
            <Button size="small" onClick={handleMarkAllAsRead}>
              Mark all read
            </Button>
          )}
        </Box>
        <Divider />

        {loading ? (
          <Box sx={{ p: 2, textAlign: 'center' }}>
            <Typography variant="body2" color="text.secondary">
              Loading...
            </Typography>
          </Box>
        ) : notifications.length === 0 ? (
          <Box sx={{ p: 2, textAlign: 'center' }}>
            <Typography variant="body2" color="text.secondary">
              No notifications
            </Typography>
          </Box>
        ) : (
          <List sx={{ p: 0, maxHeight: 400, overflow: 'auto' }}>
            {notifications.map((notification) => (
              <ListItem
                key={notification.id}
                sx={{
                  borderLeft: 3,
                  borderLeftColor: notification.readAt ? 'transparent' : 'primary.main',
                  bgcolor: notification.readAt ? 'background.paper' : 'action.hover',
                  '&:hover': {
                    bgcolor: 'action.selected',
                  },
                }}
              >
                <ListItemText
                  primary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
                      <Typography variant="subtitle2">{notification.title}</Typography>
                      <Chip
                        label={notification.type}
                        size="small"
                        color={getNotificationColor(notification.type)}
                        sx={{ fontSize: '0.65rem', height: 20 }}
                      />
                    </Box>
                  }
                  secondary={
                    <Box>
                      <Typography variant="body2" sx={{ mb: 0.5 }}>
                        {notification.message}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {notification.createdAt
                          ? formatDistanceToNow(parseISO(notification.createdAt), { addSuffix: true })
                          : 'Unknown time'}
                      </Typography>
                    </Box>
                  }
                />
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                  {!notification.readAt && (
                    <IconButton
                      size="small"
                      onClick={() => handleMarkAsRead(notification.id)}
                      title="Mark as read"
                    >
                      <CheckCircle fontSize="small" />
                    </IconButton>
                  )}
                  <IconButton
                    size="small"
                    onClick={() => handleDelete(notification.id)}
                    title="Delete"
                  >
                    <Delete fontSize="small" />
                  </IconButton>
                </Box>
              </ListItem>
            ))}
          </List>
        )}
      </Menu>
    </>
  );
};

export default NotificationCenter;
