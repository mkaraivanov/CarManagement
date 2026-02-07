import { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
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
import { Add, Edit, Delete, Visibility, DirectionsCar } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import AppLayout from '../../components/layout/AppLayout';
import vehicleService from '../../services/vehicleService';
import { TableSkeleton } from '../../components/common/LoadingSkeleton';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import PageTransition from '../../components/common/PageTransition';
import EmptyState from '../../components/common/EmptyState';

const VehicleList = () => {
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [confirmDelete, setConfirmDelete] = useState({ open: false, vehicleId: null });
  const navigate = useNavigate();

  useEffect(() => {
    fetchVehicles();
  }, []);

  const fetchVehicles = async () => {
    try {
      setLoading(true);
      const data = await vehicleService.getAll();
      setVehicles(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load vehicles');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteClick = (id) => {
    setConfirmDelete({ open: true, vehicleId: id });
  };

  const handleDeleteConfirm = async () => {
    try {
      await vehicleService.delete(confirmDelete.vehicleId);
      setVehicles(vehicles.filter((v) => v.id !== confirmDelete.vehicleId));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete vehicle');
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 0: // Active
        return 'success';
      case 1: // Sold
        return 'warning';
      case 2: // Inactive
        return 'default';
      default:
        return 'default';
    }
  };

  const getStatusLabel = (status) => {
    switch (status) {
      case 0:
        return 'Active';
      case 1:
        return 'Sold';
      case 2:
        return 'Inactive';
      default:
        return 'Unknown';
    }
  };

  if (loading) {
    return (
      <AppLayout>
        <Box>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
            <Typography variant="h4">My Vehicles</Typography>
            <Button
              variant="contained"
              startIcon={<Add />}
              onClick={() => navigate('/vehicles/new')}
            >
              Add Vehicle
            </Button>
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
          <Typography variant="h4">My Vehicles</Typography>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => navigate('/vehicles/new')}
          >
            Add Vehicle
          </Button>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        {vehicles.length === 0 ? (
          <Card>
            <CardContent>
              <EmptyState
                icon={<DirectionsCar />}
                title="No vehicles yet"
                description="Add your first vehicle to get started"
                actionLabel="Add Vehicle"
                onAction={() => navigate('/vehicles/new')}
              />
            </CardContent>
          </Card>
        ) : (
          <TableContainer component={Card}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Make & Model</TableCell>
                  <TableCell>Year</TableCell>
                  <TableCell>License Plate</TableCell>
                  <TableCell>Mileage</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {vehicles.map((vehicle) => (
                  <TableRow key={vehicle.id} hover>
                    <TableCell>
                      <Typography variant="body1" fontWeight="medium">
                        {vehicle.make} {vehicle.model}
                      </Typography>
                    </TableCell>
                    <TableCell>{vehicle.year}</TableCell>
                    <TableCell>{vehicle.licensePlate}</TableCell>
                    <TableCell>{vehicle.currentMileage.toLocaleString()} mi</TableCell>
                    <TableCell>
                      <Chip
                        label={getStatusLabel(vehicle.status)}
                        color={getStatusColor(vehicle.status)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="right">
                      <IconButton
                        size="small"
                        onClick={() => navigate(`/vehicles/${vehicle.id}`)}
                      >
                        <Visibility />
                      </IconButton>
                      <IconButton
                        size="small"
                        onClick={() => navigate(`/vehicles/${vehicle.id}/edit`)}
                      >
                        <Edit />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDeleteClick(vehicle.id)}
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
          onClose={() => setConfirmDelete({ open: false, vehicleId: null })}
          onConfirm={handleDeleteConfirm}
          title="Delete Vehicle"
          message="Are you sure you want to delete this vehicle? This action cannot be undone."
          confirmText="Delete"
          severity="error"
        />
        </Box>
      </PageTransition>
    </AppLayout>
  );
};

export default VehicleList;
