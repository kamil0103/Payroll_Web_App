// src/pages/Home.jsx
import { Link } from "react-router-dom";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./Home.css";

export default function Home() {
  return (
    <div className="home-layout">
      <Header />

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

      <Footer />
    </div>
  );
}
