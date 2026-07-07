using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Interfaces;
using HospitalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HospitalSystem.Infrastructure.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(HospitalDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Department)
            .Where(d => d.DepartmentId == departmentId && d.IsActive)
            .ToListAsync(cancellationToken);
    }
}
