using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(d => d.Name)
                .IsUnique();
        });

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.MobileNumber)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.JobTitle)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.HireDate)
                .IsRequired();
            
            entity.Property(e => e.IsActive)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.HasIndex(e => e.DepartmentId);

            // Foreign key relationship
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
