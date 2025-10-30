using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Payroll")]
public class Payroll
{
    public int PayrollId { get; set; }
    public int EmployeeId { get; set; }

    [Column("PayDate")]      
    public DateTime PayDate { get; set; }

    [Column("GrossPay")]                
    public decimal BasicPay { get; set; } 

    [Column("BenefitsDeductions")]
    public decimal? Bonus { get; set; }    

    [Column("TaxDeductions")]
    public decimal? Deductions { get; set; }

    public decimal NetPay { get; set; }    
}
