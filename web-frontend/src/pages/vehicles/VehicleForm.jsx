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
import { Save, Cancel, CloudUpload } from '@mui/icons-material';
import AppLayout from '../../components/layout/AppLayout';
import vehicleService from '../../services/vehicleService';
import vehicleReferenceService from '../../services/vehicleReferenceService';
import RegistrationUploadDialog from '../../components/vehicles/RegistrationUploadDialog';
import PageTransition from '../../components/common/PageTransition';
import BackButton from '../../components/common/BackButton';

const VehicleForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [uploadDialogOpen, setUploadDialogOpen] = useState(false);

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
    // Registration fields
    registrationNumber: '',
    registrationIssueDate: '',
    registrationExpiryDate: '',
    ownerName: '',
    // Additional vehicle specs
    bodyType: '',
    engineInfo: '',
    fuelType: '',
    transmission: '',
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
        // Registration fields
        registrationNumber: data.registrationNumber || '',
        registrationIssueDate: data.registrationIssueDate ? data.registrationIssueDate.split('T')[0] : '',
        registrationExpiryDate: data.registrationExpiryDate ? data.registrationExpiryDate.split('T')[0] : '',
        ownerName: data.ownerName || '',
        // Additional vehicle specs
        bodyType: data.bodyType || '',
        engineInfo: data.engineInfo || '',
        fuelType: data.fuelType || '',
        transmission: data.transmission || '',
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

  const handleExtractedData = (extractedData) => {
    // Pre-fill form with extracted data
    setFormData((prev) => ({
      ...prev,
      ...(extractedData.make && { make: extractedData.make }),
      ...(extractedData.model && { model: extractedData.model }),
      ...(extractedData.year && { year: parseInt(extractedData.year) || prev.year }),
      ...(extractedData.licensePlate && { licensePlate: extractedData.licensePlate }),
      ...(extractedData.vin && { vin: extractedData.vin }),
      ...(extractedData.color && { color: extractedData.color }),
      ...(extractedData.registrationNumber && { registrationNumber: extractedData.registrationNumber }),
      ...(extractedData.registrationIssueDate && { registrationIssueDate: extractedData.registrationIssueDate }),
      ...(extractedData.registrationExpiryDate && { registrationExpiryDate: extractedData.registrationExpiryDate }),
      ...(extractedData.ownerName && { ownerName: extractedData.ownerName }),
      ...(extractedData.bodyType && { bodyType: extractedData.bodyType }),
      ...(extractedData.engineInfo && { engineInfo: extractedData.engineInfo }),
      ...(extractedData.fuelType && { fuelType: extractedData.fuelType }),
      ...(extractedData.transmission && { transmission: extractedData.transmission }),
    }));

    // If make was extracted, find and set the models
    if (extractedData.make && makes.length > 0) {
      const make = makes.find(m => m.name.toLowerCase() === extractedData.make.toLowerCase());
      if (make) {
        setSelectedMakeId(make.id);
        setAvailableModels(make.models);
      }
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
        registrationNumber: formData.registrationNumber || null,
        registrationIssueDate: formData.registrationIssueDate || null,
        registrationExpiryDate: formData.registrationExpiryDate || null,
        ownerName: formData.ownerName || null,
        bodyType: formData.bodyType || null,
        engineInfo: formData.engineInfo || null,
        fuelType: formData.fuelType || null,
        transmission: formData.transmission || null,
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
      <PageTransition>
        <Box>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <BackButton disabled={saving} />
            <Typography variant="h4">
              {isEdit ? 'Edit Vehicle' : 'Add New Vehicle'}
            </Typography>
          </Box>
          {!isEdit && (
            <Button
              variant="outlined"
              startIcon={<CloudUpload />}
              onClick={() => setUploadDialogOpen(true)}
            >
              Upload Registration
            </Button>
          )}
        </Box>

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
                    autoFocus
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

                {/* Registration Information Section */}
                <Grid item xs={12}>
                  <Typography variant="h6" sx={{ mt: 2, mb: 1 }}>
                    Registration Information (Optional)
                  </Typography>
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Registration Number"
                    name="registrationNumber"
                    value={formData.registrationNumber}
                    onChange={handleChange}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Owner Name"
                    name="ownerName"
                    value={formData.ownerName}
                    onChange={handleChange}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Registration Issue Date"
                    name="registrationIssueDate"
                    type="date"
                    value={formData.registrationIssueDate}
                    onChange={handleChange}
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Registration Expiry Date"
                    name="registrationExpiryDate"
                    type="date"
                    value={formData.registrationExpiryDate}
                    onChange={handleChange}
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>

                {/* Vehicle Specifications Section */}
                <Grid item xs={12}>
                  <Typography variant="h6" sx={{ mt: 2, mb: 1 }}>
                    Vehicle Specifications (Optional)
                  </Typography>
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Body Type"
                    name="bodyType"
                    value={formData.bodyType}
                    onChange={handleChange}
                    placeholder="e.g., Sedan, SUV, Truck"
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Fuel Type"
                    name="fuelType"
                    value={formData.fuelType}
                    onChange={handleChange}
                    placeholder="e.g., Gasoline, Diesel, Electric"
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Transmission"
                    name="transmission"
                    value={formData.transmission}
                    onChange={handleChange}
                    placeholder="e.g., Automatic, Manual, CVT"
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    fullWidth
                    label="Engine Info"
                    name="engineInfo"
                    value={formData.engineInfo}
                    onChange={handleChange}
                    placeholder="e.g., 2.0L 4-cylinder"
                  />
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

        {/* Registration Upload Dialog */}
        <RegistrationUploadDialog
          open={uploadDialogOpen}
          onClose={() => setUploadDialogOpen(false)}
          onDataExtracted={handleExtractedData}
        />
        </Box>
      </PageTransition>
    </AppLayout>
  );
};

export default VehicleForm;
