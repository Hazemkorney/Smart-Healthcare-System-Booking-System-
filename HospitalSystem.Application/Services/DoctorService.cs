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

        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var existingUser = users.FirstOrDefault(u =>
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (existingUser is not null)
        {
            if (existingUser.IsActive || existingUser.Role != UserRole.Doctor)
                throw new ValidationException("Email is already registered.");

            var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);
            var doctor = doctors.FirstOrDefault(d => d.UserId == existingUser.Id)
                ?? throw new ValidationException("Email is already registered.");

            existingUser.Activate();
            existingUser.UpdatePassword(_passwordHasher.Hash(request.Password));
            doctor.Activate();
            doctor.Update(request.FullName, request.Specialization, request.Phone);
            doctor.AssignToDepartment(request.DepartmentId);

            var existingSchedules = await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken);
            foreach (var schedule in existingSchedules.Where(s => s.DoctorId == doctor.Id && s.IsActive))
            {
                schedule.Deactivate();
                await _unitOfWork.DoctorSchedules.UpdateAsync(schedule, cancellationToken);
            }

            await _unitOfWork.Users.UpdateAsync(existingUser, cancellationToken);
            await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await ApplyDefaultDateSchedulesToDoctorAsync(doctor.Id, cancellationToken);
            return await MapDoctorAsync(doctor, cancellationToken);
        }

        var user = User.Create(request.Email, _passwordHasher.Hash(request.Password), UserRole.Doctor);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        var newDoctor = Doctor.Create(user.Id, request.DepartmentId, request.FullName, request.Specialization, request.Phone);
        await _unitOfWork.Doctors.AddAsync(newDoctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await ApplyDefaultDateSchedulesToDoctorAsync(newDoctor.Id, cancellationToken);

        return await MapDoctorAsync(newDoctor, cancellationToken);
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

        var user = await _unitOfWork.Users.GetByIdAsync(doctor.UserId, cancellationToken);
        user?.Deactivate();

        var schedules = await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken);
        foreach (var schedule in schedules.Where(s => s.DoctorId == id && s.IsActive))
        {
            schedule.Deactivate();
            await _unitOfWork.DoctorSchedules.UpdateAsync(schedule, cancellationToken);
        }

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
        var doctors = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();
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

        await UpsertDoctorSchedulesAsync(doctorId, request.Schedules, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorScheduleResponse>> GetDefaultScheduleAsync(CancellationToken cancellationToken = default)
    {
        var schedules = await _unitOfWork.DefaultDoctorSchedules.GetAllAsync(cancellationToken);
        return schedules
            .Where(s => s.IsActive)
            .Select(s => new DoctorScheduleResponse(s.Id, s.DayOfWeek, s.StartTime, s.EndTime, s.AppointmentDurationMinutes, s.IsActive))
            .ToList();
    }

    public async Task SetDefaultScheduleAsync(SetDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var existing = (await _unitOfWork.DefaultDoctorSchedules.GetAllAsync(cancellationToken)).ToList();
        var requestedDays = request.Schedules.Select(s => s.DayOfWeek).ToHashSet();

        foreach (var schedule in existing.Where(s => !requestedDays.Contains(s.DayOfWeek)))
        {
            schedule.Deactivate();
            await _unitOfWork.DefaultDoctorSchedules.UpdateAsync(schedule, cancellationToken);
        }

        foreach (var item in request.Schedules)
        {
            ValidateScheduleItem(item);

            var schedule = existing.FirstOrDefault(s => s.DayOfWeek == item.DayOfWeek);
            if (schedule is not null)
            {
                schedule.Update(item.StartTime, item.EndTime, item.AppointmentDurationMinutes);
                schedule.Activate();
                await _unitOfWork.DefaultDoctorSchedules.UpdateAsync(schedule, cancellationToken);
            }
            else
            {
                var newSchedule = DefaultDoctorSchedule.Create(
                    item.DayOfWeek, item.StartTime, item.EndTime, item.AppointmentDurationMinutes);
                await _unitOfWork.DefaultDoctorSchedules.AddAsync(newSchedule, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyDefaultScheduleToAllDoctorsAsync(CancellationToken cancellationToken = default)
    {
        var defaultSchedules = await BuildDefaultScheduleRequestAsync(cancellationToken);
        if (defaultSchedules.Schedules.Count == 0)
            throw new ValidationException("No default schedule configured.");

        var doctors = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();

        foreach (var doctor in doctors)
            await SetScheduleAsync(doctor.Id, defaultSchedules, cancellationToken);
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

    public async Task<IReadOnlyList<DoctorDateScheduleResponse>> GetDefaultDateSchedulesAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        if (from.HasValue && to.HasValue && to.Value < from.Value)
            throw new ValidationException("'to' date must be on or after 'from' date.");

        var schedules = await _unitOfWork.DefaultDoctorDateSchedules.GetAllAsync(cancellationToken);
        return schedules
            .Where(s => s.IsActive
                && (!from.HasValue || s.ScheduleDate >= from.Value)
                && (!to.HasValue || s.ScheduleDate <= to.Value))
            .OrderBy(s => s.ScheduleDate)
            .Select(MapDefaultDateSchedule)
            .ToList();
    }

    public async Task<DoctorDateScheduleResponse> SetDefaultDateScheduleAsync(
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateDateScheduleRequest(request);

        var existing = (await _unitOfWork.DefaultDoctorDateSchedules.GetAllAsync(cancellationToken))
            .FirstOrDefault(s => s.ScheduleDate == request.Date);

        if (existing is not null)
        {
            existing.Update(request.StartTime, request.EndTime, request.AppointmentDurationMinutes);
            existing.Activate();
            await _unitOfWork.DefaultDoctorDateSchedules.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            existing = DefaultDoctorDateSchedule.Create(
                request.Date, request.StartTime, request.EndTime, request.AppointmentDurationMinutes);
            await _unitOfWork.DefaultDoctorDateSchedules.AddAsync(existing, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDefaultDateSchedule(existing);
    }

    public async Task RemoveDefaultDateScheduleAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var existing = (await _unitOfWork.DefaultDoctorDateSchedules.GetAllAsync(cancellationToken))
            .FirstOrDefault(s => s.ScheduleDate == date)
            ?? throw new NotFoundException("Schedule for this date was not found.");

        existing.Deactivate();
        await _unitOfWork.DefaultDoctorDateSchedules.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyDefaultDateSchedulesToAllDoctorsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var defaults = await GetDefaultDateSchedulesAsync(from, to, cancellationToken);
        await ApplyDefaultSchedulesToAllDoctorsCoreAsync(defaults, cancellationToken);
    }

    public async Task ApplySelectedDefaultDateSchedulesToAllDoctorsAsync(
        IReadOnlyList<DateOnly> dates,
        CancellationToken cancellationToken = default)
    {
        if (dates.Count == 0)
            throw new ValidationException("Select at least one date.");

        var selectedDates = dates.Distinct().ToHashSet();
        var defaults = (await GetDefaultDateSchedulesAsync(cancellationToken: cancellationToken))
            .Where(s => selectedDates.Contains(s.Date))
            .ToList();

        if (defaults.Count == 0)
            throw new ValidationException("No default date schedules found for the selected dates.");

        await ApplyDefaultSchedulesToAllDoctorsCoreAsync(defaults, cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorDateScheduleResponse>> GetAppliedDateSchedulesAsync(
        CancellationToken cancellationToken = default)
    {
        var doctorIds = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .Select(d => d.Id)
            .ToHashSet();

        if (doctorIds.Count == 0)
            return Array.Empty<DoctorDateScheduleResponse>();

        var schedules = (await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken))
            .Where(s => s.IsActive && doctorIds.Contains(s.DoctorId))
            .ToList();

        return schedules
            .GroupBy(s => s.ScheduleDate)
            .Where(g => g.Select(x => x.DoctorId).Distinct().Count() == doctorIds.Count)
            .Where(g => g.All(s =>
                s.StartTime == g.First().StartTime &&
                s.EndTime == g.First().EndTime &&
                s.AppointmentDurationMinutes == g.First().AppointmentDurationMinutes))
            .OrderBy(g => g.Key)
            .Select(g => MapDoctorDateSchedule(g.First()))
            .ToList();
    }

    public async Task<DoctorDateScheduleResponse> ApplyDateScheduleToAllDoctorsAsync(
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateDateScheduleRequest(request);

        var doctors = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();

        if (doctors.Count == 0)
            throw new ValidationException("No active doctors found.");

        DoctorDateScheduleResponse? last = null;
        foreach (var doctor in doctors)
            last = await SetDateScheduleAsync(doctor.Id, request, cancellationToken);

        return last!;
    }

    public async Task RemoveDateScheduleFromAllDoctorsAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var doctors = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();

        if (doctors.Count == 0)
            throw new ValidationException("No active doctors found.");

        var schedules = await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken);
        var found = false;

        foreach (var doctor in doctors)
        {
            var existing = schedules.FirstOrDefault(s =>
                s.DoctorId == doctor.Id && s.ScheduleDate == date && s.IsActive);
            if (existing is null)
                continue;

            existing.Deactivate();
            await _unitOfWork.DoctorDateSchedules.UpdateAsync(existing, cancellationToken);
            found = true;
        }

        if (!found)
            throw new NotFoundException("Schedule for this date was not found on any doctor.");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ApplyDefaultSchedulesToAllDoctorsCoreAsync(
        IReadOnlyList<DoctorDateScheduleResponse> defaults,
        CancellationToken cancellationToken)
    {
        if (defaults.Count == 0)
            throw new ValidationException("No default date schedules found.");

        var doctors = (await _unitOfWork.Doctors.GetAllAsync(cancellationToken))
            .Where(d => d.IsActive)
            .ToList();

        foreach (var doctor in doctors)
        {
            foreach (var item in defaults)
            {
                await SetDateScheduleAsync(doctor.Id, new SetDoctorDateScheduleRequest(
                    item.Date, item.StartTime, item.EndTime, item.AppointmentDurationMinutes), cancellationToken);
            }
        }
    }

    public async Task<IReadOnlyList<DoctorDateScheduleResponse>> GetDateSchedulesAsync(
        Guid doctorId,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        if (from.HasValue && to.HasValue && to.Value < from.Value)
            throw new ValidationException("'to' date must be on or after 'from' date.");

        var schedules = await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken);
        return schedules
            .Where(s => s.DoctorId == doctorId && s.IsActive
                && (!from.HasValue || s.ScheduleDate >= from.Value)
                && (!to.HasValue || s.ScheduleDate <= to.Value))
            .OrderBy(s => s.ScheduleDate)
            .Select(MapDoctorDateSchedule)
            .ToList();
    }

    public async Task<DoctorDateScheduleResponse?> GetDateScheduleForDateAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var schedules = await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken);
        var schedule = schedules.FirstOrDefault(s =>
            s.DoctorId == doctorId && s.ScheduleDate == date && s.IsActive);
        return schedule is null ? null : MapDoctorDateSchedule(schedule);
    }

    public async Task<DoctorDateScheduleResponse> SetDateScheduleAsync(
        Guid doctorId,
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        ValidateDateScheduleRequest(request);

        var existing = (await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken))
            .FirstOrDefault(s => s.DoctorId == doctorId && s.ScheduleDate == request.Date);

        if (existing is not null)
        {
            existing.Update(request.StartTime, request.EndTime, request.AppointmentDurationMinutes);
            existing.Activate();
            await _unitOfWork.DoctorDateSchedules.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            existing = DoctorDateSchedule.Create(
                doctorId, request.Date, request.StartTime, request.EndTime, request.AppointmentDurationMinutes);
            await _unitOfWork.DoctorDateSchedules.AddAsync(existing, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDoctorDateSchedule(existing);
    }

    public async Task RemoveDateScheduleAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        var existing = (await _unitOfWork.DoctorDateSchedules.GetAllAsync(cancellationToken))
            .FirstOrDefault(s => s.DoctorId == doctorId && s.ScheduleDate == date)
            ?? throw new NotFoundException("Schedule for this date was not found.");

        existing.Deactivate();
        await _unitOfWork.DoctorDateSchedules.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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

    private async Task ApplyDefaultScheduleToDoctorAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        var defaultSchedules = await BuildDefaultScheduleRequestAsync(cancellationToken);
        if (defaultSchedules.Schedules.Count == 0)
            return;

        await UpsertDoctorSchedulesAsync(doctorId, defaultSchedules.Schedules, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<SetDoctorScheduleRequest> BuildDefaultScheduleRequestAsync(CancellationToken cancellationToken)
    {
        var schedules = await GetDefaultScheduleAsync(cancellationToken);
        var items = schedules
            .Select(s => new DoctorScheduleRequest(s.DayOfWeek, s.StartTime, s.EndTime, s.AppointmentDurationMinutes))
            .ToList();
        return new SetDoctorScheduleRequest(items);
    }

    private async Task UpsertDoctorSchedulesAsync(
        Guid doctorId,
        IReadOnlyList<DoctorScheduleRequest> schedules,
        CancellationToken cancellationToken)
    {
        var existing = (await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken))
            .Where(s => s.DoctorId == doctorId)
            .ToList();

        var requestedDays = schedules.Select(s => s.DayOfWeek).ToHashSet();

        foreach (var schedule in existing.Where(s => !requestedDays.Contains(s.DayOfWeek)))
        {
            schedule.Deactivate();
            await _unitOfWork.DoctorSchedules.UpdateAsync(schedule, cancellationToken);
        }

        foreach (var item in schedules)
        {
            ValidateScheduleItem(item);

            var schedule = existing.FirstOrDefault(s => s.DayOfWeek == item.DayOfWeek);
            if (schedule is not null)
            {
                schedule.Update(item.StartTime, item.EndTime, item.AppointmentDurationMinutes);
                schedule.Activate();
                await _unitOfWork.DoctorSchedules.UpdateAsync(schedule, cancellationToken);
            }
            else
            {
                var newSchedule = DoctorSchedule.Create(
                    doctorId, item.DayOfWeek, item.StartTime, item.EndTime, item.AppointmentDurationMinutes);
                await _unitOfWork.DoctorSchedules.AddAsync(newSchedule, cancellationToken);
            }
        }
    }

    private static void ValidateScheduleItem(DoctorScheduleRequest item)
    {
        if (item.StartTime >= item.EndTime)
            throw new ValidationException("Start time must be before end time.");

        if (item.AppointmentDurationMinutes is < 10 or > 120)
            throw new ValidationException("Appointment duration must be between 10 and 120 minutes.");
    }

    private static void ValidateDateScheduleRequest(SetDoctorDateScheduleRequest request)
    {
        if (request.StartTime >= request.EndTime)
            throw new ValidationException("Start time must be before end time.");

        if (request.AppointmentDurationMinutes is < 10 or > 120)
            throw new ValidationException("Appointment duration must be between 10 and 120 minutes.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.Date < today)
            throw new ValidationException("Cannot set schedule for a past date.");

        if (request.Date != today)
            return;

        var now = DateTime.UtcNow.TimeOfDay;
        if (request.StartTime <= now)
            throw new ValidationException("Start time cannot be in the past for today.");

        if (request.EndTime <= now)
            throw new ValidationException("End time cannot be in the past for today.");
    }

    private async Task ApplyDefaultDateSchedulesToDoctorAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        var defaults = await _unitOfWork.DefaultDoctorDateSchedules.GetAllAsync(cancellationToken);
        foreach (var item in defaults.Where(s => s.IsActive))
        {
            await SetDateScheduleAsync(doctorId, new SetDoctorDateScheduleRequest(
                item.ScheduleDate, item.StartTime, item.EndTime, item.AppointmentDurationMinutes), cancellationToken);
        }
    }

    private static DoctorDateScheduleResponse MapDoctorDateSchedule(DoctorDateSchedule schedule) =>
        new(schedule.Id, schedule.ScheduleDate, schedule.StartTime, schedule.EndTime,
            schedule.AppointmentDurationMinutes, schedule.IsActive);

    private static DoctorDateScheduleResponse MapDefaultDateSchedule(DefaultDoctorDateSchedule schedule) =>
        new(schedule.Id, schedule.ScheduleDate, schedule.StartTime, schedule.EndTime,
            schedule.AppointmentDurationMinutes, schedule.IsActive);

    private async Task<DoctorResponse> MapDoctorAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(doctor.DepartmentId, cancellationToken);
        return new DoctorResponse(
            doctor.Id, doctor.UserId, doctor.DepartmentId,
            department?.Name ?? string.Empty,
            doctor.FullName, doctor.Specialization, doctor.Phone, doctor.IsActive);
    }
}
