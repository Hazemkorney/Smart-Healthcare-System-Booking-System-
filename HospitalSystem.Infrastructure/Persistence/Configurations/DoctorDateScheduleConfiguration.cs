using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class DoctorDateScheduleConfiguration : IEntityTypeConfiguration<DoctorDateSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorDateSchedule> builder)
    {
        builder.ToTable("DoctorDateSchedules");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ScheduleDate).IsRequired();
        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.AppointmentDurationMinutes).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        builder.HasIndex(s => new { s.DoctorId, s.ScheduleDate }).IsUnique();

        builder.HasOne(s => s.Doctor)
            .WithMany()
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
