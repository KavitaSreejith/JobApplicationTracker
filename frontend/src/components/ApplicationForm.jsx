import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import applicationService from '../services/applicationService';
import './ApplicationForm.css';

const ApplicationForm = ({ isEditing = false }) => {
  const navigate = useNavigate();
  const { id } = useParams();
  
  const [formData, setFormData] = useState({
    companyName: '',
    position: '',
    status: 0,
    dateApplied: new Date().toISOString().split('T')[0],
    contactPerson: '',
    contactEmail: '',
    notes: '',
    jobUrl: '',
    salaryRange: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [fetchLoading, setFetchLoading] = useState(isEditing);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  
  // Status options
  const statusOptions = [
    { value: 0, label: 'Applied' },
    { value: 1, label: 'Interview' },
    { value: 2, label: 'Offer' },
    { value: 3, label: 'Rejected' }
  ];
  
  // Fetch application data if editing
  useEffect(() => {
    const fetchApplication = async () => {
      if (!isEditing || !id) return;
      
      setFetchLoading(true);
      try {
        const application = await applicationService.getApplicationById(id);
        
        // Format date for input field
        const formattedApp = {
          ...application,
          dateApplied: new Date(application.dateApplied).toISOString().split('T')[0],
          salaryRange: application.salaryRange?.toString() || ''
        };
        
        setFormData(formattedApp);
      } catch (err) {
        console.error('Error fetching application:', err);
        setError('Failed to load application data');
      } finally {
        setFetchLoading(false);
      }
    };
    
    fetchApplication();
  }, [id, isEditing]);
  
  // Input change handler
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prevData => ({
      ...prevData,
      [name]: name === 'status' ? parseInt(value, 10) : value
    }));
  };
  
  // Form submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError('');
    
    try {
      // Prepare data for submission
      const submissionData = {
        ...formData,
        salaryRange: formData.salaryRange ? parseFloat(formData.salaryRange) : null
      };
      
      if (isEditing) {
        await applicationService.updateApplication(id, submissionData);
      } else {
        await applicationService.createApplication(submissionData);
      }
      
      // Navigate back to list on success
      navigate('/');
    } catch (err) {
      console.error('Error saving application:', err);
      setError(err.response?.data?.message || 'Failed to save application');
    } finally {
      setSubmitting(false);
    }
  };
  
  // Delete application
  const handleDelete = async () => {
    if (!isEditing || !id) return;
    
    if (!window.confirm('Are you sure you want to delete this application?')) {
      return;
    }
    
    setLoading(true);
    
    try {
      await applicationService.deleteApplication(id);
      navigate('/');
    } catch (err) {
      console.error('Error deleting application:', err);
      setError('Failed to delete application');
    } finally {
      setLoading(false);
    }
  };
  
  // Cancel and go back
  const handleCancel = () => {
    navigate('/');
  };
  
  // Show loading state while fetching data
  if (fetchLoading) {
    return (
      <div className="form-loading">
        <div className="spinner"></div>
        <p>Loading application data...</p>
      </div>
    );
  }
  
  return (
    <div className="application-form-container">
      <div className="application-form-header">
        <h2>{isEditing ? 'Edit Application' : 'Add New Application'}</h2>
      </div>
      
      {error && (
        <div className="form-error">
          <p>{error}</p>
        </div>
      )}
      
      <form onSubmit={handleSubmit} className="application-form">
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="companyName">Company Name*</label>
            <input
              type="text"
              id="companyName"
              name="companyName"
              value={formData.companyName}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="position">Position*</label>
            <input
              type="text"
              id="position"
              name="position"
              value={formData.position}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </div>
        </div>
        
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="status">Status*</label>
            <select
              id="status"
              name="status"
              value={formData.status}
              onChange={handleChange}
              required
              disabled={submitting}
            >
              {statusOptions.map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
          
          <div className="form-group">
            <label htmlFor="dateApplied">Date Applied*</label>
            <input
              type="date"
              id="dateApplied"
              name="dateApplied"
              value={formData.dateApplied}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </div>
        </div>
        
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="contactPerson">Contact Person</label>
            <input
              type="text"
              id="contactPerson"
              name="contactPerson"
              value={formData.contactPerson || ''}
              onChange={handleChange}
              disabled={submitting}
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="contactEmail">Contact Email</label>
            <input
              type="email"
              id="contactEmail"
              name="contactEmail"
              value={formData.contactEmail || ''}
              onChange={handleChange}
              disabled={submitting}
            />
          </div>
        </div>
        
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="jobUrl">Job URL</label>
            <input
              type="url"
              id="jobUrl"
              name="jobUrl"
              value={formData.jobUrl || ''}
              onChange={handleChange}
              disabled={submitting}
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="salaryRange">Salary Range</label>
            <input
              type="number"
              id="salaryRange"
              name="salaryRange"
              value={formData.salaryRange || ''}
              onChange={handleChange}
              disabled={submitting}
              step="1000"
            />
          </div>
        </div>
        
        <div className="form-group">
          <label htmlFor="notes">Notes</label>
          <textarea
            id="notes"
            name="notes"
            value={formData.notes || ''}
            onChange={handleChange}
            disabled={submitting}
            // @ts-ignore
            rows="4"
          ></textarea>
        </div>
        
        <div className="form-actions">
          <button 
            type="button" 
            className="btn-cancel" 
            onClick={handleCancel}
            disabled={submitting || loading}
          >
            Cancel
          </button>
          
          {isEditing && (
            <button 
              type="button" 
              className="btn-delete" 
              onClick={handleDelete}
              disabled={submitting || loading}
            >
              {loading ? 'Deleting...' : 'Delete'}
            </button>
          )}
          
          <button 
            type="submit" 
            className="btn-submit" 
            disabled={submitting || loading}
          >
            {submitting ? 'Saving...' : isEditing ? 'Update' : 'Add'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default ApplicationForm;