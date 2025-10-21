using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models
{
    [Table("Users")] // maps to dbo.Users
    public class AppUser
    {
        [Column("UserID")]
        public int UserId { get; set; }

        [Column("EmployeeId")]
        public int? EmployeeId { get; set; }

        [Column("Username")]
        public string UserName { get; set; } = string.Empty;

        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty; // changed from byte[] to string

        [Column("Role")]
        public string Role { get; set; } = "Employee";

        [Column("Email")]
        public string? Email { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
