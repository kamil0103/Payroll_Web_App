using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Employees")] // maps to dbo.Employees
public class Employee
{
    public int EmployeeId { get; set; }      
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
}
