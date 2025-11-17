using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Payroll_Web_App.Server.Models;

namespace Payroll_Web_App.Server.Services
{
    public class PayrollCalculationService : IPayrollCalculationService
    {
        private readonly IConfiguration _config;

        public PayrollCalculationService(IConfiguration config)
        {
            _config = config;
        }

        public PayrollResult CalculatePayroll(
            Employee employee,
            IEnumerable<Attendance> attendance,
            DateTime periodStart,
            DateTime periodEnd)
        {
            // Load settings from appsettings.json 
            var section = _config.GetSection("PayrollSettings");
            var standardHours = section.GetValue<decimal>("StandardHours");       // 40
            var overtimeMultiplier = section.GetValue<decimal>("OvertimeMultiplier");  // 1.5
            var taxRate = section.GetValue<decimal>("TaxRate");             // 0.20
            var benefitsRate = section.GetValue<decimal>("BenefitsRate");        // 0.05
            var defaultHourlyRate = section.GetValue<decimal>("HourlyRateDefault");   // 35

            // Sum hours from attendance
            var totalHours = attendance
                .Where(a => a.Status == "Present")       // only worked days
                .Sum(a => a.HoursWorked ?? 0m);

            decimal grossPay;

            // Salary employees: BaseSalary > 0 => monthly base salary 
            if (employee.BaseSalary > 0)
            {
                // BaseSalary is monthly pay and the period is a full month
                grossPay = employee.BaseSalary;
            }
            else
            {
                // Hourly employees: use total hours + overtime rules
                var regularHours = Math.Min(totalHours, standardHours);
                var overtimeHours = Math.Max(0m, totalHours - standardHours);

                grossPay =
                    regularHours * defaultHourlyRate +
                    overtimeHours * defaultHourlyRate * overtimeMultiplier;
            }

            // Deductions & net pay 
            var tax = grossPay * taxRate;
            var benefits = grossPay * benefitsRate;
            var net = grossPay - tax - benefits;

            return new PayrollResult
            {
                PeriodStart = periodStart.Date,
                PeriodEnd = periodEnd.Date,
                TotalHours = totalHours,
                GrossPay = decimal.Round(grossPay, 2),
                TaxDeductions = decimal.Round(tax, 2),
                BenefitsDeductions = decimal.Round(benefits, 2),
                NetPay = decimal.Round(net, 2)
            };
        }
    }
}
