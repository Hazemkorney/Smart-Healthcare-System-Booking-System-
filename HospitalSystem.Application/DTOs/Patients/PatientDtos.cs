using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Application.DTOs.Patients;

public record CreatePatientRequest(
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string Phone,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType);

public record UpdatePatientRequest(
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string Phone,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType);

public record PatientResponse(
    Guid Id,
    Guid? UserId,
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string Phone,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType,
    DateTime CreatedAt);
