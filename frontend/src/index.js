import React from 'react';
import ReactDOM from 'react-dom/client';
import App from '../src/App';
import './index.css';

// Create root element
const root = ReactDOM.createRoot(document.getElementById('root'));

// Render the app
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);