// src/pages/SignIn.jsx
import { useContext, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./SignIn.css";

export default function SignIn() {
  const { setUser } = useContext(AuthContext);
  const navigate = useNavigate();
  const [role, setRole] = useState("employee");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  function handleSubmit(e) {
    e.preventDefault();
    if (email && password) {
      setUser({ role });
      navigate(`/dashboard/${role}`);
    } else {
      alert("Please enter email and password.");
    }
  }

  return (
    <div className="signin-layout">
      <Header />

      <main className="signin-main">
        <div className="signin-container">
          <h2 className="form-title">Login</h2>
          <form className="signin-form" onSubmit={handleSubmit}>
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />

            <div className="forgot-links">
              <span className="forgot-dot">•</span>
              <Link to="/forgot-email" className="forgot-link">Forgot Email</Link>
              <span className="forgot-dot">•</span>
              <Link to="/forgot-password" className="forgot-link">Forgot Password</Link>
            </div>

            <div className="role-select">
              <label>
                <input
                  type="radio"
                  name="role"
                  value="employee"
                  checked={role === "employee"}
                  onChange={() => setRole("employee")}
                />
                Employee
              </label>
              <label>
                <input
                  type="radio"
                  name="role"
                  value="employer"
                  checked={role === "employer"}
                  onChange={() => setRole("employer")}
                />
                Employer
              </label>
            </div>

            <button type="submit">Log In</button>
          </form>
        </div>
      </main>

      <Footer />
    </div>
  );
}

