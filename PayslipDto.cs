using System;

namespace Payroll_Web_App.Server.Models
{
    public class PayslipDto
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public string? Department { get; set; }

        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }

        public decimal TotalHours { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TaxDeductions { get; set; }
        public decimal BenefitsDeductions { get; set; }
        public decimal NetPay { get; set; }

        public DateTime GeneratedAt { get; set; }
    }
}