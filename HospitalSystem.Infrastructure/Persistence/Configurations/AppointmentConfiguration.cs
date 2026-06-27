using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AppointmentDate).IsRequired();
        builder.Property(a => a.StartTime).IsRequired();
        builder.Property(a => a.EndTime).IsRequired();
        builder.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Notes).HasMaxLength(2000);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();

        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.StartTime });

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CreatedByReceptionist)
            .WithMany()
            .HasForeignKey(a => a.CreatedByReceptionistId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
