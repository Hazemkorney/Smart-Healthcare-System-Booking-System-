using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.DTOs.Auth;
using HospitalSystem.Application.DTOs.Consultations;
using HospitalSystem.Application.DTOs.Departments;
using HospitalSystem.Application.DTOs.Doctors;
using HospitalSystem.Application.DTOs.Patients;
using HospitalSystem.Application.DTOs.Receptionists;

namespace HospitalSystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IAppointmentService
{
    Task<AppointmentResponse> BookAppointmentAsync(CreateAppointmentRequest request, Guid receptionistId, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> RescheduleAsync(Guid appointmentId, RescheduleRequest request, Guid receptionistId, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid appointmentId, Guid receptionistId, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> CheckInAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AvailableSlotResponse>> GetAvailableSlotsAsync(
        Guid doctorId,
        DateOnly date,
        Guid patientId,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentResponse>> GetDoctorScheduleAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentResponse>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
}

public interface IDoctorService
{
    Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);
    Task<DoctorResponse> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DoctorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<DoctorResponse>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorResponse>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task AssignToDepartmentAsync(Guid doctorId, Guid departmentId, CancellationToken cancellationToken = default);
    Task SetScheduleAsync(Guid doctorId, SetDoctorScheduleRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorScheduleResponse>> GetSchedulesAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorScheduleResponse>> GetDefaultScheduleAsync(CancellationToken cancellationToken = default);
    Task SetDefaultScheduleAsync(SetDoctorScheduleRequest request, CancellationToken cancellationToken = default);
    Task ApplyDefaultScheduleToAllDoctorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDateScheduleResponse>> GetDefaultDateSchedulesAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);
    Task<DoctorDateScheduleResponse> SetDefaultDateScheduleAsync(
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default);
    Task RemoveDefaultDateScheduleAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task ApplyDefaultDateSchedulesToAllDoctorsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
    Task ApplySelectedDefaultDateSchedulesToAllDoctorsAsync(
        IReadOnlyList<DateOnly> dates,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDateScheduleResponse>> GetAppliedDateSchedulesAsync(
        CancellationToken cancellationToken = default);
    Task<DoctorDateScheduleResponse> ApplyDateScheduleToAllDoctorsAsync(
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default);
    Task RemoveDateScheduleFromAllDoctorsAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDateScheduleResponse>> GetDateSchedulesAsync(
        Guid doctorId,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);
    Task<DoctorDateScheduleResponse> SetDateScheduleAsync(
        Guid doctorId,
        SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken = default);
    Task RemoveDateScheduleAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
    Task<DoctorDateScheduleResponse?> GetDateScheduleForDateAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default);
    Task<DoctorResponse> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IPatientService
{
    Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);
    Task<PatientResponse> UpdateAsync(Guid id, UpdatePatientRequest request, CancellationToken cancellationToken = default);
    Task<PatientResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<PatientResponse>> SearchAsync(string? query, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<PatientResponse> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IDepartmentService
{
    Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<DepartmentResponse> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepartmentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<DepartmentResponse>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}

public interface IConsultationService
{
    Task<ConsultationResponse> StartAsync(Guid appointmentId, Guid doctorId, CancellationToken cancellationToken = default);
    Task<ConsultationResponse> AddDiagnosisAsync(Guid appointmentId, Guid doctorId, AddDiagnosisRequest request, CancellationToken cancellationToken = default);
    Task<ConsultationResponse> AddPrescriptionAsync(Guid appointmentId, Guid doctorId, CreatePrescriptionRequest request, CancellationToken cancellationToken = default);
    Task<ConsultationResponse> CompleteAsync(Guid appointmentId, Guid doctorId, CancellationToken cancellationToken = default);
    Task<ConsultationResponse?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PatientMedicalHistoryEntry>> GetPatientMedicalHistoryAsync(
        Guid doctorId,
        Guid patientId,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PatientMedicalHistoryEntry>> GetOwnMedicalHistoryAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}

public interface IReceptionistService
{
    Task<ReceptionistResponse> CreateAsync(CreateReceptionistRequest request, CancellationToken cancellationToken = default);
    Task<ReceptionistResponse> UpdateAsync(Guid id, UpdateReceptionistRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReceptionistResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ReceptionistResponse>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string role);
}
