using Microsoft.EntityFrameworkCore;
using Payroll_Web_App.Server.Models;

namespace Payroll_Web_App.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<Payroll> Payroll => Set<Payroll>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Attendance decimal precision 
        b.Entity<Attendance>()
            .Property(a => a.HoursWorked)
            .HasPrecision(9, 2);

        // Payroll decimal precision 
        b.Entity<Payroll>()
            .Property(p => p.GrossPay)
            .HasPrecision(19, 4);

        b.Entity<Payroll>()
            .Property(p => p.TaxDeductions)
            .HasPrecision(19, 4);

        b.Entity<Payroll>()
            .Property(p => p.BenefitsDeductions)
            .HasPrecision(19, 4);

        b.Entity<Payroll>()
            .Property(p => p.NetPay)
            .HasPrecision(19, 4);
    }
}
