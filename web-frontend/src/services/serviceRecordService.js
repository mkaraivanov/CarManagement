import api from './api';

const serviceRecordService = {
  getByVehicle: async (vehicleId) => {
    const response = await api.get(`/vehicles/${vehicleId}/services`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/services/${id}`);
    return response.data;
  },

  create: async (vehicleId, serviceData) => {
    const response = await api.post(`/vehicles/${vehicleId}/services`, serviceData);
    return response.data;
  },

  update: async (id, serviceData) => {
    const response = await api.put(`/services/${id}`, serviceData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/services/${id}`);
  },
};

export default serviceRecordService;
