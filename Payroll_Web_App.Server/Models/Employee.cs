using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Employee")]
public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? MiddleInitial { get; set; }
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public int DepartmentId { get; set; }
    public decimal Salary { get; set; }
}
