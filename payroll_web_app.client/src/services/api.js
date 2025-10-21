// services/api.js
const API_BASE = import.meta.env.VITE_API_URL || "https://localhost:56644/api";

// ===== EMPLOYEES =====
export const getEmployees = async () => {
    const response = await fetch(`${API_BASE}/Employees`);
    if (!response.ok) throw new Error("Failed to fetch employees");
    return response.json();
};

export const addEmployee = async (employeeData) => {
    const response = await fetch(`${API_BASE}/Employees`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(employeeData),
    });
    if (!response.ok) throw new Error("Failed to add employee");
    return response.json();
};

// ===== USERS =====
export const getUsers = async () => {
    const response = await fetch(`${API_BASE}/Users`);
    if (!response.ok) throw new Error("Failed to fetch users");
    return response.json();
};

export const addUser = async (userData) => {
    const response = await fetch(`${API_BASE}/Users`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(userData),
    });
    if (!response.ok) throw new Error("Failed to add user");
    return response.json();
};
