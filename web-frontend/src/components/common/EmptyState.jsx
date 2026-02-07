import { Box, Typography, Button } from '@mui/material';
import { motion } from 'framer-motion';

const EmptyState = ({ icon, title, description, actionLabel, onAction }) => {
  return (
    <Box
      component={motion.div}
      initial={{ opacity: 0, scale: 0.9 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.3 }}
      sx={{
        textAlign: 'center',
        py: 8,
        px: 3,
      }}
    >
      <Box
        sx={{
          fontSize: 80,
          color: 'text.secondary',
          mb: 3,
          opacity: 0.5,
        }}
      >
        {icon}
      </Box>
      <Typography variant="h5" gutterBottom color="text.secondary">
        {title}
      </Typography>
      <Typography variant="body1" color="text.secondary" mb={3}>
        {description}
      </Typography>
      {actionLabel && onAction && (
        <Button variant="contained" onClick={onAction}>
          {actionLabel}
        </Button>
      )}
    </Box>
  );
};

export default EmptyState;
