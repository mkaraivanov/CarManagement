import api from './api';

const vehicleReferenceService = {
  // Get all makes with their models
  getAllMakesWithModels: async () => {
    const response = await api.get('/vehiclereferences/makes');
    return response.data;
  },

  // Get just the make names (lightweight)
  getMakeNames: async () => {
    const response = await api.get('/vehiclereferences/makes/names');
    return response.data;
  },

  // Get models for a specific make
  getModelsByMake: async (makeId) => {
    const response = await api.get(`/vehiclereferences/makes/${makeId}/models`);
    return response.data;
  },
};

export default vehicleReferenceService;
