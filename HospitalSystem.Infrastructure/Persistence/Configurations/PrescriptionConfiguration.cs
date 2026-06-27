using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MedicationName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Dosage).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Frequency).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Duration).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.HasOne(p => p.Consultation)
            .WithMany(c => c.Prescriptions)
            .HasForeignKey(p => p.ConsultationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
