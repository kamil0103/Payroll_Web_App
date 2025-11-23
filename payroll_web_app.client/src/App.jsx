// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "./context/AuthContext";
import Home from "./pages/Home";
import SignIn from "./pages/SignIn";
import SignUp from "./pages/SignUp";
import DashboardEmployer from "./pages/DashboardEmployer";
import DashboardEmployee from "./pages/DashboardEmployee";
import Logout from "./pages/Logout";
import ForgotEmail from "./pages/ForgotEmail";
import ForgotPassword from "./pages/ForgotPassword";
import "./App.css";

// Role-based protection
function ProtectedRoute({ children, role }) {
  const { user } = useContext(AuthContext);
  if (!user || user.role !== role) return <Navigate to="/signin" />;
  return children;
}

export default function App() {
  return (
    <Router>
      <Routes>
        {/* ✅ Default landing page */}
        <Route path="/" element={<Home />} />
        <Route index element={<Home />} />

        {/* 🔐 Auth pages */}
        <Route path="/signin" element={<SignIn />} />
        <Route path="/signup" element={<SignUp />} />
        <Route path="/logout" element={<Logout />} />

        {/* 🔁 Recovery pages */}
        <Route path="/forgot-email" element={<ForgotEmail />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />

        {/* 🧑‍💼 Employee dashboard */}
        <Route
          path="/dashboard/employee"
          element={
            <ProtectedRoute role="employee">
              <DashboardEmployee />
            </ProtectedRoute>
          }
        />

        {/* 👩‍💼 Employer dashboard */}
        <Route
          path="/dashboard/employer"
          element={
            <ProtectedRoute role="employer">
              <DashboardEmployer />
            </ProtectedRoute>
          }
        />

        {/* 🚫 Catch-all fallback */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </Router>
  );
}
