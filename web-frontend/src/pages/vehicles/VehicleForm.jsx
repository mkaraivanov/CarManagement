import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  Grid,
  MenuItem,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Save, Cancel } from '@mui/icons-material';
import AppLayout from '../../components/layout/AppLayout';
import vehicleService from '../../services/vehicleService';
import vehicleReferenceService from '../../services/vehicleReferenceService';

const VehicleForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  // Reference data for dropdowns
  const [makes, setMakes] = useState([]);
  const [availableModels, setAvailableModels] = useState([]);
  const [selectedMakeId, setSelectedMakeId] = useState(null);

  const [formData, setFormData] = useState({
    make: '',
    model: '',
    year: new Date().getFullYear(),
    licensePlate: '',
    currentMileage: 0,
    vin: '',
    purchaseDate: '',
    color: '',
    status: 0, // Active
  });

  useEffect(() => {
    fetchMakes();
    if (isEdit) {
      fetchVehicle();
    }
  }, [id]);

  // Fetch all makes with models on component mount
  const fetchMakes = async () => {
    try {
      const data = await vehicleReferenceService.getAllMakesWithModels();
      setMakes(data);
    } catch (err) {
      console.error('Failed to load makes:', err);
    }
  };

  const fetchVehicle = async () => {
    try {
      const data = await vehicleService.getById(id);
      setFormData({
        make: data.make || '',
        model: data.model || '',
        year: data.year || new Date().getFullYear(),
        licensePlate: data.licensePlate || '',
        currentMileage: data.currentMileage || 0,
        vin: data.vin || '',
        purchaseDate: data.purchaseDate ? data.purchaseDate.split('T')[0] : '',
        color: data.color || '',
        status: data.status || 0,
      });

      // Find the make and set its models when editing
      if (data.make && makes.length > 0) {
        const make = makes.find(m => m.name === data.make);
        if (make) {
          setSelectedMakeId(make.id);
          setAvailableModels(make.models);
        }
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load vehicle');
    } finally {
      setLoading(false);
    }
  };

  // Update available models when makes data loads (for edit mode)
  useEffect(() => {
    if (isEdit && formData.make && makes.length > 0) {
      const make = makes.find(m => m.name === formData.make);
      if (make) {
        setSelectedMakeId(make.id);
        setAvailableModels(make.models);
      }
    }
  }, [makes, formData.make, isEdit]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'year' || name === 'currentMileage' || name === 'status'
        ? parseInt(value) || 0
        : value,
    }));
  };

  // Handle make selection - update available models
  const handleMakeChange = (e) => {
    const makeName = e.target.value;
    const make = makes.find(m => m.name === makeName);

    setFormData((prev) => ({
      ...prev,
      make: makeName,
      model: '', // Reset model when make changes
    }));

    if (make) {
      setSelectedMakeId(make.id);
      setAvailableModels(make.models);
    } else {
      setSelectedMakeId(null);
      setAvailableModels([]);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSaving(true);

    try {
      const submitData = {
        ...formData,
        purchaseDate: formData.purchaseDate || null,
        vin: formData.vin || null,
        color: formData.color || null,
      };

      if (isEdit) {
        await vehicleService.update(id, submitData);
      } else {
        await vehicleService.create(submitData);
      }
      navigate('/vehicles');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save vehicle');
    } finally {
      setSaving(false);
    }
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

  return (
    <AppLayout>
      <Box>
        <Typography variant="h4" gutterBottom>
          {isEdit ? 'Edit Vehicle' : 'Add New Vehicle'}
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        <Card>
          <CardContent>
            <form onSubmit={handleSubmit}>
              <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    select
                    label="Make"
                    name="make"
                    value={formData.make}
                    onChange={handleMakeChange}
                    required
                  >
                    <MenuItem value="">
                      <em>Select a make</em>
                    </MenuItem>
                    {makes.map((make) => (
                      <MenuItem key={make.id} value={make.name}>
                        {make.name}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    select
                    label="Model"
                    name="model"
                    value={formData.model}
                    onChange={handleChange}
                    required
                    disabled={!selectedMakeId}
                    helperText={!selectedMakeId ? 'Please select a make first' : ''}
                  >
                    <MenuItem value="">
                      <em>Select a model</em>
                    </MenuItem>
                    {availableModels.map((model) => (
                      <MenuItem key={model.id} value={model.name}>
                        {model.name}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Year"
                    name="year"
                    type="number"
                    value={formData.year}
                    onChange={handleChange}
                    required
                    inputProps={{ min: 1900, max: 2100 }}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="License Plate"
                    name="licensePlate"
                    value={formData.licensePlate}
                    onChange={handleChange}
                    required
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Current Mileage"
                    name="currentMileage"
                    type="number"
                    value={formData.currentMileage}
                    onChange={handleChange}
                    required
                    inputProps={{ min: 0 }}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="VIN (Optional)"
                    name="vin"
                    value={formData.vin}
                    onChange={handleChange}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Purchase Date (Optional)"
                    name="purchaseDate"
                    type="date"
                    value={formData.purchaseDate}
                    onChange={handleChange}
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Color (Optional)"
                    name="color"
                    value={formData.color}
                    onChange={handleChange}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    select
                    label="Status"
                    name="status"
                    value={formData.status}
                    onChange={handleChange}
                    required
                  >
                    <MenuItem value={0}>Active</MenuItem>
                    <MenuItem value={1}>Sold</MenuItem>
                    <MenuItem value={2}>Inactive</MenuItem>
                  </TextField>
                </Grid>
              </Grid>

              <Box sx={{ display: 'flex', gap: 2, mt: 3 }}>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={<Save />}
                  disabled={saving}
                >
                  {saving ? 'Saving...' : isEdit ? 'Update Vehicle' : 'Add Vehicle'}
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<Cancel />}
                  onClick={() => navigate('/vehicles')}
                  disabled={saving}
                >
                  Cancel
                </Button>
              </Box>
            </form>
          </CardContent>
        </Card>
      </Box>
    </AppLayout>
  );
};

export default VehicleForm;
