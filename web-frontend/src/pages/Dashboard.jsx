import { useState, useEffect } from 'react';
import { Box, Grid, Card, CardContent, Typography, Alert } from '@mui/material';
import { DirectionsCar, Build, LocalGasStation, Add, Schedule, Warning } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import AppLayout from '../components/layout/AppLayout';
import vehicleService from '../services/vehicleService';
import serviceRecordService from '../services/serviceRecordService';
import fuelRecordService from '../services/fuelRecordService';
import maintenanceScheduleService from '../services/maintenanceScheduleService';
import { DashboardSkeleton } from '../components/common/LoadingSkeleton';
import PageTransition from '../components/common/PageTransition';

const Dashboard = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalVehicles: 0,
    totalServiceRecords: 0,
    totalFuelRecords: 0,
    maintenanceOverdue: 0,
    maintenanceDueSoon: 0,
  });

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);

      // Fetch all vehicles
      const vehicles = await vehicleService.getAll();

      // Fetch service and fuel records for all vehicles
      let totalServiceRecords = 0;
      let totalFuelRecords = 0;

      for (const vehicle of vehicles) {
        try {
          const services = await serviceRecordService.getByVehicle(vehicle.id);
          totalServiceRecords += services.length;
        } catch (err) {
          // Vehicle might not have service records yet
          console.error(`Error fetching services for vehicle ${vehicle.id}:`, err);
        }

        try {
          const fuelRecords = await fuelRecordService.getByVehicle(vehicle.id);
          totalFuelRecords += fuelRecords.length;
        } catch (err) {
          // Vehicle might not have fuel records yet
          console.error(`Error fetching fuel records for vehicle ${vehicle.id}:`, err);
        }
      }

      // Fetch maintenance stats
      let maintenanceOverdue = 0;
      let maintenanceDueSoon = 0;

      try {
        const overdueSchedules = await maintenanceScheduleService.getOverdue();
        maintenanceOverdue = overdueSchedules.length;
      } catch (err) {
        console.error('Error fetching overdue maintenance:', err);
      }

      try {
        const upcomingSchedules = await maintenanceScheduleService.getUpcoming();
        maintenanceDueSoon = upcomingSchedules.length;
      } catch (err) {
        console.error('Error fetching upcoming maintenance:', err);
      }

      setStats({
        totalVehicles: vehicles.length,
        totalServiceRecords,
        totalFuelRecords,
        maintenanceOverdue,
        maintenanceDueSoon,
      });
    } catch (err) {
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  const StatCard = ({ title, value, icon, color }) => (
    <Card
      sx={{
        height: '100%',
        '&:hover': {
          boxShadow: 4,
          transform: 'translateY(-2px)',
          transition: 'all 0.3s ease',
        },
      }}
    >
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Box
            sx={{
              bgcolor: `${color}.light`,
              color: `${color}.main`,
              p: 1,
              borderRadius: 2,
              mr: 2,
            }}
          >
            {icon}
          </Box>
          <Typography variant="h6" color="text.secondary">
            {title}
          </Typography>
        </Box>
        <Typography variant="h3" component="div">
          {value}
        </Typography>
      </CardContent>
    </Card>
  );

  const QuickActionCard = ({ title, description, icon, onClick, color }) => (
    <Card
      sx={{
        height: '100%',
        cursor: 'pointer',
        '&:hover': {
          boxShadow: 6,
          transform: 'translateY(-4px)',
          transition: 'all 0.3s ease',
        },
        '&:active': {
          transform: 'translateY(-2px)',
        },
      }}
      onClick={onClick}
    >
      <CardContent>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            textAlign: 'center',
          }}
        >
          <Box
            sx={{
              bgcolor: `${color}.light`,
              color: `${color}.main`,
              p: 2,
              borderRadius: 2,
              mb: 2,
            }}
          >
            {icon}
          </Box>
          <Typography variant="h6" gutterBottom>
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );

  return (
    <AppLayout>
      <PageTransition>
        <Box>
        <Typography variant="h4" gutterBottom>
          Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary" mb={4}>
          Welcome to your car management dashboard
        </Typography>

        {/* Maintenance Alerts */}
        {stats.maintenanceOverdue > 0 && (
          <Alert severity="error" sx={{ mb: 3 }}>
            <Typography variant="subtitle2" gutterBottom>
              {stats.maintenanceOverdue} overdue maintenance {stats.maintenanceOverdue === 1 ? 'item' : 'items'}
            </Typography>
            <Typography variant="body2">
              Some maintenance tasks are past due. Visit the{' '}
              <strong style={{ cursor: 'pointer' }} onClick={() => navigate('/maintenance')}>
                Maintenance
              </strong>{' '}
              page to review and complete them.
            </Typography>
          </Alert>
        )}

        {stats.maintenanceDueSoon > 0 && stats.maintenanceOverdue === 0 && (
          <Alert severity="warning" sx={{ mb: 3 }}>
            <Typography variant="subtitle2" gutterBottom>
              {stats.maintenanceDueSoon} upcoming maintenance {stats.maintenanceDueSoon === 1 ? 'item' : 'items'}
            </Typography>
            <Typography variant="body2">
              Some maintenance tasks are due soon. Visit the{' '}
              <strong style={{ cursor: 'pointer' }} onClick={() => navigate('/maintenance')}>
                Maintenance
              </strong>{' '}
              page to review them.
            </Typography>
          </Alert>
        )}

        {/* Statistics Cards */}
        {loading ? (
          <Box mb={4}>
            <DashboardSkeleton />
          </Box>
        ) : (
          <Grid container spacing={3} mb={4}>
            <Grid item xs={12} md={3}>
              <StatCard
                title="Total Vehicles"
                value={stats.totalVehicles}
                icon={<DirectionsCar />}
                color="primary"
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <StatCard
                title="Service Records"
                value={stats.totalServiceRecords}
                icon={<Build />}
                color="secondary"
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <StatCard
                title="Fuel Records"
                value={stats.totalFuelRecords}
                icon={<LocalGasStation />}
                color="success"
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <StatCard
                title="Overdue Maintenance"
                value={stats.maintenanceOverdue}
                icon={<Warning />}
                color={stats.maintenanceOverdue > 0 ? 'error' : 'info'}
              />
            </Grid>
          </Grid>
        )}

        {/* Quick Actions */}
        <Typography variant="h5" gutterBottom mt={4} mb={2}>
          Quick Actions
        </Typography>
        <Grid container spacing={3}>
          <Grid item xs={12} md={4}>
            <QuickActionCard
              title="Add Vehicle"
              description="Register a new vehicle to your fleet"
              icon={<Add sx={{ fontSize: 40 }} />}
              color="primary"
              onClick={() => navigate('/vehicles/new')}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <QuickActionCard
              title="View Vehicles"
              description="See all your registered vehicles"
              icon={<DirectionsCar sx={{ fontSize: 40 }} />}
              color="primary"
              onClick={() => navigate('/vehicles')}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <QuickActionCard
              title="Maintenance"
              description="Manage preventive maintenance schedules"
              icon={<Schedule sx={{ fontSize: 40 }} />}
              color="secondary"
              onClick={() => navigate('/maintenance')}
            />
          </Grid>
        </Grid>
        </Box>
      </PageTransition>
    </AppLayout>
  );
};

export default Dashboard;
