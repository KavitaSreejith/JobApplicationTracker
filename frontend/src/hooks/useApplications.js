import { useState, useEffect, useCallback } from 'react';
import applicationService from '../services/applicationService';

const useApplications = (initialPageNumber = 1, initialPageSize = 10) => {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [status, setStatus] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [refreshCounter, setRefreshCounter] = useState(0);

  // Fetch applications
  const fetchApplications = useCallback(async () => {
    setLoading(true);
    setError(null);
    
    try {
      const { applications: fetchedApplications, pagination } = await applicationService.getApplications(
        pageNumber,
        pageSize,
        status,
        searchTerm
      );
      
      setApplications(fetchedApplications);
      
      if (pagination) {
        setTotalCount(pagination.totalCount);
        setTotalPages(pagination.totalPages);
      }
    } catch (err) {
      setError(err.message || 'Failed to fetch applications');
      console.error('Error fetching applications:', err);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, status, searchTerm]);

  // Force refresh
  const refreshApplications = useCallback(() => {
    setRefreshCounter(count => count + 1);
  }, []);

  // Load applications when dependencies change
  useEffect(() => {
    fetchApplications();
  }, [fetchApplications, refreshCounter]);

  // Reset to page 1 when filters change
  useEffect(() => {
    setPageNumber(1);
  }, [status, searchTerm]);

  // Update application status
  const updateApplicationStatus = useCallback(async (id, newStatus) => {
    try {
      await applicationService.updateApplicationStatus(id, newStatus);
      
      // Update local state to avoid refetching
      setApplications(prevApplications =>
        prevApplications.map(app =>
          app.id === id ? { ...app, status: newStatus } : app
        )
      );
      
      return true;
    } catch (err) {
      console.error('Error updating application status:', err);
      return false;
    }
  }, []);

  // Delete application
  const deleteApplication = useCallback(async (id) => {
    try {
      await applicationService.deleteApplication(id);
      
      // Update local state
      setApplications(prevApplications =>
        prevApplications.filter(app => app.id !== id)
      );
      
      // Update total count
      setTotalCount(prevCount => prevCount - 1);
      
      return true;
    } catch (err) {
      console.error('Error deleting application:', err);
      return false;
    }
  }, []);

  return {
    applications,
    loading,
    error,
    pageNumber,
    pageSize,
    totalCount,
    totalPages,
    status,
    searchTerm,
    setPageNumber,
    setPageSize,
    setStatus,
    setSearchTerm,
    refreshApplications,
    updateApplicationStatus,
    deleteApplication
  };
};

export default useApplications;