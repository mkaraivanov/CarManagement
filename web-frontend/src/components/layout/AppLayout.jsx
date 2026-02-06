import { Box, Container } from '@mui/material';
import Navbar from './Navbar';

const AppLayout = ({ children }) => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Navbar />
      <Container
        component="main"
        maxWidth="lg"
        sx={{
          flexGrow: 1,
          py: 3,
          mt: 8, // Account for fixed navbar height
        }}
      >
        {children}
      </Container>
    </Box>
  );
};

export default AppLayout;
