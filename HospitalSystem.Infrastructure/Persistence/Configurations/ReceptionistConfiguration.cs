using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FullName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Phone).HasMaxLength(50);
        builder.Property(r => r.IsActive).IsRequired();
    }
}
