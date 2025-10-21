using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Web_App.Server.Models
{
    [Table("Attendance")] // maps to dbo.Attendance
    public class Attendance
    {
        public int AttendanceId { get; set; }        // Matches AttendanceID INT
        public int EmployeeId { get; set; }          // Matches EmployeeID INT
        public DateTime Date { get; set; }           // Matches [Date] DATE
        public decimal? HoursWorked { get; set; }    // Matches DECIMAL(4,2)
        public string? Status { get; set; }          // Matches NVARCHAR(20)
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default GETDATE()
    }
}
