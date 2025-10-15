// src/pages/SignUp.jsx
import { useState } from "react";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./SignUp.css";

export default function SignUp() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  function handleSubmit(e) {
    e.preventDefault();
    console.log("Signing up:", { email, password });
    // Add registration logic here
  }

  return (
    <div className="signup-layout">
      <Header />

      <main className="signup-main">
        <div className="signup-container">
          <h2 className="form-title">Create an Account</h2>
          <form className="signup-form" onSubmit={handleSubmit}>
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
            <button type="submit">Sign Up</button>
          </form>
        </div>
      </main>

      <Footer />
    </div>
  );
}

