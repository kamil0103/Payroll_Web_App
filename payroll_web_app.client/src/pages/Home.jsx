// src/pages/Home.jsx
import { Link } from "react-router-dom";
import "./Home.css";

export default function Home() {
  return (
    <div className="home-layout">
      <header className="home-header">
        <h1 className="header-title">Payroll Management System</h1>
      </header>

      <main className="home-main">
        <div className="home-container">
          <h2>Welcome to Payroll Management</h2>
          <p>Manage employee records, payroll, and more—all in one place.</p>
          <div className="home-buttons">
            <Link to="/signin" className="home-button">Sign In</Link>
            <Link to="/signup" className="home-button">Sign Up</Link>
          </div>
        </div>
      </main>

      <footer className="home-footer">
        <p>© 2025 Payroll System. All rights reserved.</p>
      </footer>
    </div>
  );
}
