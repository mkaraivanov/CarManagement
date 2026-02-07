import { useState, useCallback } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  CircularProgress,
  Alert,
  Paper,
} from '@mui/material';
import {
  CloudUpload,
  Close,
  CheckCircle,
} from '@mui/icons-material';
import registrationService from '../../services/registrationService';
import ExtractedDataReview from './ExtractedDataReview';

const ALLOWED_TYPES = ['image/jpeg', 'image/jpg', 'image/png', 'application/pdf'];
const MAX_SIZE_MB = 10;

const validateFile = (file) => {
  if (!ALLOWED_TYPES.includes(file.type)) {
    throw new Error('Invalid file type. Please upload JPG, PNG, or PDF files only.');
  }
  if (file.size > MAX_SIZE_MB * 1024 * 1024) {
    throw new Error(`File size must be less than ${MAX_SIZE_MB}MB.`);
  }
};

const RegistrationUploadDialog = ({ open, onClose, onDataExtracted }) => {
  const [dragActive, setDragActive] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState('');
  const [selectedFile, setSelectedFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);
  const [extractedData, setExtractedData] = useState(null);

  const handleFile = useCallback((file) => {
    try {
      setError('');
      validateFile(file);
      setSelectedFile(file);

      // Create preview for images
      if (file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onloadend = () => {
          setPreviewUrl(reader.result);
        };
        reader.readAsDataURL(file);
      } else {
        setPreviewUrl(null);
      }
    } catch (err) {
      setError(err.message);
      setSelectedFile(null);
      setPreviewUrl(null);
    }
  }, []);

  const handleDrag = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  }, []);

  const handleDrop = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFile(e.dataTransfer.files[0]);
    }
  }, [handleFile]);

  const handleFileInput = (e) => {
    if (e.target.files && e.target.files[0]) {
      handleFile(e.target.files[0]);
    }
  };

  const handleExtract = async () => {
    if (!selectedFile) return;

    setUploading(true);
    setError('');
    setExtractedData(null);

    try {
      const result = await registrationService.extractData(selectedFile);
      setExtractedData(result);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to extract data from document. Please try again.');
    } finally {
      setUploading(false);
    }
  };

  const handleUseData = (reviewedData) => {
    onDataExtracted(reviewedData);
    handleClose();
  };

  const handleClose = () => {
    setSelectedFile(null);
    setPreviewUrl(null);
    setExtractedData(null);
    setError('');
    setDragActive(false);
    setUploading(false);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>
        Upload Vehicle Registration Document
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          Upload a photo or PDF of your vehicle registration to automatically extract vehicle information
        </Typography>
      </DialogTitle>

      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        {!extractedData ? (
          <>
            {/* File Upload Area */}
            <Paper
              variant="outlined"
              onDragEnter={handleDrag}
              onDragLeave={handleDrag}
              onDragOver={handleDrag}
              onDrop={handleDrop}
              sx={{
                p: 4,
                textAlign: 'center',
                borderStyle: 'dashed',
                borderWidth: 2,
                borderColor: dragActive ? 'primary.main' : 'grey.300',
                bgcolor: dragActive ? 'action.hover' : 'background.paper',
                cursor: 'pointer',
                transition: 'all 0.2s',
                '&:hover': {
                  borderColor: 'primary.main',
                  bgcolor: 'action.hover',
                },
              }}
            >
              <input
                type="file"
                id="file-upload"
                accept=".jpg,.jpeg,.png,.pdf"
                onChange={handleFileInput}
                style={{ display: 'none' }}
              />
              <label htmlFor="file-upload" style={{ cursor: 'pointer', display: 'block' }}>
                <CloudUpload sx={{ fontSize: 64, color: 'primary.main', mb: 2 }} />
                <Typography variant="h6" gutterBottom>
                  Drop your registration document here
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  or click to browse
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Supports JPG, PNG, PDF (max {MAX_SIZE_MB}MB)
                </Typography>
              </label>
            </Paper>

            {/* File Preview */}
            {selectedFile && (
              <Box sx={{ mt: 3 }}>
                <Typography variant="subtitle2" gutterBottom>
                  Selected File:
                </Typography>
                <Paper variant="outlined" sx={{ p: 2 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <CheckCircle color="success" />
                    <Typography variant="body2">
                      {selectedFile.name} ({(selectedFile.size / 1024 / 1024).toFixed(2)} MB)
                    </Typography>
                  </Box>
                  {previewUrl && (
                    <Box sx={{ mt: 2, textAlign: 'center' }}>
                      <img
                        src={previewUrl}
                        alt="Registration preview"
                        style={{
                          maxWidth: '100%',
                          maxHeight: '300px',
                          borderRadius: '4px',
                        }}
                      />
                    </Box>
                  )}
                </Paper>
              </Box>
            )}

            {/* Processing State */}
            {uploading && (
              <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 3 }}>
                <CircularProgress />
                <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
                  Extracting vehicle information...
                </Typography>
              </Box>
            )}
          </>
        ) : (
          /* Extracted Data Review */
          <ExtractedDataReview
            extractedData={extractedData}
            onUseData={handleUseData}
            onCancel={() => setExtractedData(null)}
          />
        )}
      </DialogContent>

      {!extractedData && (
        <DialogActions>
          <Button onClick={handleClose} startIcon={<Close />}>
            Cancel
          </Button>
          <Button
            onClick={handleExtract}
            variant="contained"
            disabled={!selectedFile || uploading}
            startIcon={<CloudUpload />}
          >
            {uploading ? 'Processing...' : 'Extract Data'}
          </Button>
        </DialogActions>
      )}
    </Dialog>
  );
};

export default RegistrationUploadDialog;
