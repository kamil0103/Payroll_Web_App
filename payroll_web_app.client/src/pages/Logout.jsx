// src/pages/Logout.jsx
import { useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./Logout.css";

export default function Logout() {
  const { setUser } = useContext(AuthContext);
  const navigate = useNavigate();

  useEffect(() => {
    setUser(null); // Clears context and localStorage
    setTimeout(() => {
      navigate("/");
    }, 1500);
  }, [setUser, navigate]);

  return (
    <div className="logout-layout">
      <Header />

      <main className="logout-main">
        <div className="logout-container">
          <h2>Logging out...</h2>
          <p>You’ll be redirected to the homepage shortly.</p>
        </div>
      </main>

      <Footer />
    </div>
  );
}
