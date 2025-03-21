import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-logo">
          <i className="fas fa-briefcase"></i>
          <div className="logo-text">
            <span className="app-name">Job Application Tracker</span>
            <span className="app-tagline">Manage your job hunt</span>
          </div>
        </Link>
        
        <div className="navbar-links">
          <Link to="/" className="nav-link">
            <i className="fas fa-list"></i> Applications
          </Link>
          <Link to="/add" className="nav-link add">
            <i className="fas fa-plus"></i> Add New
          </Link>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;