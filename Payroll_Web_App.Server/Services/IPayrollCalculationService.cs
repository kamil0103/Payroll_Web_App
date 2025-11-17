using System;
using System.Collections.Generic;
using Payroll_Web_App.Server.Models;

namespace Payroll_Web_App.Server.Services
{
    public interface IPayrollCalculationService
    {
        
        // Calculate payroll for one employee in a given period, using attendance rows.

        PayrollResult CalculatePayroll(
            Employee employee,
            IEnumerable<Attendance> attendance,
            DateTime periodStart,
            DateTime periodEnd);
    }
}
