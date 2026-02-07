import { useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Chip,
  IconButton,
  Grid,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
} from '@mui/material';
import {
  CheckCircle,
  Warning,
  Error,
  Delete,
  Schedule,
} from '@mui/icons-material';
import { format } from 'date-fns';
import maintenanceScheduleService from '../../services/maintenanceScheduleService';

const MaintenanceScheduleList = ({ schedules, onUpdate }) => {
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [selectedSchedule, setSelectedSchedule] = useState(null);
  const [completionData, setCompletionData] = useState({
    completedDate: new Date().toISOString().split('T')[0],
    completedMileage: '',
    completedHours: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const getStatusColor = (status) => {
    switch (status) {
      case 'OK':
        return 'success';
      case 'Due Soon':
        return 'warning';
      case 'Overdue':
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'OK':
        return <CheckCircle />;
      case 'Due Soon':
        return <Warning />;
      case 'Overdue':
        return <Error />;
      default:
        return <Schedule />;
    }
  };

  const handleCompleteClick = (schedule) => {
    setSelectedSchedule(schedule);
    setCompletionData({
      completedDate: new Date().toISOString().split('T')[0],
      completedMileage: '',
      completedHours: '',
    });
    setError('');
    setCompleteDialogOpen(true);
  };

  const handleCompleteSubmit = async () => {
    try {
      setLoading(true);
      setError('');

      const data = {
        completedDate: completionData.completedDate,
        completedMileage: completionData.completedMileage ? parseInt(completionData.completedMileage) : undefined,
        completedHours: completionData.completedHours ? parseFloat(completionData.completedHours) : undefined,
      };

      await maintenanceScheduleService.complete(selectedSchedule.id, data);
      setCompleteDialogOpen(false);
      onUpdate();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to complete maintenance');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (scheduleId) => {
    if (window.confirm('Are you sure you want to delete this maintenance schedule?')) {
      try {
        await maintenanceScheduleService.delete(scheduleId);
        onUpdate();
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to delete schedule');
      }
    }
  };

  const formatInterval = (schedule) => {
    const parts = [];
    if (schedule.intervalMonths) {
      parts.push(`${schedule.intervalMonths} months`);
    }
    if (schedule.intervalKilometers) {
      parts.push(`${schedule.intervalKilometers.toLocaleString()} km`);
    }
    if (schedule.intervalHours) {
      parts.push(`${schedule.intervalHours} hours`);
    }

    if (parts.length === 0) return 'N/A';

    return parts.join(schedule.useCompoundRule ? ' OR ' : ' AND ');
  };

  const formatNextDue = (schedule) => {
    const parts = [];

    if (schedule.nextDueDate) {
      parts.push(format(new Date(schedule.nextDueDate), 'MMM dd, yyyy'));
    }

    if (schedule.nextDueMileage) {
      parts.push(`${schedule.nextDueMileage.toLocaleString()} km`);
    }

    if (schedule.nextDueHours) {
      parts.push(`${schedule.nextDueHours} hours`);
    }

    return parts.length > 0 ? parts.join(' or ') : 'Not set';
  };

  if (!schedules || schedules.length === 0) {
    return (
      <Alert severity="info">
        No maintenance schedules found. Create your first schedule to get started.
      </Alert>
    );
  }

  return (
    <Box>
      <Grid container spacing={2}>
        {schedules.map((schedule) => (
          <Grid item xs={12} key={schedule.id}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                  <Box sx={{ flex: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                      <Typography variant="h6">{schedule.taskName}</Typography>
                      <Chip
                        label={schedule.status}
                        color={getStatusColor(schedule.status)}
                        icon={getStatusIcon(schedule.status)}
                        size="small"
                      />
                      {schedule.category && (
                        <Chip label={schedule.category} size="small" variant="outlined" />
                      )}
                    </Box>
                    {schedule.description && (
                      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                        {schedule.description}
                      </Typography>
                    )}
                  </Box>
                  <Box sx={{ display: 'flex', gap: 1 }}>
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={() => handleCompleteClick(schedule)}
                      title="Mark as completed"
                    >
                      <CheckCircle />
                    </IconButton>
                    <IconButton
                      size="small"
                      color="error"
                      onClick={() => handleDelete(schedule.id)}
                      title="Delete schedule"
                    >
                      <Delete />
                    </IconButton>
                  </Box>
                </Box>

                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6} md={3}>
                    <Typography variant="caption" color="text.secondary">
                      Interval
                    </Typography>
                    <Typography variant="body2">{formatInterval(schedule)}</Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={3}>
                    <Typography variant="caption" color="text.secondary">
                      Next Due
                    </Typography>
                    <Typography variant="body2">{formatNextDue(schedule)}</Typography>
                  </Grid>
                  {schedule.lastCompletedDate && (
                    <Grid item xs={12} sm={6} md={3}>
                      <Typography variant="caption" color="text.secondary">
                        Last Completed
                      </Typography>
                      <Typography variant="body2">
                        {format(new Date(schedule.lastCompletedDate), 'MMM dd, yyyy')}
                        {schedule.lastCompletedMileage && ` at ${schedule.lastCompletedMileage.toLocaleString()} km`}
                      </Typography>
                    </Grid>
                  )}
                  {schedule.daysUntilDue !== null && schedule.daysUntilDue !== undefined && (
                    <Grid item xs={12} sm={6} md={3}>
                      <Typography variant="caption" color="text.secondary">
                        Time Remaining
                      </Typography>
                      <Typography variant="body2">
                        {schedule.daysUntilDue > 0 ? `${schedule.daysUntilDue} days` : 'Overdue'}
                      </Typography>
                    </Grid>
                  )}
                </Grid>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Complete Schedule Dialog */}
      <Dialog open={completeDialogOpen} onClose={() => setCompleteDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Complete Maintenance</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
            <TextField
              label="Completion Date"
              type="date"
              value={completionData.completedDate}
              onChange={(e) => setCompletionData({ ...completionData, completedDate: e.target.value })}
              InputLabelProps={{ shrink: true }}
              fullWidth
            />
            <TextField
              label="Mileage at Completion"
              type="number"
              value={completionData.completedMileage}
              onChange={(e) => setCompletionData({ ...completionData, completedMileage: e.target.value })}
              placeholder="Optional"
              fullWidth
            />
            <TextField
              label="Engine Hours at Completion"
              type="number"
              value={completionData.completedHours}
              onChange={(e) => setCompletionData({ ...completionData, completedHours: e.target.value })}
              placeholder="Optional (for equipment)"
              fullWidth
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCompleteDialogOpen(false)} disabled={loading}>
            Cancel
          </Button>
          <Button onClick={handleCompleteSubmit} variant="contained" disabled={loading}>
            {loading ? 'Completing...' : 'Complete'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default MaintenanceScheduleList;
