using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasOne(u => u.Doctor).WithOne(d => d.User).HasForeignKey<Doctor>(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(u => u.Patient).WithOne(p => p.User).HasForeignKey<Patient>(p => p.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(u => u.Receptionist).WithOne(r => r.User).HasForeignKey<Receptionist>(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
