using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }          

        public int EmployeeId { get; set; }         

        public DateTime PayPeriodStart { get; set; }  
        public DateTime PayPeriodEnd { get; set; }    

        public decimal GrossPay { get; set; }          
        public decimal TaxDeductions { get; set; }     
        public decimal BenefitsDeductions { get; set; }
        public decimal NetPay { get; set; }            

        public DateTime? GeneratedAt { get; set; }
    }
}

