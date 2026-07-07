using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Configurations
{
    public class PatientsConfigurations:IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {

            builder.Property(d => d.BirthDay).IsRequired();
            
            builder.Property(d => d.Gender)
                 .IsRequired();

            builder.HasMany(p => p.Appointments)
       .WithOne(a => a.Patient)
       .HasForeignKey(a => a.PatientId)
       .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
