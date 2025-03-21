import React from 'react';
import { Link } from 'react-router-dom';
import StatusDropdown from './StatusDropdown';
import './ApplicationTable.css';

const ApplicationTable = ({ applications, loading, onStatusChange }) => {
  // Format date for display
  const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    // @ts-ignore
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  // Status badge component
  const StatusBadge = ({ status }) => {
    const statusMap = {
      0: { text: 'Applied', className: 'status-applied' },
      1: { text: 'Interview', className: 'status-interview' },
      2: { text: 'Offer', className: 'status-offer' },
      3: { text: 'Rejected', className: 'status-rejected' }
    };

    const { text, className } = statusMap[status] || { text: 'Unknown', className: '' };
    
    return <span className={`status-badge ${className}`}>{text}</span>;
  };

  // Loading state
  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Loading applications...</p>
      </div>
    );
  }

  // Empty state
  if (!applications || applications.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">
        <span role="img" aria-label="notes">📋</span>
        </div>
        <h3>No applications found</h3>
        <p>Start tracking your job applications</p>
        <Link to="/add" className="btn-add-new">
          Add New Application
        </Link>
      </div>
    );
  }

  return (
    <div className="table-responsive">
      <table className="applications-table">
        <thead>
          <tr>
            <th>Company</th>
            <th>Position</th>
            <th>Status</th>
            <th>Date Applied</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {applications.map(application => (
            <tr key={application.id}>
              <td>{application.companyName}</td>
              <td>{application.position}</td>
              <td>
                <StatusBadge status={application.status} />
              </td>
              <td>{formatDate(application.dateApplied)}</td>
              <td className="actions-cell">
                <StatusDropdown 
                  currentStatus={application.status} 
                  onChange={(newStatus) => onStatusChange(application.id, newStatus)} 
                />
                <Link 
                  to={`/edit/${application.id}`} 
                  className="btn-edit" 
                  title="Edit application"
                >
                  <i className="fas fa-edit"></i>
                </Link>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ApplicationTable;