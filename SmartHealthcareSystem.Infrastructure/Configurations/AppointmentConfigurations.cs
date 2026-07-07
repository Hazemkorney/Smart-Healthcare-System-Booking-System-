using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Configurations
{
  public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
        {
            public void Configure(EntityTypeBuilder<Appointment> builder)
            {
                builder.HasKey(d => d.Id);

            builder.Property(d => d.AppointmentDate)
                   .IsRequired();

            builder.Property(d => d.DoctorId)
                  .IsRequired();
            builder.Property(d => d.PatientId)
                  .IsRequired();
            builder.Property(d => d.Status)
                .IsRequired();


            builder.HasOne(a => a.Patient)
           .WithMany(p => p.Appointments)
           .HasForeignKey(a => a.PatientId)
           .OnDelete(DeleteBehavior.Restrict);

          
            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
        }
    }

