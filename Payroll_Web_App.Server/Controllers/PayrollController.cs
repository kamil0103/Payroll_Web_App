using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll_Web_App.Server.Data;
using Payroll_Web_App.Server.Models;
using Payroll_Web_App.Server.Services;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Payroll_Web_App.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPayrollCalculationService _calculator;

        public PayrollController(AppDbContext db, IPayrollCalculationService calculator)
        {
            _db = db;
            _calculator = calculator;
        }

        // ===============================================================
        // Generate payroll for an employee for a given period
        // POST: /payroll/generate
        // Body: { "employeeId": 1, "periodStart": "2025-10-01", "periodEnd": "2025-10-31" }
        // Access: Admin, HR
        // ===============================================================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GeneratePayrollRequest request)
        {
            if (request.EmployeeId <= 0)
                return BadRequest(new { message = "EmployeeId is required." });

            if (request.PeriodEnd < request.PeriodStart)
                return BadRequest(new { message = "PeriodEnd must be on or after PeriodStart." });

            var employee = await _db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId);

            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            // Load attendance for the period 
            var attendance = await _db.Attendance
                .AsNoTracking()
                .Where(a =>
                    a.EmployeeId == request.EmployeeId &&
                    a.Date >= request.PeriodStart.Date &&
                    a.Date <= request.PeriodEnd.Date)
                .ToListAsync();

            // calculate
            var payrollResult = _calculator.CalculatePayroll(employee,
                                                            attendance,
                                                            request.PeriodStart,
                                                            request.PeriodEnd);

            // total hours from attendance (sum nullable HoursWorked as 0 when null)
            var totalHours = attendance.Sum(a => a.HoursWorked ?? 0m);

            

            // Map result into Payroll entity
            var payroll = new Payroll
            {
                EmployeeId = request.EmployeeId,
                PayPeriodStart = request.PeriodStart.Date,
                PayPeriodEnd = request.PeriodEnd.Date,

                GrossPay = payrollResult.GrossPay,
                BenefitsDeductions = payrollResult.BenefitsDeductions,
                TaxDeductions = payrollResult.TaxDeductions,
                NetPay = payrollResult.NetPay,
                GeneratedAt = DateTime.UtcNow
            };

            _db.Payroll.Add(payroll);
            await _db.SaveChangesAsync();

            // reload payroll including Employee to ensure navigation is populated before returning
            var payrollWithEmployee = await _db.Payroll
                .AsNoTracking()
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollId == payroll.PayrollId);

            // return the full payroll (includes Employee when available) and the computed total hours 
            return Ok(new
            {
                PayslipId = payrollWithEmployee.PayrollId, // explicit payslip id
                payrollWithEmployee.PayrollId,
                payrollWithEmployee.EmployeeId,
                payrollWithEmployee.PayPeriodStart,
                payrollWithEmployee.PayPeriodEnd,
                payrollWithEmployee.GrossPay,
                payrollWithEmployee.BenefitsDeductions,
                payrollWithEmployee.TaxDeductions,
                payrollWithEmployee.NetPay,
                payrollWithEmployee.GeneratedAt,
                TotalHours = totalHours,
                Employee = payrollWithEmployee.Employee == null ? null : new
                {
                    payrollWithEmployee.Employee.EmployeeId,
                    payrollWithEmployee.Employee.FirstName,
                    payrollWithEmployee.Employee.LastName,
                    payrollWithEmployee.Employee.Department,
                    payrollWithEmployee.Employee.JobTitle
                }
            });
        }

        // ===============================================================
        //  Get payroll by id
        // GET: /payroll/{payrollId}
        // Access: Admin, HR, Finance
        // ===============================================================
        [Authorize(Roles = "Admin,HR,Finance")]
        [HttpGet("{payrollId:int}")]
        public async Task<IActionResult> GetById(int payrollId)
        {
            var payroll = await _db.Payroll
                .AsNoTracking()
                .Include(p => p.Employee) // ensure Employee navigation is populated
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);

            if (payroll == null)
                return NotFound(new { message = "Payroll record not found." });

            return Ok(payroll);
        }

        // ===============================================================
        // List payroll records for an employee (optional date range)
        // GET: /payroll/by-employee/{employeeId}?from=2025-10-01&to=2025-10-31
        // Access: Admin, HR, Finance
        // ===============================================================
        [Authorize(Roles = "Admin,HR,Finance")]
        [HttpGet("by-employee/{employeeId:int}")]
        public async Task<IActionResult> ByEmployee(
            int employeeId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = _db.Payroll
                .AsNoTracking()
                .Where(p => p.EmployeeId == employeeId);

            if (from.HasValue)
                query = query.Where(p => p.PayPeriodEnd >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(p => p.PayPeriodEnd <= to.Value.Date);

            var list = await query
                .OrderByDescending(p => p.PayPeriodEnd)
                .ThenByDescending(p => p.PayrollId)
                .ToListAsync();

            return Ok(list);
        }

        // ===============================================================
        // Get detailed payslip information
        // GET: /payroll/{id}/payslip
        // Access: Admin, HR, Finance, Employee
        // ===============================================================
        [Authorize(Roles = "Admin,HR,Finance,Employee")]
        [HttpGet("{id}/payslip")]
        public async Task<IActionResult> GetPayslip(int id)
        {
            var payroll = await _db.Payroll
                .AsNoTracking()
                .Include(p => p.Employee) // assumes Payroll has navigation property or FK; EF will still work if configured
                .FirstOrDefaultAsync(p => p.PayrollId == id);

            if (payroll == null) return NotFound(new { message = "Payroll record not found." });

            // If Employee navigation isn't available, load explicitly
            if (payroll.Employee == null)
            {
                payroll.Employee = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == payroll.EmployeeId);
            }

            // compute total hours from Attendance table for this pay period
            var totalHours = await _db.Attendance
                .AsNoTracking()
                .Where(a => a.EmployeeId == payroll.EmployeeId &&
                            a.Date >= payroll.PayPeriodStart.Date &&
                            a.Date <= payroll.PayPeriodEnd.Date)
                .Select(a => a.HoursWorked ?? 0m)
                .SumAsync();

            var dto = new PayslipDto
            {
                PayslipId = payroll.PayrollId, // new explicit payslip id
                PayrollId = payroll.PayrollId,
                EmployeeId = payroll.EmployeeId,
                EmployeeName = $"{payroll.Employee?.FirstName} {payroll.Employee?.LastName}".Trim(),
                JobTitle = payroll.Employee?.JobTitle,
                Department = payroll.Employee?.Department,
                PayPeriodStart = payroll.PayPeriodStart,
                PayPeriodEnd = payroll.PayPeriodEnd,
                TotalHours = totalHours,
                GrossPay = payroll.GrossPay,
                TaxDeductions = payroll.TaxDeductions,
                BenefitsDeductions = payroll.BenefitsDeductions,
                NetPay = payroll.NetPay,
                GeneratedAt = payroll.GeneratedAt ?? DateTime.MinValue // preserve previous null-handling
            };

            return Ok(dto);
        }

        // ===============================================================
        // Get payslip in HTML format
        // GET: /payroll/{id}/payslip/html
        // Access: Admin, HR, Finance, Employee
        // ===============================================================
        [Authorize(Roles = "Admin,HR,Finance,Employee")]
        [HttpGet("{id}/payslip/html")]
        public async Task<IActionResult> GetPayslipHtml(int id)
        {
            var payroll = await _db.Payroll
                .AsNoTracking()
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollId == id);

            if (payroll == null) return NotFound("Payroll not found.");

            if (payroll.Employee == null)
            {
                payroll.Employee = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == payroll.EmployeeId);
            }

            // compute total hours for HTML payslip
            var totalHours = await _db.Attendance
                .AsNoTracking()
                .Where(a => a.EmployeeId == payroll.EmployeeId &&
                            a.Date >= payroll.PayPeriodStart.Date &&
                            a.Date <= payroll.PayPeriodEnd.Date)
                .Select(a => a.HoursWorked ?? 0m)
                .SumAsync();

            var employeeName = $"{payroll.Employee?.FirstName} {payroll.Employee?.LastName}".Trim();
            var generatedAtStr = payroll.GeneratedAt.HasValue ? payroll.GeneratedAt.Value.ToString("yyyy-MM-dd HH:mm") : "";

            var html = new StringBuilder();
            html.Append("<!doctype html><html><head><meta charset='utf-8'>");
            html.Append("<style>");
            html.Append("body{font-family: Arial, Helvetica, sans-serif; margin:20px; color:#222}");
            html.Append(".header{display:flex;justify-content:space-between;align-items:center}");
            html.Append(".box{border:1px solid #ddd;padding:12px;margin-top:12px}");
            html.Append("table{width:100%;border-collapse:collapse} td, th{padding:8px;border-bottom:1px solid #eee}");
            html.Append(".right{text-align:right}");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append($"<div class='header'><h2>Payslip</h2><div>Generated: {System.Net.WebUtility.HtmlEncode(generatedAtStr)}</div></div>");
            html.Append($"<div class='box'><strong>Employee:</strong> {System.Net.WebUtility.HtmlEncode(employeeName)}<br/>");
            html.Append($"<strong>Department:</strong> {System.Net.WebUtility.HtmlEncode(payroll.Employee?.Department ?? "")} ");
            html.Append($"<strong>Job Title:</strong> {System.Net.WebUtility.HtmlEncode(payroll.Employee?.JobTitle ?? "")}</div>");

            html.Append("<div class='box'><table>");
            html.Append("<tr><th>Pay Period</th><td class='right'>" + $"{payroll.PayPeriodStart:yyyy-MM-dd} → {payroll.PayPeriodEnd:yyyy-MM-dd}" + "</td></tr>");
            html.Append("<tr><th>Total Hours</th><td class='right'>" + System.Net.WebUtility.HtmlEncode(totalHours.ToString("0.##")) + "</td></tr>");
            html.Append("<tr><th>Gross Pay</th><td class='right'>" 
                + payroll.GrossPay.ToString("C", new CultureInfo("en-US")) + "</td></tr>");
            
            html.Append("<tr><th>Tax</th><td class='right'>-" 
                + payroll.TaxDeductions.ToString("C", new CultureInfo("en-US")) + "</td></tr>");
            
            html.Append("<tr><th>Benefits</th><td class='right'>-" 
                + payroll.BenefitsDeductions.ToString("C", new CultureInfo("en-US")) + "</td></tr>");
            
            html.Append("<tr><th><strong>Net Pay</strong></th><td class='right'><strong>" 
                + payroll.NetPay.ToString("C", new CultureInfo("en-US")) + "</strong></td></tr>");
            html.Append("</table></div>");

            html.Append("</body></html>");

            return Content(html.ToString(), "text/html", Encoding.UTF8);
        }
    }


    public class GeneratePayrollRequest
    {
        public int EmployeeId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class PayslipDto
    {
        // explicit payslip id (alias for PayrollId)
        public int PayslipId { get; set; }

        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public decimal TotalHours { get; set; } = 0;

    

        public decimal GrossPay { get; set; }
        public decimal TaxDeductions { get; set; }
        public decimal BenefitsDeductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
