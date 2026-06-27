using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.DTOs.Patients;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

[Route("api/patient")]
[Authorize(Roles = "Patient")]
public class PatientController : ApiControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IAppointmentService _appointmentService;
    private readonly IConsultationService _consultationService;

    public PatientController(
        IPatientService patientService,
        IAppointmentService appointmentService,
        IConsultationService consultationService)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
        _consultationService = consultationService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<PatientResponse>>> GetProfile(
        CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByUserIdAsync(GetUserId(), cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("appointments")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentResponse>>>> GetAppointments(
        CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var result = await _appointmentService.GetByPatientIdAsync(patient.Id, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("appointments/{id:guid}")]
    public async Task<ActionResult<ApiResponse<AppointmentDetailResponse>>> GetAppointmentDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByUserIdAsync(GetUserId(), cancellationToken);
        var appointment = await _appointmentService.GetByIdAsync(id, cancellationToken);

        if (appointment.PatientId != patient.Id)
            throw new ForbiddenException("You can only access your own appointments.");

        var consultation = await _consultationService.GetByAppointmentIdAsync(id, cancellationToken);
        return OkResponse(new AppointmentDetailResponse(appointment, consultation));
    }
}
