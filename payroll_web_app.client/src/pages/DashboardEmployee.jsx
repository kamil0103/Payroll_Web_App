// src/pages/DashboardEmployee.jsx
import { useEffect, useRef, useMemo } from "react";
import Header from "../components/Header";
import Footer from "../components/Footer";
import Chart from "chart.js/auto";
import "./DashboardEmployee.css";

export default function DashboardEmployee() {
    const chartRef = useRef(null);
    const chartInstance = useRef(null);

    // Mock data
    const summaryData = {
        employees: { current: 7, previous: 1 },
        expenses: {
            labels: ["Aug", "Sep", "Oct", "Nov"],
            values: [410000, 405000, 420000, 450000],
        },
    };

    // -----------------------------
    // ESLint-safe memoization for nested arrays
    // -----------------------------
    const rawLabels = summaryData.expenses.labels;
    const rawValues = summaryData.expenses.values;

    const labelsKey = useMemo(() => rawLabels.join(","), [rawLabels]);
    const valuesKey = useMemo(() => rawValues.join(","), [rawValues]);

    const expenseLabels = useMemo(() => rawLabels, [labelsKey]);
    const expenseValues = useMemo(() => rawValues, [valuesKey]);

    // % change helper
    const calcPercentChange = (current, previous) => {
        if (previous === 0) return 0;
        return ((current - previous) / previous) * 100;
    };

    // -----------------------------
    // Chart.js effect
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
                            label: (ctx) =>
                                ` $${(ctx.parsed.y || 0).toLocaleString()}`,
                        },
                    },
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: "#64748b" } },
                    y: {
                        grid: { color: "rgba(148, 163, 184, 0.15)" },
                        ticks: {
                            color: "#64748b",
                            callback: (value) =>
                                "$" + Number(value).toLocaleString(),
                        },
                    },
                },
            },
        });
    }, [expenseLabels, expenseValues]);

    // -----------------------------
    // Employee card data
    // -----------------------------
    const empCurrent = summaryData.employees.current;
    const empPrevious = summaryData.employees.previous;
    const empChange = calcPercentChange(empCurrent, empPrevious);
    const empRounded = Math.round(empChange * 10) / 10;

    // -----------------------------
    // Expense card data
    // -----------------------------
    const expCurrent = expenseValues[expenseValues.length - 1];
    const expPrev = expenseValues[expenseValues.length - 2];
    const expChange = calcPercentChange(expCurrent, expPrev);
    const expRounded = Math.round(expChange * 10) / 10;

    return (
        <div className="employee-dashboard">
            <Header />

            <main className="employee-main" style={{ padding: "2rem" }}>
                <style>{`
                    * { box-sizing: border-box; }
                    body { margin: 0; font-family: system-ui; background: #f5f7fb; }

                    .dashboard {
                        display: grid;
                        grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
                        gap: 1.5rem;
                        margin-bottom: 2rem;
                    }

                    .card {
                        background: #fff;
                        border-radius: 16px;
                        padding: 1.5rem;
                        box-shadow: 0 6px 16px rgba(15, 23, 42, 0.08);
                        display: flex;
                        flex-direction: column;
                        gap: 0.5rem;
                    }

                    .card-header {
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                        font-size: 0.9rem;
                        color: #64748b;
                    }

                    .metric-value {
                        font-size: 2.1rem;
                        font-weight: 700;
                        color: #1e293b;
                    }

                    .metric-subtitle {
                        font-size: 0.9rem;
                        color: #94a3b8;
                    }

                    .trend {
                        display: inline-flex;
                        align-items: center;
                        gap: 0.25rem;
                        border-radius: 999px;
                        padding: 0.15rem 0.55rem;
                        font-size: 0.8rem;
                        font-weight: 600;
                    }

                    .trend.up { background: rgba(22, 163, 74, 0.08); color: #15803d; }
                    .trend.down { background: rgba(220, 38, 38, 0.08); color: #b91c1c; }

                    .chart-card {
                        padding: 1.5rem;
                        background: #fff;
                        border-radius: 16px;
                        box-shadow: 0 6px 16px rgba(15, 23, 42, 0.08);
                    }

                    .chart-title { font-size: 1rem; font-weight: 600; color: #1e293b; margin-bottom: 0.5rem; }
                    .chart-subtitle { font-size: 0.85rem; color: #94a3b8; margin-bottom: 1rem; }
                `}</style>

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
                            vs {expenseLabels[expenseLabels.length - 2]}: ${expPrev.toLocaleString()}
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
