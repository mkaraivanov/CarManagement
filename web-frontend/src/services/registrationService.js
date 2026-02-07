import api from './api';

const registrationService = {
  /**
   * Extract vehicle data from registration document using OCR
   * @param {File} file - Image or PDF file of vehicle registration
   * @returns {Promise<Object>} Extracted vehicle data with confidence scores
   */
  extractData: async (file) => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await api.post('/vehicle-registration/extract', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  /**
   * Upload registration document for a specific vehicle
   * @param {number} vehicleId - ID of the vehicle
   * @param {File} file - Image or PDF file of vehicle registration
   * @returns {Promise<Object>} Upload result with document URL
   */
  uploadDocument: async (vehicleId, file) => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await api.post(`/vehicle-registration/upload/${vehicleId}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};

export default registrationService;
