using Microsoft.EntityFrameworkCore;
using ConsultantManagementApi.Models;

namespace ConsultantManagementApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Consultant> Consultants { get; set; }
    public DbSet<ConsultantRole> ConsultantRoles { get; set; }
    public DbSet<ConsultantTask> Tasks { get; set; }
    public DbSet<TaskAssignment> TaskAssignments { get; set; }
    public DbSet<WorkEntry> WorkEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Consultant>()
            .HasOne(c => c.ConsultantRole)
            .WithMany(r => r.Consultants)
            .HasForeignKey(c => c.ConsultantRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskAssignment>()
            .HasOne(ta => ta.Consultant)
            .WithMany(c => c.TaskAssignments)
            .HasForeignKey(ta => ta.ConsultantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskAssignment>()
            .HasOne(ta => ta.Task)
            .WithMany(t => t.TaskAssignments)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkEntry>()
            .HasOne(we => we.Consultant)
            .WithMany(c => c.WorkEntries)
            .HasForeignKey(we => we.ConsultantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkEntry>()
            .HasOne(we => we.Task)
            .WithMany(t => t.WorkEntries)
            .HasForeignKey(we => we.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConsultantRole>().HasData(
            new ConsultantRole { Id = 1, Name = "Consultant Level 1", RatePerHour = 50.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ConsultantRole { Id = 2, Name = "Consultant Level 2", RatePerHour = 75.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
