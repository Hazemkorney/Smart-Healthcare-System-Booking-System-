using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("Consultations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Diagnosis).HasMaxLength(2000);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.StartedAt).IsRequired();

        builder.HasIndex(c => c.AppointmentId).IsUnique();

        builder.HasOne(c => c.Appointment)
            .WithOne(a => a.Consultation)
            .HasForeignKey<Consultation>(c => c.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
