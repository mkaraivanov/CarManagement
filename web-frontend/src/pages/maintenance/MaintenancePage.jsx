import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Tabs,
  Tab,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  CircularProgress,
  Chip,
} from '@mui/material';
import { Add, Build } from '@mui/icons-material';
import AppLayout from '../../components/layout/AppLayout';
import MaintenanceScheduleList from '../../components/maintenance/MaintenanceScheduleList';
import MaintenanceCalendar from '../../components/maintenance/MaintenanceCalendar';
import CreateScheduleForm from '../../components/maintenance/CreateScheduleForm';
import vehicleService from '../../services/vehicleService';
import maintenanceScheduleService from '../../services/maintenanceScheduleService';
import PageTransition from '../../components/common/PageTransition';
import BackButton from '../../components/common/BackButton';

const MaintenancePage = () => {
  const [activeTab, setActiveTab] = useState(0);
  const [vehicles, setVehicles] = useState([]);
  const [selectedVehicleId, setSelectedVehicleId] = useState('');
  const [schedules, setSchedules] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [stats, setStats] = useState({
    total: 0,
    overdue: 0,
    dueSoon: 0,
    ok: 0,
  });

  useEffect(() => {
    fetchVehicles();
  }, []);

  useEffect(() => {
    if (selectedVehicleId) {
      fetchSchedules();
    }
  }, [selectedVehicleId]);

  const fetchVehicles = async () => {
    try {
      setLoading(true);
      const data = await vehicleService.getAll();
      setVehicles(data);

      if (data.length > 0 && !selectedVehicleId) {
        setSelectedVehicleId(data[0].id);
      }
    } catch (err) {
      setError('Failed to load vehicles');
      console.error('Error fetching vehicles:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchSchedules = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await maintenanceScheduleService.getByVehicle(selectedVehicleId);
      setSchedules(data);

      // Calculate stats
      const stats = {
        total: data.length,
        overdue: data.filter(s => s.status === 'Overdue').length,
        dueSoon: data.filter(s => s.status === 'Due Soon').length,
        ok: data.filter(s => s.status === 'OK').length,
      };
      setStats(stats);
    } catch (err) {
      setError('Failed to load maintenance schedules');
      console.error('Error fetching schedules:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateSuccess = () => {
    fetchSchedules();
  };

  return (
    <AppLayout>
      <PageTransition>
        <Box>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 3 }}>
          <Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 1 }}>
              <BackButton to="/dashboard" variant="text" label="" />
              <Build sx={{ fontSize: 32 }} />
              <Typography variant="h4">Preventive Maintenance</Typography>
            </Box>
            <Typography variant="body1" color="text.secondary">
              Track and manage your vehicle maintenance schedules
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setCreateDialogOpen(true)}
            disabled={!selectedVehicleId}
          >
            Create Schedule
          </Button>
        </Box>

        {/* Vehicle Selector */}
        <Box sx={{ mb: 3 }}>
          <FormControl fullWidth sx={{ maxWidth: 400 }}>
            <InputLabel>Select Vehicle</InputLabel>
            <Select
              value={selectedVehicleId}
              onChange={(e) => setSelectedVehicleId(e.target.value)}
              label="Select Vehicle"
            >
              {vehicles.map((vehicle) => (
                <MenuItem key={vehicle.id} value={vehicle.id}>
                  {vehicle.year} {vehicle.make} {vehicle.model} ({vehicle.licensePlate})
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>

        {/* Stats */}
        {selectedVehicleId && schedules.length > 0 && (
          <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
            <Chip label={`Total: ${stats.total}`} />
            {stats.overdue > 0 && (
              <Chip label={`Overdue: ${stats.overdue}`} color="error" />
            )}
            {stats.dueSoon > 0 && (
              <Chip label={`Due Soon: ${stats.dueSoon}`} color="warning" />
            )}
            {stats.ok > 0 && (
              <Chip label={`OK: ${stats.ok}`} color="success" />
            )}
          </Box>
        )}

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
            <CircularProgress />
          </Box>
        ) : !selectedVehicleId ? (
          <Alert severity="info">
            Please add a vehicle first to create maintenance schedules.
          </Alert>
        ) : (
          <Box>
            <Tabs value={activeTab} onChange={(e, v) => setActiveTab(v)} sx={{ mb: 3 }}>
              <Tab label="List View" />
              <Tab label="Calendar View" />
            </Tabs>

            {activeTab === 0 && (
              <MaintenanceScheduleList
                schedules={schedules}
                onUpdate={fetchSchedules}
              />
            )}

            {activeTab === 1 && (
              <MaintenanceCalendar schedules={schedules} />
            )}
          </Box>
        )}

        {/* Create Schedule Dialog */}
        <CreateScheduleForm
          open={createDialogOpen}
          onClose={() => setCreateDialogOpen(false)}
          vehicleId={selectedVehicleId}
          onSuccess={handleCreateSuccess}
        />
        </Box>
      </PageTransition>
    </AppLayout>
  );
};

export default MaintenancePage;
