using System;

namespace Payroll_Web_App.Server.Models
{
    public class PayrollResult
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public decimal TotalHours { get; set; }

        public decimal GrossPay { get; set; }
        public decimal TaxDeductions { get; set; }
        public decimal BenefitsDeductions { get; set; }
        public decimal NetPay { get; set; }
    }
}

