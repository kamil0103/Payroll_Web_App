// src/pages/DashboardEmployee.jsx
import { useEffect, useRef, useMemo } from "react";
import Header from "../components/Header";
import Footer from "../components/Footer";
import Chart from "chart.js/auto";
import "./DashboardEmployee.css";

export default function DashboardEmployee() {
  const chartRef = useRef(null);
  const chartInstance = useRef(null);

  // -----------------------------
  // Mock Data
  // -----------------------------
  const summaryData = {
    employees: { current: 7, previous: 1 },
    expenses: {
      labels: ["Aug", "Sep", "Oct", "Nov"],
      values: [410000, 405000, 420000, 450000],
    },
  };

  // -----------------------------
  // Memoized Labels & Values (fixed dependencies)
  // -----------------------------
  const rawLabels = summaryData.expenses.labels;
  const rawValues = summaryData.expenses.values;

  const expenseLabels = useMemo(() => rawLabels, [rawLabels]);
  const expenseValues = useMemo(() => rawValues, [rawValues]);

  // -----------------------------
  // Helper: % Change
  // -----------------------------
  const calcPercentChange = (current, previous) => {
    if (previous === 0) return 0;
    return ((current - previous) / previous) * 100;
  };

  // -----------------------------
  // Chart.js Effect
  // -----------------------------
  useEffect(() => {
    if (!chartRef.current) return;

    const ctx = chartRef.current.getContext("2d");

    if (chartInstance.current) {
      chartInstance.current.destroy();
    }

    chartInstance.current = new Chart(ctx, {
      type: "line",
      data: {
        labels: expenseLabels,
        datasets: [
          {
            label: "Payroll expenses",
            data: expenseValues,
            fill: true,
            backgroundColor: "transparent",
            borderColor: "rgba(37, 99, 235, 1)",
            tension: 0.35,
            pointRadius: 3,
            pointBackgroundColor: "rgba(37, 99, 235, 1)",
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: (ctx) => ` $${(ctx.parsed.y || 0).toLocaleString()}`,
            },
          },
        },
        scales: {
          x: {
            grid: { display: false },
            ticks: { color: "#64748b" },
          },
          y: {
            grid: { color: "rgba(148, 163, 184, 0.15)" },
            ticks: {
              color: "#64748b",
              callback: (value) => "$" + Number(value).toLocaleString(),
            },
          },
        },
      },
    });
  }, [expenseLabels, expenseValues]);

  // -----------------------------
  // Employee Card Data
  // -----------------------------
  const empCurrent = summaryData.employees.current;
  const empPrevious = summaryData.employees.previous;
  const empChange = calcPercentChange(empCurrent, empPrevious);
  const empRounded = Math.round(empChange * 10) / 10;

  // -----------------------------
  // Expense Card Data
  // -----------------------------
  const expCurrent = expenseValues[expenseValues.length - 1];
  const expPrev = expenseValues[expenseValues.length - 2];
  const expChange = calcPercentChange(expCurrent, expPrev);
  const expRounded = Math.round(expChange * 10) / 10;

  // -----------------------------
  // Render
  // -----------------------------
  return (
    <div className="employee-dashboard">
      <Header />

      <main className="employee-main">
        {/* Dashboard Cards */}
        <div className="dashboard">
          {/* Employee Count Card */}
          <div className="card">
            <div className="card-header">
              <span>Employee Count</span>
              <span className={`trend ${empChange >= 0 ? "up" : "down"}`}>
                <span className="arrow">{empChange >= 0 ? "↑" : "↓"}</span>
                <span className="trend-text">
                  {empChange >= 0 ? "+" : ""}
                  {empRounded}% vs last month
                </span>
              </span>
            </div>
            <div className="metric-value">{empCurrent}</div>
            <div className="metric-subtitle">Active employees</div>
          </div>

          {/* Payroll Expenses Card */}
          <div className="card">
            <div className="card-header">
              <span>Total Payroll Expenses</span>
              <span className={`trend ${expChange >= 0 ? "up" : "down"}`}>
                <span className="arrow">{expChange >= 0 ? "↑" : "↓"}</span>
                <span className="trend-text">
                  {expChange >= 0 ? "+" : ""}
                  {expRounded}% vs last month
                </span>
              </span>
            </div>
            <div className="metric-value">${expCurrent.toLocaleString()}</div>
            <div className="metric-subtitle">
              vs {expenseLabels[expenseLabels.length - 2]}: $
              {expPrev.toLocaleString()}
            </div>
          </div>
        </div>

        {/* Chart Section */}
        <div className="chart-card">
          <div className="chart-title">Payroll Expenses (Last 6 Months)</div>
          <div className="chart-subtitle">
            Monthly gross payroll including bonuses & deductions
          </div>
          <canvas ref={chartRef} height="110"></canvas>
        </div>
      </main>

      <Footer />
    </div>
  );
}
