import api from './api';

const notificationService = {
  getAll: async (limit = 50) => {
    const response = await api.get(`/notifications?limit=${limit}`);
    return response.data;
  },

  getUnread: async () => {
    const response = await api.get('/notifications/unread');
    return response.data;
  },

  getCount: async () => {
    const response = await api.get('/notifications/count');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/notifications/${id}`);
    return response.data;
  },

  markAsRead: async (id) => {
    const response = await api.post(`/notifications/${id}/read`);
    return response.data;
  },

  markAllAsRead: async () => {
    const response = await api.post('/notifications/read-all');
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/notifications/${id}`);
  },
};

export default notificationService;
