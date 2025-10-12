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
        b.Entity<AppUser>()
            .HasOne<Employee>()
            .WithMany()
            .HasForeignKey(u => u.EmployeeId);

        b.Entity<Attendance>()
            .HasOne<Employee>()
            .WithMany()
            .HasForeignKey(a => a.EmployeeId);

        b.Entity<Payroll>()
            .HasOne<Employee>()
            .WithMany()
            .HasForeignKey(p => p.EmployeeId);
    }
}
