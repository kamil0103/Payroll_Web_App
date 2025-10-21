using Payroll_Web_App.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Payroll_Web_App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var users = new List<AppUser>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT UserID, Username, Email, Role, IsActive FROM Users", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new AppUser
                        {
                            UserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0,
                            UserName = reader["Username"]?.ToString() ?? "",
                            Email = reader["Email"]?.ToString() ?? "",
                        };

                        // Optional: safely read IsActive if it exists
                        if (reader.GetOrdinal("IsActive") >= 0 && reader["IsActive"] != DBNull.Value)
                        {
                            // assuming your User model has a bool field for IsActive
                            user.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        }

                        users.Add(user);
                    }
                }
            }

            return Ok(users);
        }

        // =========================
        // POST: /users/updateuser
        // =========================
        [HttpPost("updateuser")]
        public IActionResult UpdateUser([FromBody] AppUser dto)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand(
                    "UPDATE Users SET UserName = @username, Role = @role, IsActive = @isActive WHERE UserId = @id",
                    connection);

                cmd.Parameters.AddWithValue("@username", dto.UserName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@role", dto.Role ?? (object)"Employee");
                cmd.Parameters.AddWithValue("@isActive", dto.IsActive);
                cmd.Parameters.AddWithValue("@id", dto.UserId);

                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok(new { message = "User updated successfully." });
                else
                    return NotFound(new { message = "User not found." });
            }
        }

        // =========================
        // POST: /users/adduser
        // =========================
        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] AppUser newUser)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand(
                    @"INSERT INTO Users (EmployeeId, UserName, Role, IsActive)
                      VALUES (@employeeId, @username, @role, @isActive);",
                    connection);

                cmd.Parameters.AddWithValue("@employeeId", newUser.EmployeeId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@username", newUser.UserName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@role", newUser.Role ?? "Employee");
                cmd.Parameters.AddWithValue("@isActive", newUser.IsActive);

                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok(new { message = "User added successfully." });
                else
                    return BadRequest(new { message = "Failed to add user." });
            }
        }
    }
}
