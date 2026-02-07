import api from './api';

const maintenanceScheduleService = {
  getByVehicle: async (vehicleId) => {
    const response = await api.get(`/maintenance-schedules/vehicle/${vehicleId}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/maintenance-schedules/${id}`);
    return response.data;
  },

  getOverdue: async () => {
    const response = await api.get('/maintenance-schedules/overdue');
    return response.data;
  },

  getUpcoming: async () => {
    const response = await api.get('/maintenance-schedules/upcoming');
    return response.data;
  },

  create: async (scheduleData) => {
    const response = await api.post('/maintenance-schedules', scheduleData);
    return response.data;
  },

  update: async (id, scheduleData) => {
    const response = await api.put(`/maintenance-schedules/${id}`, scheduleData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/maintenance-schedules/${id}`);
  },

  complete: async (id, completionData) => {
    const response = await api.post(`/maintenance-schedules/${id}/complete`, completionData);
    return response.data;
  },

  linkService: async (scheduleId, serviceRecordId) => {
    const response = await api.post(`/maintenance-schedules/${scheduleId}/link-service/${serviceRecordId}`);
    return response.data;
  },

  recalculate: async (id) => {
    const response = await api.post(`/maintenance-schedules/${id}/recalculate`);
    return response.data;
  },

  recalculateVehicle: async (vehicleId) => {
    const response = await api.post(`/maintenance-schedules/vehicle/${vehicleId}/recalculate`);
    return response.data;
  },
};

export default maintenanceScheduleService;
