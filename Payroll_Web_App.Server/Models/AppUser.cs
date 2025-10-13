using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Users")] // maps to dbo.Users
public class AppUser
{
    public int UserId { get; set; }          
    public int? EmployeeId { get; set; }     
    public string UserName { get; set; } = "";
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public string Role { get; set; } = "Employee";
    public bool IsActive { get; set; } = true;
}
