import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import ApplicationTable from './components/ApplicationTable';
import ApplicationForm from './components/ApplicationForm';
import Navbar from './components/Navbar';
import useApplications from './hooks/useApplications';
import './App.css';

function App() {
  const {
    applications,
    loading,
    error,
    pageNumber,
    setPageNumber,
    totalPages,
    setStatus,
    setSearchTerm,
    updateApplicationStatus
  } = useApplications();

  // Handler for status changes
  const handleStatusChange = async (id, newStatus) => {
    const result = await updateApplicationStatus(id, newStatus);
    if (result) {
      // Success notification could be shown here
    } else {
      // Error notification could be shown here
    }
  };

  // Handler for search
  const handleSearch = (term) => {
    setSearchTerm(term);
  };

  // Handler for filter
  const handleFilter = (statusValue) => {
    setStatus(statusValue);
  };

  // Handler for pagination
  const handlePageChange = (newPage) => {
    setPageNumber(newPage);
  };

  return (
    <Router>
      <div className="app">
        <Navbar />
        <div className="container">
          <Routes>
            <Route path="/" element={
              <div className="main-content">
                <div className="content-header">
                  <div className="search-filter">
                    <input
                      type="text"
                      placeholder="Search applications..."
                      onChange={(e) => handleSearch(e.target.value)}
                      className="search-input"
                    />
                    <div className="filter-buttons">
                      <button 
                        className="filter-button"
                        onClick={() => handleFilter(null)}
                      >
                        All
                      </button>
                      <button 
                        className="filter-button applied"
                        onClick={() => handleFilter(0)}
                      >
                        Applied
                      </button>
                      <button 
                        className="filter-button interview"
                        onClick={() => handleFilter(1)}
                      >
                        Interview
                      </button>
                      <button 
                        className="filter-button offer"
                        onClick={() => handleFilter(2)}
                      >
                        Offer
                      </button>
                      <button 
                        className="filter-button rejected"
                        onClick={() => handleFilter(3)}
                      >
                        Rejected
                      </button>
                    </div>
                  </div>
                  <div className="add-button-container">
                    <a href="/add" className="add-button">
                      <i className="fas fa-plus"></i> Add Application
                    </a>
                  </div>
                </div>
                
                {error && (
                  <div className="error-message">
                    <p>{error}</p>
                    <button onClick={() => window.location.reload()}>Retry</button>
                  </div>
                )}
                
                <ApplicationTable 
                  applications={applications}
                  loading={loading}
                  onStatusChange={handleStatusChange}
                />
                
                {applications.length > 0 && totalPages > 1 && (
                  <div className="pagination">
                    <button 
                      onClick={() => handlePageChange(pageNumber - 1)}
                      disabled={pageNumber === 1}
                      className="pagination-button"
                    >
                      <i className="fas fa-chevron-left"></i>
                    </button>
                    
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                      <button
                        key={page}
                        onClick={() => handlePageChange(page)}
                        className={`pagination-button ${pageNumber === page ? 'active' : ''}`}
                      >
                        {page}
                      </button>
                    ))}
                    
                    <button 
                      onClick={() => handlePageChange(pageNumber + 1)}
                      disabled={pageNumber === totalPages}
                      className="pagination-button"
                    >
                      <i className="fas fa-chevron-right"></i>
                    </button>
                  </div>
                )}
              </div>
            } />
            <Route path="/add" element={<ApplicationForm />} />
            <Route path="/edit/:id" element={<ApplicationForm isEditing={true} />} />
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;