/* src/components/Header.jsx */
import Sidebar from "./Sidebar";
import "./Header.css";

export default function Header() {
  return (
    <header className="shared-header">
      {/* Sidebar toggle on the left */}
      <div className="header-left">
        <Sidebar />
      </div>

      {/* Centered title */}
      <h1 className="header-title">Payroll Management System</h1>

      {/* Search bar on the right */}
      <div className="header-right">
        <div className="search-container">
          <input
            type="text"
            className="header-search"
            placeholder="Search..."
          />
          <i className="fas fa-search search-icon"></i> {/* ✅ icon inside */}
        </div>
      </div>
    </header>
  );
}
