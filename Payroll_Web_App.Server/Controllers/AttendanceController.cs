using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Payroll_Web_App.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AttendanceController(IConfiguration config) => _config = config;

        // ======================================
        // Add attendance record
        // Purpose: Insert a new attendance entry 
        // Access: Admin, HR, Employee 
        // ======================================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("add")]
        public IActionResult Add([FromBody] AttendanceCreateDto dto)
        {
            // validation 
            if (dto.EmployeeId <= 0) return BadRequest("EmployeeId is required.");
            if (dto.HoursWorked is < 0) return BadRequest("HoursWorked must be >= 0.");
            if (!AttendanceStatusHelper.IsValid(dto.Status)) return BadRequest("Status must be Present, Absent, or Leave.");

            var cs = _config.GetConnectionString("DefaultConnection");
            using var con = new SqlConnection(cs);
            con.Open();

            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Attendance (EmployeeID, [Date], HoursWorked, [Status], CreatedAt)
                VALUES (@empId, @date, @hours, @status, GETDATE());
                SELECT SCOPE_IDENTITY();
            ", con);

            cmd.Parameters.Add("@empId", SqlDbType.Int).Value = dto.EmployeeId;

            // date
            var pDate = cmd.Parameters.Add("@date", SqlDbType.Date); 
            pDate.Value = dto.Date.Date;

            var pHours = cmd.Parameters.Add("@hours", SqlDbType.Decimal);
            pHours.Precision = 9; pHours.Scale = 2;
            pHours.Value = dto.HoursWorked is null ? DBNull.Value : dto.HoursWorked;

            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = (object?)dto.Status ?? DBNull.Value;

            try
            {
                var newId = Convert.ToInt32(cmd.ExecuteScalar());
                return Ok(new { attendanceId = newId });
            }
            catch (SqlException ex) when (ex.Number == 547) // FK violation
            {
                return BadRequest(new { message = "EmployeeId does not exist.", sqlError = 547 });
            }
        }

        // =============================================
        // Update attendance record
        // Purpose: Modify hours worked, status, or date
        // Access: Admin, HR
        // =============================================



        [Authorize(Roles = "Admin,HR")]
        [HttpPost("update")]
        public IActionResult Update([FromBody] AttendanceUpdateDto dto)
        {
            if (dto.AttendanceId <= 0) return BadRequest("AttendanceId is required.");
            if (dto.HoursWorked is < 0) return BadRequest("HoursWorked must be >= 0.");
            if (dto.Status is not null && !AttendanceStatusHelper.IsValid(dto.Status))
                return BadRequest("Status must be Present, Absent, or Leave.");

            var cs = _config.GetConnectionString("DefaultConnection");
            using var con = new SqlConnection(cs);
            con.Open();

            using var cmd = new SqlCommand(@"
                UPDATE dbo.Attendance
                   SET HoursWorked = @hours,
                       [Status]    = @status
                 WHERE AttendanceID = @id;
            ", con);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = dto.AttendanceId;

            var pHours = cmd.Parameters.Add("@hours", SqlDbType.Decimal);
            pHours.Precision = 9; pHours.Scale = 2;
            pHours.Value = dto.HoursWorked is null ? DBNull.Value : dto.HoursWorked;

            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = (object?)dto.Status ?? DBNull.Value;

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok(new { message = "Updated." }) : NotFound(new { message = "Not found." });
        }

        // ================================
        // Delete attendance record
        // Purpose: Remove attendance entry
        // Access: Admin, HR
        // ================================

        [Authorize(Roles = "Admin,HR")]
        [HttpDelete("{attendanceId:int}")]
        public IActionResult Delete(int attendanceId)
        {
            var cs = _config.GetConnectionString("DefaultConnection");
            using var con = new SqlConnection(cs);
            con.Open();

            using var cmd = new SqlCommand("DELETE FROM dbo.Attendance WHERE AttendanceID = @id;", con);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = attendanceId;
            var rows = cmd.ExecuteNonQuery();

            return rows > 0 ? Ok(new { message = "Deleted." }) : NotFound(new { message = "Not found." });
        }

        // ============================
        // List attendance records
        // Purpose: View all attendance
        // Access: Admin, HR
        // ============================

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("by-employee/{employeeId:int}")]
        public IActionResult ByEmployee(int employeeId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var cs = _config.GetConnectionString("DefaultConnection");
            using var con = new SqlConnection(cs);
            con.Open();

            var sql = @"
                SELECT AttendanceID, EmployeeID, [Date], HoursWorked, [Status], CreatedAt
                FROM dbo.Attendance
                WHERE EmployeeID = @empId";
            if (from.HasValue) sql += " AND [Date] >= @from";
            if (to.HasValue) sql += " AND [Date] <  @to";
            sql += " ORDER BY [Date] DESC, AttendanceID DESC;";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@empId", SqlDbType.Int).Value = employeeId;
            if (from.HasValue) cmd.Parameters.Add("@from", SqlDbType.Date).Value = from.Value.Date;
            if (to.HasValue) cmd.Parameters.Add("@to", SqlDbType.Date).Value = to.Value.Date;

            var list = new List<object>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new
                {
                    attendanceId = Convert.ToInt32(r["AttendanceID"]),
                    employeeId = Convert.ToInt32(r["EmployeeID"]),
                    date = Convert.ToDateTime(r["Date"]).Date,
                    hoursWorked = r["HoursWorked"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HoursWorked"]),
                    status = r["Status"] == DBNull.Value ? null : r["Status"]!.ToString(),
                    createdAt = Convert.ToDateTime(r["CreatedAt"])
                });
            }
            return Ok(list);
        }

        // ===============================================
        // List attendance by employee
        // Purpose: View attendance history for a employee
        // Access: Admin, HR, Employee 
        // ===============================================
        [HttpGet("mine/{employeeId:int}")]
        public IActionResult Mine(int employeeId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var isPrivileged = User.IsInRole("Admin") || User.IsInRole("HR");
            if (!isPrivileged)
            {
                var claimEmployeeId = User.FindFirst("EmployeeId")?.Value;
                if (!int.TryParse(claimEmployeeId, out var authenticatedEmployeeId))
                    return Forbid();

                if (authenticatedEmployeeId != employeeId)
                    return Forbid();
            }

            var cs = _config.GetConnectionString("DefaultConnection");
            using var con = new SqlConnection(cs);
            con.Open();

            var sql = @"
                SELECT AttendanceID, EmployeeID, [Date], HoursWorked, [Status], CreatedAt
                FROM dbo.Attendance
                WHERE EmployeeID = @empId";
            if (from.HasValue) sql += " AND [Date] >= @from";
            if (to.HasValue) sql += " AND [Date] <  @to";
            sql += " ORDER BY [Date] DESC, AttendanceID DESC;";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@empId", SqlDbType.Int).Value = employeeId;
            if (from.HasValue) cmd.Parameters.Add("@from", SqlDbType.Date).Value = from.Value.Date;
            if (to.HasValue) cmd.Parameters.Add("@to", SqlDbType.Date).Value = to.Value.Date;

            var list = new List<object>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new
                {
                    attendanceId = Convert.ToInt32(r["AttendanceID"]),
                    employeeId = Convert.ToInt32(r["EmployeeID"]),
                    date = Convert.ToDateTime(r["Date"]).Date,
                    hoursWorked = r["HoursWorked"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HoursWorked"]),
                    status = r["Status"] == DBNull.Value ? null : r["Status"]!.ToString(),
                    createdAt = Convert.ToDateTime(r["CreatedAt"])
                });
            }

            return Ok(list);
        }
    }

    // Helpers (inline for now but can be brought out)

    public class AttendanceCreateDto
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date; 
        public decimal? HoursWorked { get; set; }
        public string? Status { get; set; } // Present, Absent, Leave
    }

    public class AttendanceUpdateDto
    {
        public int AttendanceId { get; set; } 
        public decimal? HoursWorked { get; set; }
        public string? Status { get; set; }  // Present, Absent, Leave
    }

    public static class AttendanceStatusHelper
    {
        public static bool IsValid(string? s) =>
            s is "Present" or "Absent" or "Leave";
    }
}
