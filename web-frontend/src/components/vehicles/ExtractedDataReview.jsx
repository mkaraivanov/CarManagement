import { useState } from 'react';
import {
  Box,
  Typography,
  TextField,
  Button,
  Grid,
  Chip,
  Paper,
  Alert,
} from '@mui/material';
import {
  CheckCircle,
  Warning,
  Error as ErrorIcon,
  ArrowForward,
  Cancel,
} from '@mui/icons-material';

const ExtractedDataReview = ({ extractedData, onUseData, onCancel }) => {
  // Initialize editable data from extracted values
  const [editableData, setEditableData] = useState(() => {
    const initial = {};
    if (extractedData?.extractedData) {
      Object.keys(extractedData.extractedData).forEach((key) => {
        initial[key] = extractedData.extractedData[key]?.value || '';
      });
    }
    return initial;
  });

  const getConfidenceColor = (confidence) => {
    switch (confidence?.toLowerCase()) {
      case 'high':
        return 'success';
      case 'medium':
        return 'warning';
      case 'low':
        return 'error';
      default:
        return 'default';
    }
  };

  const getConfidenceIcon = (confidence) => {
    switch (confidence?.toLowerCase()) {
      case 'high':
        return <CheckCircle fontSize="small" />;
      case 'medium':
        return <Warning fontSize="small" />;
      case 'low':
        return <ErrorIcon fontSize="small" />;
      default:
        return null;
    }
  };

  const handleChange = (field, value) => {
    setEditableData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleUseData = () => {
    onUseData(editableData);
  };

  const fieldLabels = {
    make: 'Make',
    model: 'Model',
    year: 'Year',
    vin: 'VIN',
    licensePlate: 'License Plate',
    ownerName: 'Owner Name',
    registrationNumber: 'Registration Number',
    registrationIssueDate: 'Issue Date',
    registrationExpiryDate: 'Expiry Date',
    color: 'Color',
    bodyType: 'Body Type',
    engineInfo: 'Engine Info',
    fuelType: 'Fuel Type',
    transmission: 'Transmission',
  };

  const hasLowConfidenceFields = extractedData?.extractedData &&
    Object.values(extractedData.extractedData).some(
      (field) => field?.confidence?.toLowerCase() === 'low'
    );

  return (
    <Box>
      <Alert severity="info" sx={{ mb: 3 }}>
        <Typography variant="body2">
          Review the extracted information below. Fields marked with{' '}
          <Chip
            size="small"
            label="Low"
            color="error"
            icon={<ErrorIcon />}
            sx={{ mx: 0.5 }}
          />{' '}
          require your attention. You can edit any field before using this data.
        </Typography>
      </Alert>

      {hasLowConfidenceFields && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          Some fields have low confidence. Please verify and correct them before proceeding.
        </Alert>
      )}

      <Paper variant="outlined" sx={{ p: 3, mb: 2, maxHeight: '400px', overflowY: 'auto' }}>
        <Grid container spacing={2}>
          {extractedData?.extractedData &&
            Object.entries(extractedData.extractedData).map(([key, fieldData]) => {
              if (!fieldData?.value) return null;

              return (
                <Grid item xs={12} md={6} key={key}>
                  <Box sx={{ position: 'relative' }}>
                    <TextField
                      fullWidth
                      label={fieldLabels[key] || key}
                      value={editableData[key] || ''}
                      onChange={(e) => handleChange(key, e.target.value)}
                      size="small"
                      InputProps={{
                        endAdornment: (
                          <Chip
                            size="small"
                            label={fieldData.confidence || 'Unknown'}
                            color={getConfidenceColor(fieldData.confidence)}
                            icon={getConfidenceIcon(fieldData.confidence)}
                            sx={{ ml: 1 }}
                          />
                        ),
                      }}
                    />
                  </Box>
                </Grid>
              );
            })}
        </Grid>

        {(!extractedData?.extractedData ||
          Object.keys(extractedData.extractedData).length === 0) && (
          <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 3 }}>
            No data could be extracted from the document. Please try a different image or enter
            the information manually.
          </Typography>
        )}
      </Paper>

      {/* Raw OCR Text (for debugging) */}
      {extractedData?.rawText && (
        <Box sx={{ mt: 2 }}>
          <Typography variant="caption" color="text.secondary" gutterBottom>
            Raw extracted text (for reference):
          </Typography>
          <Paper
            variant="outlined"
            sx={{
              p: 1,
              maxHeight: '100px',
              overflowY: 'auto',
              bgcolor: 'grey.50',
              fontFamily: 'monospace',
              fontSize: '0.75rem',
            }}
          >
            {extractedData.rawText}
          </Paper>
        </Box>
      )}

      <Box sx={{ display: 'flex', gap: 2, mt: 3, justifyContent: 'flex-end' }}>
        <Button onClick={onCancel} startIcon={<Cancel />}>
          Try Another Document
        </Button>
        <Button
          onClick={handleUseData}
          variant="contained"
          startIcon={<ArrowForward />}
          disabled={
            !extractedData?.extractedData ||
            Object.keys(extractedData.extractedData).length === 0
          }
        >
          Use This Data
        </Button>
      </Box>
    </Box>
  );
};

export default ExtractedDataReview;
