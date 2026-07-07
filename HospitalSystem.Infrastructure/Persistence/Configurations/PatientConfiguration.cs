using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.DateOfBirth).IsRequired();
        builder.Property(p => p.Gender).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Phone).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.NationalId).HasMaxLength(50);
        builder.Property(p => p.BloodType).HasMaxLength(10);
        builder.Property(p => p.CreatedAt).IsRequired();
    }
}
