using Payroll_Web_App.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            var users = new List<User>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT * FROM users", connection);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        user_id = Convert.ToInt32(reader["userid"]),
                        user_name = reader["username"].ToString(),
                        first_name = reader["firstname"].ToString(),
                        last_name = reader["lastname"].ToString(),
                        middle_initial = reader["mi"].ToString(),
                        email_address = reader["email"].ToString(),
                        phone_number = reader["phone"].ToString(),
                    });
                }
            }

            return Ok(users);
        }



        [HttpPost("updateusers")]
        public IActionResult UpdateUsers([FromBody] User dto)
        {
            var connectionString = "DefaultConnection";
            var users = new List<User>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("Update UserProfile set username='" + dto.user_name + "' WHERE userid=" + dto.user_id + "     ", connection);

                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok(new { message = "User updated successfully." });
                else
                    return NotFound(new { message = "User not found." });

            }
        }



        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] User newUser)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO UserProfile (username, firstname, lastname, mi, email, phone) " +
                    "VALUES (@username, @firstname, @lastname, @mi, @email, @phone);", connection);

                cmd.Parameters.AddWithValue("@username", newUser.user_name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@firstname", newUser.first_name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lastname", newUser.last_name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mi", newUser.middle_initial ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", newUser.email_address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", newUser.phone_number ?? (object)DBNull.Value);

                var rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    return Ok(new { message = "User added successfully." });
                else
                    return BadRequest(new { message = "Failed to add user." });
            }
        }






    }

}
