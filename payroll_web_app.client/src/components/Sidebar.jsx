// src/components/Sidebar.jsx
import { Link } from "react-router-dom";
import { useState } from "react";
import "./Sidebar.css";

export default function Sidebar() {
  const [open, setOpen] = useState(false);

  return (
    <>
      {/* Toggle button (hamburger) */}
      <button className="sidebar-toggle" onClick={() => setOpen(true)}>
        <i className="fas fa-bars"></i>
      </button>

      {/* Sidebar drawer */}
      <div className={`sidebar ${open ? "open" : ""}`}>
        {/* Close button inside sidebar */}
        <button className="sidebar-close" onClick={() => setOpen(false)}>
          <i className="fas fa-arrow-left"></i>
        </button>

        <h3>Navigation</h3>
        <Link to="/" className="nav-link">
          <i className="fas fa-home"></i> Home
        </Link>
        <Link to="/logout" className="nav-link">
          <i className="fas fa-sign-out-alt"></i> Logout
        </Link>
      </div>

      {/* Overlay to close sidebar */}
      {open && <div className="sidebar-overlay" onClick={() => setOpen(false)}></div>}
    </>
  );
}
