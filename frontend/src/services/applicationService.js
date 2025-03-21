import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5095/api';

// Create axios instance with common configuration
const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Process pagination headers
const getPaginationFromHeaders = (headers) => {
  const paginationHeader = headers['x-pagination'];
  if (paginationHeader) {
    return JSON.parse(paginationHeader);
  }
  return null;
};

const applicationService = {
  // Get all applications with optional filtering and pagination
  async getApplications(pageNumber = 1, pageSize = 10, status = null, searchTerm = null) {
    let url = `/applications?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    
    if (status !== null) {
      url += `&status=${status}`;
    }
    
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    
    const response = await apiClient.get(url);
    
    return {
      applications: response.data.items,
      pagination: getPaginationFromHeaders(response.headers)
    };
  },
  
  // Get application by ID
  async getApplicationById(id) {
    const response = await apiClient.get(`/applications/${id}`);
    return response.data;
  },
  
  // Create new application
  async createApplication(application) {
    const response = await apiClient.post('/applications', application);
    return response.data;
  },
  
  // Update application
  async updateApplication(id, application) {
    await apiClient.put(`/applications/${id}`, application);
  },
  
  // Update application status only
  async updateApplicationStatus(id, status) {
    await apiClient.patch(`/applications/${id}/status`, { status });
  },
  
  // Delete application
  async deleteApplication(id) {
    await apiClient.delete(`/applications/${id}`);
  },
  
  // Get application statistics
  async getStatistics() {
    const response = await apiClient.get('/applications/statistics');
    return response.data;
  }
};

export default applicationService;