using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Payroll")]
public class Payroll
{
    public int PayrollId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime PayDate { get; set; }
    public decimal BasicPay { get; set; }
    public decimal? Bonus { get; set; }
    public decimal? Deductions { get; set; }
    public decimal NetPay { get; set; }
}
