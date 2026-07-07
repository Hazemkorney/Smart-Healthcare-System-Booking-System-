using HospitalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSystem.Infrastructure.Persistence.Configurations;

public class DefaultDoctorScheduleConfiguration : IEntityTypeConfiguration<DefaultDoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DefaultDoctorSchedule> builder)
    {
        builder.ToTable("DefaultDoctorSchedules");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.AppointmentDurationMinutes).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        builder.HasIndex(s => s.DayOfWeek).IsUnique();
    }
}
