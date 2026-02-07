import api from './api';

const maintenanceTemplateService = {
  getAll: async () => {
    const response = await api.get('/maintenance-templates');
    return response.data;
  },

  getSystemTemplates: async () => {
    const response = await api.get('/maintenance-templates/system');
    return response.data;
  },

  getByCategory: async (category) => {
    const response = await api.get(`/maintenance-templates/category/${category}`);
    return response.data;
  },

  getCategories: async () => {
    const response = await api.get('/maintenance-templates/categories');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/maintenance-templates/${id}`);
    return response.data;
  },

  create: async (templateData) => {
    const response = await api.post('/maintenance-templates', templateData);
    return response.data;
  },

  update: async (id, templateData) => {
    const response = await api.put(`/maintenance-templates/${id}`, templateData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/maintenance-templates/${id}`);
  },
};

export default maintenanceTemplateService;
