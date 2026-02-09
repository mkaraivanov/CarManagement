import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  Alert,
} from '@mui/material';
import { Edit, Delete, Person } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import AppLayout from '../../components/layout/AppLayout';
import userService from '../../services/userService';
import { useAuth } from '../../context/AuthContext';
import { TableSkeleton } from '../../components/common/LoadingSkeleton';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import PageTransition from '../../components/common/PageTransition';
import EmptyState from '../../components/common/EmptyState';
import BackButton from '../../components/common/BackButton';

const UserList = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [confirmDelete, setConfirmDelete] = useState({ open: false, userId: null });
  const { user: currentUser } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await userService.getAll();
      setUsers(data);
    } catch (err) {
      console.error('Error fetching users:', err);

      // Provide more specific error messages
      if (err.response) {
        // The request was made and the server responded with a status code
        if (err.response.status === 401) {
          setError('Session expired. Please log in again.');
        } else if (err.response.status === 403) {
          setError('You do not have permission to view users.');
        } else if (err.response.status === 404) {
          setError('Users endpoint not found. Please contact support.');
        } else if (err.response.status >= 500) {
          setError('Server error. Please try again later.');
        } else {
          setError(err.response?.data?.message || 'Failed to load users');
        }
      } else if (err.request) {
        // The request was made but no response was received
        setError('Cannot connect to server. Please check if the backend is running.');
      } else {
        // Something happened in setting up the request
        setError('An unexpected error occurred. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteClick = (id) => {
    setConfirmDelete({ open: true, userId: id });
  };

  const handleDeleteConfirm = async () => {
    try {
      await userService.delete(confirmDelete.userId);
      setUsers(users.filter((u) => u.id !== confirmDelete.userId));
      setConfirmDelete({ open: false, userId: null });
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete user');
      setConfirmDelete({ open: false, userId: null });
    }
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  if (loading) {
    return (
      <AppLayout>
        <Box>
          <Box sx={{ mb: 3 }}>
            <Typography variant="h4">User Management</Typography>
          </Box>
          <TableSkeleton />
        </Box>
      </AppLayout>
    );
  }

  return (
    <AppLayout>
      <PageTransition>
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
            <BackButton to="/dashboard" variant="text" label="" />
            <Typography variant="h4">User Management</Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
              {error}
            </Alert>
          )}

          {users.length === 0 ? (
            <Card>
              <EmptyState
                icon={<Person />}
                title="No users found"
                description="There are no users in the system"
              />
            </Card>
          ) : (
            <TableContainer component={Card}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Username</TableCell>
                    <TableCell>Email</TableCell>
                    <TableCell>Created</TableCell>
                    <TableCell align="center">Vehicles</TableCell>
                    <TableCell align="center">Service Records</TableCell>
                    <TableCell align="center">Fuel Records</TableCell>
                    <TableCell align="right">Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {users.map((user) => (
                    <TableRow key={user.id} hover>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="body1" fontWeight="medium">
                            {user.username}
                          </Typography>
                          {currentUser?.id === user.id && (
                            <Chip label="You" size="small" color="primary" />
                          )}
                        </Box>
                      </TableCell>
                      <TableCell>{user.email}</TableCell>
                      <TableCell>{formatDate(user.createdAt)}</TableCell>
                      <TableCell align="center">
                        <Chip
                          label={user.statistics.vehicleCount}
                          size="small"
                          color={user.statistics.vehicleCount > 0 ? 'primary' : 'default'}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={user.statistics.serviceRecordCount}
                          size="small"
                          color={user.statistics.serviceRecordCount > 0 ? 'secondary' : 'default'}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={user.statistics.fuelRecordCount}
                          size="small"
                          color={user.statistics.fuelRecordCount > 0 ? 'success' : 'default'}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/users/${user.id}/edit`)}
                        >
                          <Edit />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          disabled={currentUser?.id === user.id}
                          onClick={() => handleDeleteClick(user.id)}
                        >
                          <Delete />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}

          <ConfirmDialog
            open={confirmDelete.open}
            onClose={() => setConfirmDelete({ open: false, userId: null })}
            onConfirm={handleDeleteConfirm}
            title="Delete User"
            message="Are you sure you want to delete this user? This will permanently delete all their vehicles, service records, and fuel records. This action cannot be undone."
            confirmText="Delete"
            severity="error"
          />
        </Box>
      </PageTransition>
    </AppLayout>
  );
};

export default UserList;
