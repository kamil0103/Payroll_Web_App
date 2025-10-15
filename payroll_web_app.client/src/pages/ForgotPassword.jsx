// src/pages/ForgotPassword.jsx
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./Forgot.css";

export default function ForgotPassword() {
  return (
    <div className="forgot-layout">
      <Header />

      <main className="forgot-main">
        <div className="forgot-container">
          <h2>Reset Your Password</h2>
          <p>Enter your registered email and we’ll send you instructions to reset your password.</p>
          <form className="forgot-form">
            <input type="email" placeholder="Email" required />
            <button type="submit">Send Reset Link</button>
          </form>
        </div>
      </main>

      <Footer />
    </div>
  );
}
