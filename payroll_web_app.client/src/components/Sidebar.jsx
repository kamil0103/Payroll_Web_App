// src/components/Sidebar.jsx
import { Link } from "react-router-dom";
import { useState } from "react";
import "./Sidebar.css";

export default function Sidebar() {
  const [open, setOpen] = useState(false);

  return (
    <>
      {/* Toggle button */}
      <button className="sidebar-toggle" onClick={() => setOpen(!open)}>
        <i className="fas fa-bars"></i>
      </button>

      {/* Sidebar drawer */}
      <div className={`sidebar ${open ? "open" : ""}`}>
        <h3>Navigation</h3>
        <Link to="/" className="nav-link">Home</Link>
        <Link to="/logout" className="nav-link">Logout</Link>
      </div>

      {/* Overlay to close sidebar */}
      {open && <div className="sidebar-overlay" onClick={() => setOpen(false)}></div>}
    </>
  );
}
