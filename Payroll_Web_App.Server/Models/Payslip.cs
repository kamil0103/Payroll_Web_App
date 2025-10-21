using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Payslip")]
public class Payslip
{
    public int PayslipId { get; set; }
    public int PayrollId { get; set; }
    public string Period { get; set; } = "";
    public string? PdfPath { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
}
