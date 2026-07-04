using HospitalSystem.Domain.Entities;

namespace HospitalSystem.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<bool> IsSlotAvailableAsync(
        Guid doctorId,
        DateOnly date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsPatientSlotAvailableAsync(
        Guid patientId,
        DateOnly date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
