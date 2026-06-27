using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Interfaces;
using HospitalSystem.Infrastructure.Persistence;

namespace HospitalSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HospitalDbContext _context;

    public UnitOfWork(HospitalDbContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(context);
        Departments = new GenericRepository<Department>(context);
        Doctors = new DoctorRepository(context);
        DoctorSchedules = new GenericRepository<DoctorSchedule>(context);
        Patients = new GenericRepository<Patient>(context);
        Appointments = new AppointmentRepository(context);
        Consultations = new GenericRepository<Consultation>(context);
        Prescriptions = new GenericRepository<Prescription>(context);
        Receptionists = new GenericRepository<Receptionist>(context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Department> Departments { get; }
    public IDoctorRepository Doctors { get; }
    public IRepository<DoctorSchedule> DoctorSchedules { get; }
    public IRepository<Patient> Patients { get; }
    public IAppointmentRepository Appointments { get; }
    public IRepository<Consultation> Consultations { get; }
    public IRepository<Prescription> Prescriptions { get; }
    public IRepository<Receptionist> Receptionists { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
