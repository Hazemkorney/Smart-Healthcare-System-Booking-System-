using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SmartHealthcareSystem.Domain.Entities;
namespace SmartHealthcareSystem.Infrastructure.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }
        public DbSet<SmartHealthcareSystem.Domain.Entitis.Patient> Patients { get; set; }
        public DbSet<SmartHealthcareSystem.Domain.Entitis.Doctor> Doctors { get; set; }
        public DbSet<SmartHealthcareSystem.Domain.Entitis.Department> Departments { get; set; }
        public DbSet<SmartHealthcareSystem.Domain.Entitis.Receptionist> Receptionists { get; set; }
        public DbSet<SmartHealthcareSystem.Domain.Entitis.Appointment> Appointments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        }
}}
