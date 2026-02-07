import { Skeleton, Card, CardContent, Grid, Box } from '@mui/material';

// Dashboard skeleton for stat cards
export const DashboardSkeleton = () => (
  <Grid container spacing={3}>
    {[1, 2, 3, 4].map((i) => (
      <Grid item xs={12} md={3} key={i}>
        <Card>
          <CardContent>
            <Skeleton variant="rectangular" height={60} sx={{ borderRadius: 1 }} />
            <Skeleton variant="text" sx={{ mt: 2, fontSize: '1.5rem' }} />
          </CardContent>
        </Card>
      </Grid>
    ))}
  </Grid>
);

// Table skeleton for vehicle list and similar tables
export const TableSkeleton = () => (
  <Card>
    <CardContent>
      {[1, 2, 3, 4, 5].map((i) => (
        <Skeleton
          key={i}
          variant="rectangular"
          height={50}
          sx={{ mb: 1, borderRadius: 1 }}
        />
      ))}
    </CardContent>
  </Card>
);

// Vehicle detail skeleton for detailed pages
export const VehicleDetailSkeleton = () => (
  <Box>
    {/* Header section */}
    <Card sx={{ mb: 3 }}>
      <CardContent>
        <Skeleton variant="text" sx={{ fontSize: '2rem', mb: 2 }} width="40%" />
        <Grid container spacing={2}>
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <Grid item xs={12} sm={6} md={4} key={i}>
              <Skeleton variant="text" sx={{ fontSize: '1rem' }} />
              <Skeleton variant="text" sx={{ fontSize: '1.2rem' }} />
            </Grid>
          ))}
        </Grid>
      </CardContent>
    </Card>

    {/* Tabs section */}
    <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
      <Skeleton variant="rectangular" height={48} sx={{ borderRadius: 1 }} />
    </Box>

    {/* Tab content */}
    <Card>
      <CardContent>
        {[1, 2, 3].map((i) => (
          <Skeleton
            key={i}
            variant="rectangular"
            height={60}
            sx={{ mb: 2, borderRadius: 1 }}
          />
        ))}
      </CardContent>
    </Card>
  </Box>
);

// Form skeleton for forms
export const FormSkeleton = () => (
  <Card>
    <CardContent>
      <Skeleton variant="text" sx={{ fontSize: '2rem', mb: 3 }} width="30%" />
      <Grid container spacing={2}>
        {[1, 2, 3, 4, 5, 6].map((i) => (
          <Grid item xs={12} sm={6} key={i}>
            <Skeleton variant="rectangular" height={56} sx={{ borderRadius: 1 }} />
          </Grid>
        ))}
      </Grid>
      <Box sx={{ mt: 3, display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
        <Skeleton variant="rectangular" width={100} height={36} sx={{ borderRadius: 1 }} />
        <Skeleton variant="rectangular" width={100} height={36} sx={{ borderRadius: 1 }} />
      </Box>
    </CardContent>
  </Card>
);
