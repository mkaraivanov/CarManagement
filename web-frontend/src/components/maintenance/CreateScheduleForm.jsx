import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Box,
  Typography,
  Alert,
  FormControlLabel,
  Switch,
  Tabs,
  Tab,
  Chip,
  Grid,
} from '@mui/material';
import maintenanceTemplateService from '../../services/maintenanceTemplateService';
import maintenanceScheduleService from '../../services/maintenanceScheduleService';

const CreateScheduleForm = ({ open, onClose, vehicleId, onSuccess }) => {
  const [activeTab, setActiveTab] = useState(0);
  const [templates, setTemplates] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [selectedTemplate, setSelectedTemplate] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    taskName: '',
    description: '',
    category: '',
    intervalMonths: '',
    intervalKilometers: '',
    intervalHours: '',
    useCompoundRule: true,
    lastCompletedDate: '',
    lastCompletedMileage: '',
    lastCompletedHours: '',
    reminderDaysBefore: 30,
    reminderKilometersBefore: 1000,
    reminderHoursBefore: 10,
  });

  useEffect(() => {
    if (open) {
      fetchTemplates();
      fetchCategories();
    }
  }, [open]);

  const fetchTemplates = async () => {
    try {
      const data = await maintenanceTemplateService.getSystemTemplates();
      setTemplates(data);
    } catch (err) {
      console.error('Error fetching templates:', err);
    }
  };

  const fetchCategories = async () => {
    try {
      const data = await maintenanceTemplateService.getCategories();
      setCategories(data);
    } catch (err) {
      console.error('Error fetching categories:', err);
    }
  };

  const handleTemplateSelect = (template) => {
    setSelectedTemplate(template);
    setFormData({
      ...formData,
      taskName: template.name,
      description: template.description || '',
      category: template.category,
      intervalMonths: template.defaultIntervalMonths || '',
      intervalKilometers: template.defaultIntervalKilometers || '',
      intervalHours: template.defaultIntervalHours || '',
      useCompoundRule: template.useCompoundRule,
    });
  };

  const handleSubmit = async () => {
    try {
      setLoading(true);
      setError('');

      if (!formData.taskName) {
        setError('Task name is required');
        return;
      }

      if (!formData.intervalMonths && !formData.intervalKilometers && !formData.intervalHours) {
        setError('At least one interval (months, kilometers, or hours) is required');
        return;
      }

      const scheduleData = {
        vehicleId,
        templateId: selectedTemplate?.id,
        taskName: formData.taskName,
        description: formData.description || undefined,
        category: formData.category || undefined,
        intervalMonths: formData.intervalMonths ? parseInt(formData.intervalMonths) : undefined,
        intervalKilometers: formData.intervalKilometers ? parseInt(formData.intervalKilometers) : undefined,
        intervalHours: formData.intervalHours ? parseFloat(formData.intervalHours) : undefined,
        useCompoundRule: formData.useCompoundRule,
        lastCompletedDate: formData.lastCompletedDate || undefined,
        lastCompletedMileage: formData.lastCompletedMileage ? parseInt(formData.lastCompletedMileage) : undefined,
        lastCompletedHours: formData.lastCompletedHours ? parseFloat(formData.lastCompletedHours) : undefined,
        reminderDaysBefore: parseInt(formData.reminderDaysBefore),
        reminderKilometersBefore: parseInt(formData.reminderKilometersBefore),
        reminderHoursBefore: parseFloat(formData.reminderHoursBefore),
      };

      await maintenanceScheduleService.create(scheduleData);
      onSuccess();
      handleClose();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create maintenance schedule');
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setFormData({
      taskName: '',
      description: '',
      category: '',
      intervalMonths: '',
      intervalKilometers: '',
      intervalHours: '',
      useCompoundRule: true,
      lastCompletedDate: '',
      lastCompletedMileage: '',
      lastCompletedHours: '',
      reminderDaysBefore: 30,
      reminderKilometersBefore: 1000,
      reminderHoursBefore: 10,
    });
    setSelectedTemplate(null);
    setActiveTab(0);
    setSelectedCategory('all');
    setError('');
    onClose();
  };

  const filteredTemplates = selectedCategory === 'all'
    ? templates
    : templates.filter(t => t.category === selectedCategory);

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>Create Maintenance Schedule</DialogTitle>
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Tabs value={activeTab} onChange={(e, v) => setActiveTab(v)} sx={{ mb: 3 }}>
          <Tab label="From Template" />
          <Tab label="Custom Schedule" />
        </Tabs>

        {activeTab === 0 ? (
          <Box>
            <FormControl fullWidth sx={{ mb: 2 }}>
              <InputLabel>Category</InputLabel>
              <Select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                label="Category"
              >
                <MenuItem value="all">All Categories</MenuItem>
                {categories.map((cat) => (
                  <MenuItem key={cat} value={cat}>
                    {cat}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <Grid container spacing={2}>
              {filteredTemplates.map((template) => (
                <Grid item xs={12} sm={6} key={template.id}>
                  <Box
                    sx={{
                      p: 2,
                      border: 1,
                      borderColor: selectedTemplate?.id === template.id ? 'primary.main' : 'divider',
                      borderRadius: 1,
                      cursor: 'pointer',
                      bgcolor: selectedTemplate?.id === template.id ? 'action.selected' : 'background.paper',
                      '&:hover': {
                        bgcolor: 'action.hover',
                      },
                    }}
                    onClick={() => handleTemplateSelect(template)}
                  >
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 1 }}>
                      <Typography variant="subtitle1">{template.name}</Typography>
                      <Chip label={template.category} size="small" />
                    </Box>
                    {template.description && (
                      <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                        {template.description}
                      </Typography>
                    )}
                    <Typography variant="caption" color="text.secondary">
                      {[
                        template.defaultIntervalMonths && `${template.defaultIntervalMonths} months`,
                        template.defaultIntervalKilometers && `${template.defaultIntervalKilometers.toLocaleString()} km`,
                        template.defaultIntervalHours && `${template.defaultIntervalHours} hours`,
                      ]
                        .filter(Boolean)
                        .join(template.useCompoundRule ? ' OR ' : ' AND ')}
                    </Typography>
                  </Box>
                </Grid>
              ))}
            </Grid>

            {selectedTemplate && (
              <Box sx={{ mt: 3 }}>
                <Typography variant="subtitle2" gutterBottom>
                  Customize Schedule
                </Typography>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                  <TextField
                    label="Task Name"
                    value={formData.taskName}
                    onChange={(e) => setFormData({ ...formData, taskName: e.target.value })}
                    fullWidth
                    required
                  />
                  <TextField
                    label="Last Completed Date"
                    type="date"
                    value={formData.lastCompletedDate}
                    onChange={(e) => setFormData({ ...formData, lastCompletedDate: e.target.value })}
                    InputLabelProps={{ shrink: true }}
                    fullWidth
                    helperText="Optional - When was this maintenance last done?"
                  />
                  {formData.intervalKilometers && (
                    <TextField
                      label="Last Completed Mileage"
                      type="number"
                      value={formData.lastCompletedMileage}
                      onChange={(e) => setFormData({ ...formData, lastCompletedMileage: e.target.value })}
                      fullWidth
                      helperText="Optional - Mileage when last completed"
                    />
                  )}
                </Box>
              </Box>
            )}
          </Box>
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <TextField
              label="Task Name"
              value={formData.taskName}
              onChange={(e) => setFormData({ ...formData, taskName: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Description"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              fullWidth
              multiline
              rows={2}
            />
            <FormControl fullWidth>
              <InputLabel>Category</InputLabel>
              <Select
                value={formData.category}
                onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                label="Category"
              >
                {categories.map((cat) => (
                  <MenuItem key={cat} value={cat}>
                    {cat}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <Typography variant="subtitle2" gutterBottom>
              Intervals
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Months"
                  type="number"
                  value={formData.intervalMonths}
                  onChange={(e) => setFormData({ ...formData, intervalMonths: e.target.value })}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Kilometers"
                  type="number"
                  value={formData.intervalKilometers}
                  onChange={(e) => setFormData({ ...formData, intervalKilometers: e.target.value })}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Engine Hours"
                  type="number"
                  value={formData.intervalHours}
                  onChange={(e) => setFormData({ ...formData, intervalHours: e.target.value })}
                  fullWidth
                />
              </Grid>
            </Grid>

            <FormControlLabel
              control={
                <Switch
                  checked={formData.useCompoundRule}
                  onChange={(e) => setFormData({ ...formData, useCompoundRule: e.target.checked })}
                />
              }
              label={formData.useCompoundRule ? 'OR logic (whichever comes first)' : 'AND logic (all must be met)'}
            />

            <Typography variant="subtitle2" gutterBottom>
              Last Completed (Optional)
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Date"
                  type="date"
                  value={formData.lastCompletedDate}
                  onChange={(e) => setFormData({ ...formData, lastCompletedDate: e.target.value })}
                  InputLabelProps={{ shrink: true }}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Mileage"
                  type="number"
                  value={formData.lastCompletedMileage}
                  onChange={(e) => setFormData({ ...formData, lastCompletedMileage: e.target.value })}
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  label="Hours"
                  type="number"
                  value={formData.lastCompletedHours}
                  onChange={(e) => setFormData({ ...formData, lastCompletedHours: e.target.value })}
                  fullWidth
                />
              </Grid>
            </Grid>
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={loading}>
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={loading || (activeTab === 0 && !selectedTemplate)}>
          {loading ? 'Creating...' : 'Create Schedule'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default CreateScheduleForm;
