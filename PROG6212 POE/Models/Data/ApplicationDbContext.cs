using System.Collections.Generic;
using System.Reflection.Emit;
using ContractMonthlyClaimSystem.Models;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models.Data.Entities;

namespace PROG6212_POE.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed initial data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "lecturer1", Password = "password", Email = "lecturer1@university.com", FullName = "Dr. John Smith", Role = UserRole.Lecturer },
                new User { Id = 2, Username = "coordinator1", Password = "password", Email = "coordinator1@university.com", FullName = "Prof. Sarah Johnson", Role = UserRole.ProgramCoordinator },
                new User { Id = 3, Username = "manager1", Password = "password", Email = "manager1@university.com", FullName = "Dr. Michael Brown", Role = UserRole.AcademicManager }
            );

            modelBuilder.Entity<Claim>().HasData(
                new Claim { Id = 1, Title = "Research Materials", Description = "Books and journals for research", HoursWorked = 10, HourlyRate = 150, Date = DateTime.Now.AddDays(-5), Status = "Approved", LecturerId = 1, ApprovedById = 2, ApprovalDate = DateTime.Now.AddDays(-3), Notes = "Approved for research purposes" },
                new Claim { Id = 2, Title = "Conference Travel", Description = "Travel expenses for academic conference", HoursWorked = 8, HourlyRate = 120, Date = DateTime.Now.AddDays(-12), Status = "Pending", LecturerId = 1, Notes = "International conference presentation" },
                new Claim { Id = 3, Title = "Software License", Description = "Annual license for statistical software", HoursWorked = 5, HourlyRate = 200, Date = DateTime.Now.AddDays(-18), Status = "Approved", LecturerId = 1, ApprovedById = 3, ApprovalDate = DateTime.Now.AddDays(-10), Notes = "Essential research tool" }
            );
        }
    }
}