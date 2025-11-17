using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll_Web_App.Server.Data;          
using Payroll_Web_App.Server.Models;        
using Payroll_Web_App.Server.Services;     

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

            return Ok(new
            {
                payroll.PayrollId,
                payroll.EmployeeId,
                payroll.PayPeriodStart,
                payroll.PayPeriodEnd,
                payroll.GrossPay,
                payroll.BenefitsDeductions,
                payroll.TaxDeductions,
                payroll.NetPay
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
    }

    
    public class GeneratePayrollRequest
    {
        public int EmployeeId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
