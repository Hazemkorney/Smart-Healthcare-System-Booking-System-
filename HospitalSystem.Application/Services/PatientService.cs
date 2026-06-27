using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Patients;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;

    public PatientService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var patient = Patient.Create(
            request.FullName,
            request.DateOfBirth,
            request.Gender,
            request.Phone,
            request.Email,
            request.Address,
            request.NationalId,
            request.BloodType);

        await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(patient);
    }

    public async Task<PatientResponse> UpdateAsync(Guid id, UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");

        patient.Update(
            request.FullName, request.DateOfBirth, request.Gender,
            request.Phone, request.Email, request.Address,
            request.NationalId, request.BloodType);

        await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(patient);
    }

    public async Task<PatientResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");
        return Map(patient);
    }

    public async Task<PagedResponse<PatientResponse>> SearchAsync(
        string? query,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
        IEnumerable<Patient> filtered = patients;

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();
            filtered = patients.Where(p =>
                p.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Phone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (p.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.NationalId?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return Pagination.Create(filtered.Select(Map), page, pageSize);
    }

    public async Task<PatientResponse> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
        var patient = patients.FirstOrDefault(p => p.UserId == userId)
            ?? throw new NotFoundException("Patient profile not found.");
        return Map(patient);
    }

    private static PatientResponse Map(Patient p) =>
        new(p.Id, p.UserId, p.FullName, p.DateOfBirth, p.Gender,
            p.Phone, p.Email, p.Address, p.NationalId, p.BloodType, p.CreatedAt);
}
