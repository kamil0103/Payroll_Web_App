// src/pages/DashboardEmployer.jsx
import { useEffect, useState } from "react";
import { getEmployees } from "../services/api";
import EmployeeForm from "../components/EmployeeForm";
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
      <header className="dashboard-header">
        <h1 className="header-title">Payroll Management</h1>
      </header>

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

      <footer className="dashboard-footer">
        <p>© 2025 Payroll System. All rights reserved.</p>
      </footer>
    </div>
  );
}
