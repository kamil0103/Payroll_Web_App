// src/pages/DashboardEmployer.jsx
import { useEffect, useState } from "react";
import { getEmployees } from "../services/api";
import EmployeeForm from "../components/EmployeeForm";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./DashboardEmployer.css";

export default function DashboardEmployer() {
  const [employees, setEmployees] = useState([]);

  useEffect(() => {
    async function fetchData() {
      try {
        const data = await getEmployees();
        setEmployees(data);
      } catch (error) {
        console.error("Error fetching employees:", error);
      }
    }
    fetchData();
  }, []);

  return (
    <div className="dashboard-layout">
      <Header />

      <main className="dashboard-main">
        <div className="dashboard-container">
          <EmployeeForm />
          <h2 className="employee-list-title">Employee List</h2>
          <ul className="employee-list">
            {employees.map((emp) => (
              <li key={emp.id} className="employee-item">
                {emp.name} – {emp.department} – ${emp.salary}
              </li>
            ))}
          </ul>
        </div>
      </main>

      <Footer />
    </div>
  );
}
