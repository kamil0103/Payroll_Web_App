using System;

namespace Payroll_Web_App.Server.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string? Department { get; set; }
        public string? JobTitle { get; set; }

        public decimal BaseSalary { get; set; }

        public DateTime HireDate { get; set; }

        public string? Email { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
