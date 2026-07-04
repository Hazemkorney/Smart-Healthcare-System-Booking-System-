using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Departments;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var department = Department.Create(request.Name, request.Description);
        await _unitOfWork.Departments.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(department);
    }

    public async Task<DepartmentResponse> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Department not found.");

        department.Update(request.Name, request.Description);
        await _unitOfWork.Departments.UpdateAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(department);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Department not found.");

        department.Deactivate();
        await _unitOfWork.Departments.UpdateAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DepartmentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Department not found.");
        return Map(department);
    }

    public async Task<PagedResponse<DepartmentResponse>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var departments = (await _unitOfWork.Departments.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();
        return Pagination.Create(departments.Select(Map), page, pageSize);
    }

    private static DepartmentResponse Map(Department d) =>
        new(d.Id, d.Name, d.Description, d.IsActive);
}
