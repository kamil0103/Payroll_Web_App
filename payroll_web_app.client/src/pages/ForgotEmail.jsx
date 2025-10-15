// src/pages/ForgotEmail.jsx
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./Forgot.css";

export default function ForgotEmail() {
  return (
    <div className="forgot-layout">
      <Header />

      <main className="forgot-main">
        <div className="forgot-container">
          <h2>Recover Your Email</h2>
          <p>If you’ve forgotten your email, please contact your administrator or support team.</p>
        </div>
      </main>

      <Footer />
    </div>
  );
}
