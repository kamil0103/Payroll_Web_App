// services/api.js
const API_BASE = "http://localhost:5000/api"; // Adjust if your backend runs on a different port

export const getEmployees = async () => {
  const response = await fetch(`${API_BASE}/employee`);
  return response.json();
};

export const addEmployee = async (employeeData) => {
  const response = await fetch(`${API_BASE}/employee`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(employeeData),
  });
  return response.json();
};
