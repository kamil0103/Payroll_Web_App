using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Payroll_Web_App.Server.Models;
using System.Data;

namespace Payroll_Web_App.Server.Controllers
{
    [Authorize] // jwt validation needed
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ====================================================================
        // endpoint to verify your token/role via Swagger
        // GET: /users/me
        // ====================================================================
        [HttpGet("me")]
        public IActionResult Me()
        {
            var name = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/name"))?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value;
            var id = User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;
            return Ok(new { id, name, role });
        }


        // ==========================
        // Admin & HR can list users
        // GET: /users/getusers
        // ==========================
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            var users = new List<AppUser>();

            using var connection = new SqlConnection(cs);
            connection.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    UserID, EmployeeId, Username, PasswordHash, Role, Email, IsActive, CreatedAt
                FROM dbo.Users
                ORDER BY CreatedAt DESC;", connection);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var u = new AppUser
                {
                    UserId = reader["UserID"] is int i ? i : Convert.ToInt32(reader["UserID"]),
                    EmployeeId = reader["EmployeeId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["EmployeeId"]),
                    UserName = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty, // replace with Hash
                    Role = reader["Role"]?.ToString() ?? "Employee",
                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"]!.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["CreatedAt"])
                };
                users.Add(u);
            }

            return Ok(users);
        }

        // =============================
        // Admin & HR can update a user
        // POST: /users/updateuser
        // =============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("updateuser")]
        public IActionResult UpdateUser([FromBody] AppUser dto)
        {
            if (dto.UserId <= 0)
                return BadRequest(new { message = "UserId is required." });

            var cs = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(cs);
            connection.Open();

            
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Users
                   SET Username = @username,
                       Role     = @role,
                       IsActive = @isActive
                 WHERE UserID   = @id;", connection);

            cmd.Parameters.AddWithValue("@id", dto.UserId);
            cmd.Parameters.AddWithValue("@username", (object?)dto.UserName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@role", (object?)dto.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@isActive", dto.IsActive);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0
                ? Ok(new { message = "User updated." })
                : NotFound(new { message = "User not found." });
        }

        // =========================
        // POST: /users/adduser
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] AppUser newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.UserName))
                return BadRequest(new { message = "UserName is required." });

            // For now, we’re storing the plain string in PasswordHash (until hashing is added)
            var password = string.IsNullOrWhiteSpace(newUser.PasswordHash)
                ? "ChangeMe123!"  // sensible default in dev
                : newUser.PasswordHash;

            var cs = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(cs);
            connection.Open();

            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Users
                    (EmployeeId, Username, PasswordHash, Role, Email, IsActive, CreatedAt)
                VALUES
                    (@employeeId, @username, @passwordHash, @role, @email, @isActive, GETDATE());", connection);

            cmd.Parameters.AddWithValue("@employeeId", (object?)newUser.EmployeeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@username", newUser.UserName);
            cmd.Parameters.AddWithValue("@passwordHash", password); // add hash 
            cmd.Parameters.AddWithValue("@role", (object?)newUser.Role ?? "Employee");
            cmd.Parameters.AddWithValue("@email", (object?)newUser.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@isActive", newUser.IsActive);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0
                ? Ok(new { message = "User added." })
                : BadRequest(new { message = "Failed to add user." });
            
        }
    }
}
