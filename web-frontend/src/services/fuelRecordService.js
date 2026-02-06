import api from './api';

const fuelRecordService = {
  getByVehicle: async (vehicleId) => {
    const response = await api.get(`/vehicles/${vehicleId}/fuel-records`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/fuel-records/${id}`);
    return response.data;
  },

  create: async (vehicleId, fuelData) => {
    const response = await api.post(`/vehicles/${vehicleId}/fuel-records`, fuelData);
    return response.data;
  },

  update: async (id, fuelData) => {
    const response = await api.put(`/fuel-records/${id}`, fuelData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/fuel-records/${id}`);
  },

  getFuelEfficiency: async (vehicleId) => {
    const response = await api.get(`/vehicles/${vehicleId}/fuel-efficiency`);
    return response.data;
  },
};

export default fuelRecordService;
