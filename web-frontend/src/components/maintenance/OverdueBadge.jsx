import { useState, useEffect } from 'react';
import { Chip, Tooltip } from '@mui/material';
import { Warning } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import maintenanceScheduleService from '../../services/maintenanceScheduleService';

const OverdueBadge = () => {
  const [overdueCount, setOverdueCount] = useState(0);
  const navigate = useNavigate();

  const fetchOverdueCount = async () => {
    try {
      const schedules = await maintenanceScheduleService.getOverdue();
      setOverdueCount(schedules.length);
    } catch (err) {
      console.error('Error fetching overdue schedules:', err);
    }
  };

  useEffect(() => {
    fetchOverdueCount();
    const interval = setInterval(fetchOverdueCount, 300000); // Update every 5 minutes
    return () => clearInterval(interval);
  }, []);

  if (overdueCount === 0) {
    return null;
  }

  return (
    <Tooltip title={`${overdueCount} overdue maintenance ${overdueCount === 1 ? 'item' : 'items'}`}>
      <Chip
        icon={<Warning />}
        label={`${overdueCount} Overdue`}
        color="error"
        size="small"
        onClick={() => navigate('/maintenance')}
        sx={{ cursor: 'pointer' }}
      />
    </Tooltip>
  );
};

export default OverdueBadge;
