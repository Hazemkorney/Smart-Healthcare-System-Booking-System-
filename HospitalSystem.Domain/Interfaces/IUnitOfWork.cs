using HospitalSystem.Domain.Entities;

namespace HospitalSystem.Domain.Interfaces;

public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Department> Departments { get; }
    IDoctorRepository Doctors { get; }
    IRepository<DoctorSchedule> DoctorSchedules { get; }
    IRepository<DefaultDoctorSchedule> DefaultDoctorSchedules { get; }
    IRepository<DoctorDateSchedule> DoctorDateSchedules { get; }
    IRepository<DefaultDoctorDateSchedule> DefaultDoctorDateSchedules { get; }
    IRepository<Patient> Patients { get; }
    IAppointmentRepository Appointments { get; }
    IRepository<Consultation> Consultations { get; }
    IRepository<Prescription> Prescriptions { get; }
    IRepository<Receptionist> Receptionists { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
