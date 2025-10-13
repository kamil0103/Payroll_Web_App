// src/components/EmployeeForm.jsx
import { useState } from "react";
import { addEmployee } from "../services/api";
import "./EmployeeForm.css";

export default function EmployeeForm() {
  const [form, setForm] = useState({
    name: "",
    department: "",
    salary: "",
  });

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await addEmployee(form);
      alert("Employee added successfully!");
      setForm({ name: "", department: "", salary: "" });
    } catch (error) {
      console.error("Error adding employee:", error);
      alert("Failed to add employee.");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Add Employee</h2>
      <input
        type="text"
        name="name"
        placeholder="Name"
        value={form.name}
        onChange={handleChange}
        required
      />
      <input
        type="text"
        name="department"
        placeholder="Department"
        value={form.department}
        onChange={handleChange}
        required
      />
      <input
        type="number"
        name="salary"
        placeholder="Salary"
        value={form.salary}
        onChange={handleChange}
        required
      />
      <button type="submit">Submit</button>
    </form>
  );
}
