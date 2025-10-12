using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models;

[Table("Attendance")] // maps to dbo.Attendance
public class Attendance
{
    public long AttendanceId { get; set; }   
    public int EmployeeId { get; set; }      
    public DateTime WorkDate { get; set; }
    public decimal? HoursWorked { get; set; }
    public byte Status { get; set; } = 1;
    public string? Notes { get; set; }
}
