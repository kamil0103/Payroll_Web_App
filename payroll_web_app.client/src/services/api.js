// services/api.js
const API_BASE = import.meta.env.VITE_API_URL || "https://localhost:56644/Users";

export const getEmployees = async () => {
    const response = await fetch(`${API_BASE}/getusers`);
  return response.json();
};

export const addEmployee = async (employeeData) => {
    const response = await fetch(`${API_BASE}/adduser`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(employeeData),
  });
  return response.json();
};
