using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Doctors;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public DoctorService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);
        await EnsureEmailUnique(request.Email, cancellationToken);

        var user = User.Create(request.Email, _passwordHasher.Hash(request.Password), UserRole.Doctor);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        var doctor = Doctor.Create(user.Id, request.DepartmentId, request.FullName, request.Specialization, request.Phone);
        await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapDoctorAsync(doctor, cancellationToken);
    }

    public async Task<DoctorResponse> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);
        doctor.Update(request.FullName, request.Specialization, request.Phone);
        doctor.AssignToDepartment(request.DepartmentId);

        await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await MapDoctorAsync(doctor, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        doctor.Deactivate();
        await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DoctorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");
        return await MapDoctorAsync(doctor, cancellationToken);
    }

    public async Task<PagedResponse<DoctorResponse>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);
        var mapped = new List<DoctorResponse>();
        foreach (var doctor in doctors)
            mapped.Add(await MapDoctorAsync(doctor, cancellationToken));
        return Pagination.Create(mapped, page, pageSize);
    }

    public async Task<IReadOnlyList<DoctorResponse>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(departmentId, cancellationToken);
        var results = new List<DoctorResponse>();
        foreach (var doctor in doctors)
            results.Add(await MapDoctorAsync(doctor, cancellationToken));
        return results;
    }

    public async Task AssignToDepartmentAsync(Guid doctorId, Guid departmentId, CancellationToken cancellationToken = default)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        await EnsureDepartmentExists(departmentId, cancellationToken);
        doctor.AssignToDepartment(departmentId);
        await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SetScheduleAsync(Guid doctorId, SetDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        var existing = await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken);
        foreach (var schedule in existing.Where(s => s.DoctorId == doctorId))
        {
            schedule.Deactivate();
            await _unitOfWork.DoctorSchedules.UpdateAsync(schedule, cancellationToken);
        }

        foreach (var item in request.Schedules)
        {
            if (item.StartTime >= item.EndTime)
                throw new ValidationException("Start time must be before end time.");

            if (item.AppointmentDurationMinutes is < 10 or > 120)
                throw new ValidationException("Appointment duration must be between 10 and 120 minutes.");

            var schedule = DoctorSchedule.Create(
                doctorId, item.DayOfWeek, item.StartTime, item.EndTime, item.AppointmentDurationMinutes);
            await _unitOfWork.DoctorSchedules.AddAsync(schedule, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorScheduleResponse>> GetSchedulesAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        var schedules = await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken);
        return schedules
            .Where(s => s.DoctorId == doctorId && s.IsActive)
            .Select(s => new DoctorScheduleResponse(s.Id, s.DayOfWeek, s.StartTime, s.EndTime, s.AppointmentDurationMinutes, s.IsActive))
            .ToList();
    }

    public async Task<DoctorResponse> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);
        var doctor = doctors.FirstOrDefault(d => d.UserId == userId)
            ?? throw new NotFoundException("Doctor profile not found.");
        return await MapDoctorAsync(doctor, cancellationToken);
    }

    private async Task EnsureDepartmentExists(Guid departmentId, CancellationToken cancellationToken)
    {
        _ = await _unitOfWork.Departments.GetByIdAsync(departmentId, cancellationToken)
            ?? throw new NotFoundException("Department not found.");
    }

    private async Task EnsureEmailUnique(string email, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException("Email is already registered.");
    }

    private async Task<DoctorResponse> MapDoctorAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(doctor.DepartmentId, cancellationToken);
        return new DoctorResponse(
            doctor.Id, doctor.UserId, doctor.DepartmentId,
            department?.Name ?? string.Empty,
            doctor.FullName, doctor.Specialization, doctor.Phone, doctor.IsActive);
    }
}
