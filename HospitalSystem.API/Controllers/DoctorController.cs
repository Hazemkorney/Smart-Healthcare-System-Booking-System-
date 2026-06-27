using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.DTOs.Consultations;
using HospitalSystem.Application.DTOs.Patients;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

[Route("api/doctor")]
[Authorize(Roles = "Doctor")]
public class DoctorController : ApiControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IAppointmentService _appointmentService;
    private readonly IConsultationService _consultationService;
    private readonly IPatientService _patientService;

    public DoctorController(
        IDoctorService doctorService,
        IAppointmentService appointmentService,
        IConsultationService consultationService,
        IPatientService patientService)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;
        _consultationService = consultationService;
        _patientService = patientService;
    }

    [HttpGet("schedule")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentResponse>>>> GetSchedule(
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _appointmentService.GetDoctorScheduleAsync(doctor.Id, date, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("appointments/{id:guid}")]
    public async Task<ActionResult<ApiResponse<DoctorAppointmentDetailResponse>>> GetAppointment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var appointment = await _appointmentService.GetByIdAsync(id, cancellationToken);

        if (appointment.DoctorId != doctor.Id)
            throw new ForbiddenException("You can only access your own appointments.");

        var patient = await _patientService.GetByIdAsync(appointment.PatientId, cancellationToken);
        var consultation = await _consultationService.GetByAppointmentIdAsync(id, cancellationToken);

        return OkResponse(new DoctorAppointmentDetailResponse(appointment, patient, consultation));
    }

    [HttpPost("appointments/{id:guid}/start")]
    public async Task<ActionResult<ApiResponse<ConsultationResponse>>> Start(
        Guid id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _consultationService.StartAsync(id, doctor.Id, cancellationToken);
        return OkResponse(result, "Consultation started.");
    }

    [HttpPut("appointments/{id:guid}/diagnosis")]
    public async Task<ActionResult<ApiResponse<ConsultationResponse>>> AddDiagnosis(
        Guid id,
        [FromBody] AddDiagnosisRequest request,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _consultationService.AddDiagnosisAsync(id, doctor.Id, request, cancellationToken);
        return OkResponse(result, "Diagnosis added.");
    }

    [HttpPost("appointments/{id:guid}/prescriptions")]
    public async Task<ActionResult<ApiResponse<ConsultationResponse>>> AddPrescription(
        Guid id,
        [FromBody] CreatePrescriptionRequest request,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _consultationService.AddPrescriptionAsync(id, doctor.Id, request, cancellationToken);
        return OkResponse(result, "Prescription added.");
    }

    [HttpPut("appointments/{id:guid}/complete")]
    public async Task<ActionResult<ApiResponse<ConsultationResponse>>> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _consultationService.CompleteAsync(id, doctor.Id, cancellationToken);
        return OkResponse(result, "Appointment completed.");
    }
}

public record DoctorAppointmentDetailResponse(
    AppointmentResponse Appointment,
    PatientResponse Patient,
    ConsultationResponse? Consultation);
