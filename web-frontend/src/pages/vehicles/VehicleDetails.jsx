import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Grid,
  Chip,
  Button,
  Tabs,
  Tab,
  CircularProgress,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
} from '@mui/material';
import { Edit, Delete, ArrowBack, Add } from '@mui/icons-material';
import AppLayout from '../../components/layout/AppLayout';
import vehicleService from '../../services/vehicleService';
import serviceRecordService from '../../services/serviceRecordService';
import fuelRecordService from '../../services/fuelRecordService';

const VehicleDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [vehicle, setVehicle] = useState(null);
  const [services, setServices] = useState([]);
  const [fuelRecords, setFuelRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);

  // Modal states
  const [serviceModalOpen, setServiceModalOpen] = useState(false);
  const [fuelModalOpen, setFuelModalOpen] = useState(false);
  const [saving, setSaving] = useState(false);

  // Service form state
  const [serviceForm, setServiceForm] = useState({
    serviceDate: new Date().toISOString().split('T')[0],
    mileageAtService: 0,
    serviceType: 0,
    serviceCenter: '',
    description: '',
    cost: 0,
    nextServiceDue: null,
    nextServiceMileage: null,
  });

  // Fuel form state
  const [fuelForm, setFuelForm] = useState({
    refuelDate: new Date().toISOString().split('T')[0],
    mileage: 0,
    quantity: 0,
    pricePerUnit: 0,
    fuelType: 0,
    gasStation: '',
    notes: '',
  });

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [vehicleData, servicesData, fuelData] = await Promise.all([
        vehicleService.getById(id),
        serviceRecordService.getByVehicle(id),
        fuelRecordService.getByVehicle(id),
      ]);
      setVehicle(vehicleData);
      setServices(servicesData);
      setFuelRecords(fuelData);

      // Set default mileage for forms
      setServiceForm(prev => ({ ...prev, mileageAtService: vehicleData.currentMileage }));
      setFuelForm(prev => ({ ...prev, mileage: vehicleData.currentMileage }));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load vehicle details');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this vehicle?')) {
      return;
    }

    try {
      await vehicleService.delete(id);
      navigate('/vehicles');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete vehicle');
    }
  };

  const handleServiceSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      // Clean up the form data - remove empty strings for nullable fields
      const serviceData = {
        serviceDate: serviceForm.serviceDate,
        mileageAtService: serviceForm.mileageAtService,
        serviceType: serviceForm.serviceType,
        serviceCenter: serviceForm.serviceCenter,
        description: serviceForm.description || '',
        cost: serviceForm.cost,
        nextServiceDue: serviceForm.nextServiceDue || null,
        nextServiceMileage: serviceForm.nextServiceMileage || null,
      };
      console.log('Submitting service form:', serviceData);
      await serviceRecordService.create(id, serviceData);
      setServiceModalOpen(false);
      setServiceForm({
        serviceDate: new Date().toISOString().split('T')[0],
        mileageAtService: vehicle.currentMileage,
        serviceType: 0,
        serviceCenter: '',
        description: '',
        cost: 0,
        nextServiceDue: null,
        nextServiceMileage: null,
      });
      await fetchData(); // Refresh data
    } catch (err) {
      console.error('Service record creation error:', err);
      console.error('Error response:', err.response?.data);
      setError(err.response?.data?.message || 'Failed to create service record');
    } finally {
      setSaving(false);
    }
  };

  const handleFuelSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await fuelRecordService.create(id, fuelForm);
      setFuelModalOpen(false);
      setFuelForm({
        refuelDate: new Date().toISOString().split('T')[0],
        mileage: vehicle.currentMileage,
        quantity: 0,
        pricePerUnit: 0,
        fuelType: 0,
        gasStation: '',
        notes: '',
      });
      await fetchData(); // Refresh data
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create fuel record');
    } finally {
      setSaving(false);
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 0:
        return 'success';
      case 1:
        return 'warning';
      case 2:
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

  const getServiceTypeLabel = (type) => {
    const types = ['Oil Change', 'Tire Rotation', 'Brake Service', 'Inspection', 'General', 'Other'];
    return types[type] || 'Unknown';
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return (
      <AppLayout>
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  if (!vehicle) {
    return (
      <AppLayout>
        <Alert severity="error">Vehicle not found</Alert>
      </AppLayout>
    );
  }

  return (
    <AppLayout>
      <Box>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
          <Button startIcon={<ArrowBack />} onClick={() => navigate('/vehicles')}>
            Back to Vehicles
          </Button>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        {/* Vehicle Info Card */}
        <Card sx={{ mb: 3 }}>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
              <Typography variant="h4">
                {vehicle.make} {vehicle.model}
              </Typography>
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button
                  variant="outlined"
                  startIcon={<Edit />}
                  onClick={() => navigate(`/vehicles/${id}/edit`)}
                >
                  Edit
                </Button>
                <Button
                  variant="outlined"
                  color="error"
                  startIcon={<Delete />}
                  onClick={handleDelete}
                >
                  Delete
                </Button>
              </Box>
            </Box>

            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="body2" color="text.secondary">
                  Year
                </Typography>
                <Typography variant="body1" mb={2}>
                  {vehicle.year}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                  License Plate
                </Typography>
                <Typography variant="body1" mb={2}>
                  {vehicle.licensePlate}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                  Current Mileage
                </Typography>
                <Typography variant="body1" mb={2}>
                  {vehicle.currentMileage.toLocaleString()} mi
                </Typography>
              </Grid>
              <Grid item xs={12} md={6}>
                {vehicle.vin && (
                  <>
                    <Typography variant="body2" color="text.secondary">
                      VIN
                    </Typography>
                    <Typography variant="body1" mb={2}>
                      {vehicle.vin}
                    </Typography>
                  </>
                )}

                {vehicle.color && (
                  <>
                    <Typography variant="body2" color="text.secondary">
                      Color
                    </Typography>
                    <Typography variant="body1" mb={2}>
                      {vehicle.color}
                    </Typography>
                  </>
                )}

                <Typography variant="body2" color="text.secondary">
                  Status
                </Typography>
                <Box mb={2}>
                  <Chip
                    label={getStatusLabel(vehicle.status)}
                    color={getStatusColor(vehicle.status)}
                    size="small"
                  />
                </Box>
              </Grid>
            </Grid>
          </CardContent>
        </Card>

        {/* Tabs for Service and Fuel Records */}
        <Card>
          <Tabs value={tabValue} onChange={(e, newValue) => setTabValue(newValue)}>
            <Tab label={`Service History (${services.length})`} />
            <Tab label={`Fuel Records (${fuelRecords.length})`} />
          </Tabs>

          <CardContent>
            {/* Service Records Tab */}
            {tabValue === 0 && (
              <Box>
                <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
                  <Button
                    startIcon={<Add />}
                    variant="contained"
                    size="small"
                    onClick={() => setServiceModalOpen(true)}
                  >
                    Add Service
                  </Button>
                </Box>

                {services.length === 0 ? (
                  <Typography color="text.secondary" align="center" py={3}>
                    No service records yet
                  </Typography>
                ) : (
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Date</TableCell>
                          <TableCell>Mileage</TableCell>
                          <TableCell>Type</TableCell>
                          <TableCell>Service Center</TableCell>
                          <TableCell align="right">Cost</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {services.map((service) => (
                          <TableRow key={service.id} hover>
                            <TableCell>{formatDate(service.serviceDate)}</TableCell>
                            <TableCell>{service.mileageAtService.toLocaleString()}</TableCell>
                            <TableCell>{getServiceTypeLabel(service.serviceType)}</TableCell>
                            <TableCell>{service.serviceCenter}</TableCell>
                            <TableCell align="right">
                              ${service.cost.toFixed(2)}
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                )}
              </Box>
            )}

            {/* Fuel Records Tab */}
            {tabValue === 1 && (
              <Box>
                <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
                  <Button
                    startIcon={<Add />}
                    variant="contained"
                    size="small"
                    onClick={() => setFuelModalOpen(true)}
                  >
                    Add Fuel Record
                  </Button>
                </Box>

                {fuelRecords.length === 0 ? (
                  <Typography color="text.secondary" align="center" py={3}>
                    No fuel records yet
                  </Typography>
                ) : (
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Date</TableCell>
                          <TableCell>Mileage</TableCell>
                          <TableCell align="right">Quantity</TableCell>
                          <TableCell align="right">Price/Unit</TableCell>
                          <TableCell align="right">Total Cost</TableCell>
                          <TableCell align="right">Efficiency</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {fuelRecords.map((fuel) => (
                          <TableRow key={fuel.id} hover>
                            <TableCell>{formatDate(fuel.refuelDate)}</TableCell>
                            <TableCell>{fuel.mileage.toLocaleString()}</TableCell>
                            <TableCell align="right">
                              {fuel.quantity.toFixed(2)} gal
                            </TableCell>
                            <TableCell align="right">
                              ${fuel.pricePerUnit.toFixed(2)}
                            </TableCell>
                            <TableCell align="right">
                              ${fuel.totalCost.toFixed(2)}
                            </TableCell>
                            <TableCell align="right">
                              {fuel.fuelEfficiency
                                ? `${fuel.fuelEfficiency.toFixed(1)} MPG`
                                : 'N/A'}
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                )}
              </Box>
            )}
          </CardContent>
        </Card>

        {/* Add Service Modal */}
        <Dialog open={serviceModalOpen} onClose={() => setServiceModalOpen(false)} maxWidth="sm" fullWidth>
          <form onSubmit={handleServiceSubmit}>
            <DialogTitle>Add Service Record</DialogTitle>
            <DialogContent>
              <Grid container spacing={2} sx={{ mt: 1 }}>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Service Date"
                    type="date"
                    value={serviceForm.serviceDate}
                    onChange={(e) => setServiceForm({ ...serviceForm, serviceDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                    required
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Mileage"
                    type="number"
                    value={serviceForm.mileageAtService}
                    onChange={(e) => setServiceForm({ ...serviceForm, mileageAtService: parseInt(e.target.value) })}
                    required
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    select
                    label="Service Type"
                    value={serviceForm.serviceType}
                    onChange={(e) => setServiceForm({ ...serviceForm, serviceType: Number(e.target.value) })}
                    required
                  >
                    <MenuItem value={0}>Oil Change</MenuItem>
                    <MenuItem value={1}>Tire Rotation</MenuItem>
                    <MenuItem value={2}>Brake Service</MenuItem>
                    <MenuItem value={3}>Inspection</MenuItem>
                    <MenuItem value={4}>General</MenuItem>
                    <MenuItem value={5}>Other</MenuItem>
                  </TextField>
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Service Center"
                    value={serviceForm.serviceCenter}
                    onChange={(e) => setServiceForm({ ...serviceForm, serviceCenter: e.target.value })}
                    required
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Description"
                    multiline
                    rows={2}
                    value={serviceForm.description}
                    onChange={(e) => setServiceForm({ ...serviceForm, description: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Cost"
                    type="number"
                    value={serviceForm.cost}
                    onChange={(e) => setServiceForm({ ...serviceForm, cost: parseFloat(e.target.value) })}
                    required
                    inputProps={{ step: 0.01 }}
                  />
                </Grid>
              </Grid>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setServiceModalOpen(false)}>Cancel</Button>
              <Button type="submit" variant="contained" disabled={saving}>
                {saving ? 'Saving...' : 'Add Service'}
              </Button>
            </DialogActions>
          </form>
        </Dialog>

        {/* Add Fuel Record Modal */}
        <Dialog open={fuelModalOpen} onClose={() => setFuelModalOpen(false)} maxWidth="sm" fullWidth>
          <form onSubmit={handleFuelSubmit}>
            <DialogTitle>Add Fuel Record</DialogTitle>
            <DialogContent>
              <Grid container spacing={2} sx={{ mt: 1 }}>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Refuel Date"
                    type="date"
                    value={fuelForm.refuelDate}
                    onChange={(e) => setFuelForm({ ...fuelForm, refuelDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                    required
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Mileage"
                    type="number"
                    value={fuelForm.mileage}
                    onChange={(e) => setFuelForm({ ...fuelForm, mileage: parseInt(e.target.value) })}
                    required
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Quantity (gallons)"
                    type="number"
                    value={fuelForm.quantity}
                    onChange={(e) => setFuelForm({ ...fuelForm, quantity: parseFloat(e.target.value) })}
                    required
                    inputProps={{ step: 0.01 }}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Price per Unit"
                    type="number"
                    value={fuelForm.pricePerUnit}
                    onChange={(e) => setFuelForm({ ...fuelForm, pricePerUnit: parseFloat(e.target.value) })}
                    required
                    inputProps={{ step: 0.01 }}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    select
                    label="Fuel Type"
                    value={fuelForm.fuelType}
                    onChange={(e) => setFuelForm({ ...fuelForm, fuelType: parseInt(e.target.value) })}
                    required
                  >
                    <MenuItem value={0}>Regular</MenuItem>
                    <MenuItem value={1}>Premium</MenuItem>
                    <MenuItem value={2}>Diesel</MenuItem>
                    <MenuItem value={3}>Electric</MenuItem>
                  </TextField>
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Gas Station"
                    value={fuelForm.gasStation}
                    onChange={(e) => setFuelForm({ ...fuelForm, gasStation: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Notes"
                    multiline
                    rows={2}
                    value={fuelForm.notes}
                    onChange={(e) => setFuelForm({ ...fuelForm, notes: e.target.value })}
                  />
                </Grid>
              </Grid>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setFuelModalOpen(false)}>Cancel</Button>
              <Button type="submit" variant="contained" disabled={saving}>
                {saving ? 'Saving...' : 'Add Fuel Record'}
              </Button>
            </DialogActions>
          </form>
        </Dialog>
      </Box>
    </AppLayout>
  );
};

export default VehicleDetails;
