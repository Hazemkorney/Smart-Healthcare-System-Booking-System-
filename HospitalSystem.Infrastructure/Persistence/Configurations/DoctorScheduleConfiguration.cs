using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.ToTable("DoctorSchedules");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.AppointmentDurationMinutes).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek }).IsUnique();

        builder.HasOne(s => s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
