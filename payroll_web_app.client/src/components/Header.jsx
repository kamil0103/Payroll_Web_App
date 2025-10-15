// src/components/Header.jsx
import { Link } from "react-router-dom";
import "./Header.css";

export default function Header() {
  return (
    <header className="shared-header">
      <h1 className="header-title">Payroll Management System</h1>
      <nav className="header-nav">
        <Link to="/" className="nav-link">Home</Link>
      </nav>
    </header>
  );
}
