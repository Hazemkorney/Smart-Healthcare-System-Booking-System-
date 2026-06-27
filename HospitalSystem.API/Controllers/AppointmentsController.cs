using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

[Route("api/appointments")]
[Authorize(Roles = "Receptionist")]
public class AppointmentsController : ApiControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService) =>
        _appointmentService = appointmentService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentResponse>>>> GetSchedule(
        [FromQuery] Guid doctorId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetDoctorScheduleAsync(doctorId, date, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("available-slots")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AvailableSlotResponse>>>> GetAvailableSlots(
        [FromQuery] Guid doctorId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetAvailableSlotsAsync(doctorId, date, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AppointmentResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentResponse>>> Book(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.BookAppointmentAsync(request, GetUserId(), cancellationToken);
        return OkResponse(result, "Appointment booked.");
    }

    [HttpPut("{id:guid}/reschedule")]
    public async Task<ActionResult<ApiResponse<AppointmentResponse>>> Reschedule(
        Guid id,
        [FromBody] RescheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.RescheduleAsync(id, request, GetUserId(), cancellationToken);
        return OkResponse(result, "Appointment rescheduled.");
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await _appointmentService.CancelAsync(id, GetUserId(), cancellationToken);
        return OkResponse<object>(null!, "Appointment cancelled.");
    }

    [HttpPut("{id:guid}/checkin")]
    public async Task<ActionResult<ApiResponse<AppointmentResponse>>> CheckIn(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CheckInAsync(id, cancellationToken);
        return OkResponse(result, "Patient checked in.");
    }
}
