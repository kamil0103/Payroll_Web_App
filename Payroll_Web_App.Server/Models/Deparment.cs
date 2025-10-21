using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Department")]
public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = "";
    public string? ManagerName { get; set; }
}
