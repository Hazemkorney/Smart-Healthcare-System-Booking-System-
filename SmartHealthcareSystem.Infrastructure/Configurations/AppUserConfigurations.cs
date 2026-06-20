using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcareSystem.Domain.Entities;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Configurations
{
    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);




        }
    }
}
