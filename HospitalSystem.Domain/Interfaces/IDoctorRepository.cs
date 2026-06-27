using HospitalSystem.Domain.Entities;

namespace HospitalSystem.Domain.Interfaces;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);
}
