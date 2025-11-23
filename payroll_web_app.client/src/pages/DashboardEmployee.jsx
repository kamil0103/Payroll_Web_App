// src/pages/DashboardEmployee.jsx
import { useEffect, useState } from "react";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./DashboardEmployee.css";

export default function DashboardEmployee() {
  const [payroll, setPayroll] = useState([]);

  useEffect(() => {
    async function fetchPayroll() {
      try {
        const data = [
          { id: 1, month: "January", salary: 3200, status: "Paid" },
          { id: 2, month: "February", salary: 3200, status: "Paid" },
          { id: 3, month: "March", salary: 3200, status: "Pending" },
        ];
        setPayroll(data);
      } catch (error) {
        console.error("Error fetching payroll:", error);
      }
    }

    fetchPayroll();
  }, []);

  return (
    <div className="employee-dashboard">
      <Header />

      <main className="employee-main">
        <div className="payroll-container">
          <h2>My Payroll Dashboard</h2>
          <table className="payroll-table">
            <thead>
              <tr>
                <th>Month</th>
                <th>Salary</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {payroll.map((entry) => (
                <tr key={entry.id}>
                  <td>{entry.month}</td>
                  <td>${entry.salary}</td>
                  <td>{entry.status}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </main>

      <Footer />
    </div>
  );
}

