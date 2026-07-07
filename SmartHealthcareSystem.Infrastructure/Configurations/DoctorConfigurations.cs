using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Configurations
{
    public class DoctorConfigurations : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
       


            builder.Property(d=>d.Specialization).IsRequired().HasMaxLength(100);
            builder.Property(d => d.ConsultationFee)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(350m);

            builder.HasOne(d => d.Department)
       .WithMany(dep => dep.Doctors)
       .HasForeignKey(d => d.DepartmentId)
       .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
