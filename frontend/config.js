/**
 * Application configuration
 * 
 * This file contains environment-specific settings that are loaded based on the
 * current environment (development, production, etc.). To add new environment variables, 
 * you need to:
 * 1. Add it to the appropriate .env.* file
 * 2. Add it to the config object below (with a fallback value)
 */

// Base configuration with fallback values
const config = {
    // API URL for backend services
    apiUrl: process.env.REACT_APP_API_URL || 'http://localhost:5095/api',
    // Current environment
    environment: process.env.REACT_APP_ENV || 'development',
    
    // Feature flags
    features: {
      enableStatistics: true,
      enableAdvancedFilters: process.env.REACT_APP_ENABLE_ADVANCED_FILTERS !== 'false',
      enableNotifications: process.env.REACT_APP_ENABLE_NOTIFICATIONS !== 'false',
    },
    
    // Pagination settings
    pagination: {
      defaultPageSize: 10,
      pageSizeOptions: [5, 10, 25, 50],
    },
  };
  
  // Export configuration
  export default config;
  
  // Helper to check if running in production
  export const isProduction = config.environment === 'production';
  
  // Helper to check if running in development
  export const isDevelopment = config.environment === 'development';