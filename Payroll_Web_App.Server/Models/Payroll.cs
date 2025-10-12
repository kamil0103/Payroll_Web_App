using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Payroll")] // maps to dbo.Payroll
public class Payroll
{
    public long PayrollId { get; set; }      // PK (bigint)
    public int EmployeeId { get; set; }      // FK
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
    public DateTime PayDate { get; set; }
    public string? Notes { get; set; }
}
