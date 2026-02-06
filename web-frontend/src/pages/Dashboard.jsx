import { useState, useEffect } from 'react';
import { Box, Grid, Card, CardContent, Typography, CircularProgress } from '@mui/material';
import { DirectionsCar, Build, LocalGasStation, Add } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import AppLayout from '../components/layout/AppLayout';
import vehicleService from '../services/vehicleService';
import serviceRecordService from '../services/serviceRecordService';
import fuelRecordService from '../services/fuelRecordService';

const Dashboard = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalVehicles: 0,
    totalServiceRecords: 0,
    totalFuelRecords: 0,
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

      setStats({
        totalVehicles: vehicles.length,
        totalServiceRecords,
        totalFuelRecords,
      });
    } catch (err) {
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  const StatCard = ({ title, value, icon, color }) => (
    <Card sx={{ height: '100%' }}>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Box
            sx={{
              bgcolor: `${color}.light`,
              color: `${color}.main`,
              p: 1,
              borderRadius: 1,
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
          {loading ? <CircularProgress size={40} /> : value}
        </Typography>
      </CardContent>
    </Card>
  );

  const QuickActionCard = ({ title, description, icon, onClick, color }) => (
    <Card sx={{ height: '100%', cursor: 'pointer' }} onClick={onClick}>
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
      <Box>
        <Typography variant="h4" gutterBottom>
          Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary" mb={4}>
          Welcome to your car management dashboard
        </Typography>

        {/* Statistics Cards */}
        <Grid container spacing={3} mb={4}>
          <Grid item xs={12} md={4}>
            <StatCard
              title="Total Vehicles"
              value={stats.totalVehicles}
              icon={<DirectionsCar />}
              color="primary"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <StatCard
              title="Service Records"
              value={stats.totalServiceRecords}
              icon={<Build />}
              color="secondary"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <StatCard
              title="Fuel Records"
              value={stats.totalFuelRecords}
              icon={<LocalGasStation />}
              color="success"
            />
          </Grid>
        </Grid>

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
              title="Service & Fuel"
              description="Track service and fuel records"
              icon={<Build sx={{ fontSize: 40 }} />}
              color="secondary"
              onClick={() => navigate('/vehicles')}
            />
          </Grid>
        </Grid>
      </Box>
    </AppLayout>
  );
};

export default Dashboard;
