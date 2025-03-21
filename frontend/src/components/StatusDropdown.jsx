import React, { useState } from 'react';
import './StatusDropdown.css';

const StatusDropdown = ({ currentStatus, onChange }) => {
  const [isUpdating, setIsUpdating] = useState(false);
  
  // Status options mapping
  const statusOptions = [
    { value: 0, label: 'Applied', className: 'status-applied' },
    { value: 1, label: 'Interview', className: 'status-interview' },
    { value: 2, label: 'Offer', className: 'status-offer' },
    { value: 3, label: 'Rejected', className: 'status-rejected' }
  ];
  
  // Handler for status change
  const handleStatusChange = async (e) => {
    const newStatus = parseInt(e.target.value, 10);
    
    // Skip if same status selected
    if (newStatus === currentStatus) return;
    
    setIsUpdating(true);
    
    try {
      await onChange(newStatus);
    } catch (error) {
      console.error('Error updating status:', error);
    } finally {
      setIsUpdating(false);
    }
  };
  
  return (
    <div className="status-dropdown-container">
      <select 
        className={`status-dropdown ${statusOptions.find(opt => opt.value === currentStatus)?.className}`}
        value={currentStatus}
        onChange={handleStatusChange}
        disabled={isUpdating}
      >
        {statusOptions.map(option => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      {isUpdating && <div className="dropdown-spinner"></div>}
    </div>
  );
};

export default StatusDropdown;