import api from './api';

const authService = {
  register: async (username, email, password) => {
    const response = await api.post('/auth/register', {
      username,
      email,
      password,
    });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
    }
    return response.data;
  },

  login: async (username, password) => {
    const response = await api.post('/auth/login', {
      username,
      password,
    });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');
  },

  getCurrentUser: async () => {
    const response = await api.get('/users/me');
    return response.data;
  },

  isAuthenticated: () => {
    return !!localStorage.getItem('token');
  },
};

export default authService;
