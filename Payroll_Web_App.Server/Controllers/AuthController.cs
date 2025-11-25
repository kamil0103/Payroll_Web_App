using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Payroll_Web_App.Server.Data;
using Payroll_Web_App.Server.Models;
using Payroll_Web_App.Server.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Payroll_Web_App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;  // Gives access to the users table via EF Core

        public AuthController(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        // the Expected JSON body for logins
        public record LoginRequest(string UserName, string Password);

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            // find user in database
            var user = _db.Users.FirstOrDefault(u => u.UserName == req.UserName);
            // reject for missing account
            if (user is null || !user.IsActive)
                return Unauthorized(new { message = "Invalid credentials" });
            bool passwordOk = PasswordHasher.Verify(req.Password, user.PasswordHash);

            if (!passwordOk)
                return Unauthorized(new { message = "Invalid credentials" });

            var jwt = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Defines the claims from the encoded token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("EmployeeId", user.EmployeeId?.ToString() ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "Employee"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // makes the jwt token
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"], // who made token
                audience: jwt["Audience"], // who token is meant for
                claims: claims, // user embedded info
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"] ?? "60")), // how long token is valid
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Role,
                    user.Email
                }
            });
        }
    }
}
