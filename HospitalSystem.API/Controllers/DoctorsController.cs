using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Doctors;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

/// <summary>Manage doctors and schedules (Admin).</summary>
[Route("api/doctors")]
[Authorize]
public class DoctorsController : ApiControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService) => _doctorService = doctorService;

    /// <summary>List doctors with pagination.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<DoctorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<DoctorResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _doctorService.GetAllAsync(page, pageSize, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<DoctorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DoctorResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("department/{departmentId:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorResponse>>>> GetByDepartment(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetByDepartmentAsync(departmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("default-schedule")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorScheduleResponse>>>> GetDefaultSchedule(
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetDefaultScheduleAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("default-schedule")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorScheduleResponse>>>> SetDefaultSchedule(
        [FromBody] SetDoctorScheduleRequest request,
        CancellationToken cancellationToken)
    {
        await _doctorService.SetDefaultScheduleAsync(request, cancellationToken);
        var schedules = await _doctorService.GetDefaultScheduleAsync(cancellationToken);
        return OkResponse(schedules, "Default schedule updated.");
    }

    [HttpPost("default-schedule/apply-all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ApplyDefaultScheduleToAll(CancellationToken cancellationToken)
    {
        await _doctorService.ApplyDefaultScheduleToAllDoctorsAsync(cancellationToken);
        return OkResponse<object>(null!, "Default schedule applied to all doctors.");
    }

    [HttpGet("default-date-schedules")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>>> GetDefaultDateSchedules(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetDefaultDateSchedulesAsync(from, to, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("default-date-schedules")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDateScheduleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DoctorDateScheduleResponse>>> SetDefaultDateSchedule(
        [FromBody] SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.SetDefaultDateScheduleAsync(request, cancellationToken);
        return OkResponse(result, "Date schedule saved.");
    }

    [HttpDelete("default-date-schedules/{date}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> RemoveDefaultDateSchedule(
        DateOnly date,
        CancellationToken cancellationToken)
    {
        await _doctorService.RemoveDefaultDateScheduleAsync(date, cancellationToken);
        return OkResponse<object>(null!, "Date schedule removed.");
    }

    [HttpPost("default-date-schedules/apply-all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ApplyDefaultDateSchedulesToAll(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        await _doctorService.ApplyDefaultDateSchedulesToAllDoctorsAsync(from, to, cancellationToken);
        return OkResponse<object>(null!, "Date schedules applied to all doctors.");
    }

    [HttpPost("default-date-schedules/apply-selected")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ApplySelectedDefaultDateSchedulesToAll(
        [FromBody] ApplyDefaultDateSchedulesRequest request,
        CancellationToken cancellationToken)
    {
        await _doctorService.ApplySelectedDefaultDateSchedulesToAllDoctorsAsync(request.Dates, cancellationToken);
        return OkResponse<object>(null!, "Selected date schedules applied to all doctors.");
    }

    [HttpGet("applied-date-schedules")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>>> GetAppliedDateSchedules(
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetAppliedDateSchedulesAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("applied-date-schedules")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDateScheduleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DoctorDateScheduleResponse>>> ApplyDateScheduleToAll(
        [FromBody] SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.ApplyDateScheduleToAllDoctorsAsync(request, cancellationToken);
        return OkResponse(result, "Date schedule applied to all doctors.");
    }

    [HttpDelete("applied-date-schedules/{date}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> RemoveAppliedDateSchedule(
        DateOnly date,
        CancellationToken cancellationToken)
    {
        await _doctorService.RemoveDateScheduleFromAllDoctorsAsync(date, cancellationToken);
        return OkResponse<object>(null!, "Date schedule removed from all doctors.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DoctorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DoctorResponse>>> Create(
        [FromBody] CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.CreateAsync(request, cancellationToken);
        return OkResponse(result, "Doctor created.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DoctorResponse>>> Update(
        Guid id,
        [FromBody] UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result, "Doctor updated.");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _doctorService.DeleteAsync(id, cancellationToken);
        return OkResponse<object>(null!, "Doctor deleted.");
    }

    [HttpPost("{id:guid}/schedule")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorScheduleResponse>>>> SetSchedule(
        Guid id,
        [FromBody] SetDoctorScheduleRequest request,
        CancellationToken cancellationToken)
    {
        await _doctorService.SetScheduleAsync(id, request, cancellationToken);
        var schedules = await _doctorService.GetSchedulesAsync(id, cancellationToken);
        return OkResponse(schedules, "Schedule updated.");
    }

    [HttpGet("{id:guid}/schedule")]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorScheduleResponse>>>> GetSchedule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetSchedulesAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}/date-schedules")]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorDateScheduleResponse>>>> GetDateSchedules(
        Guid id,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetDateSchedulesAsync(id, from, to, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}/date-schedules")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDateScheduleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DoctorDateScheduleResponse>>> SetDateSchedule(
        Guid id,
        [FromBody] SetDoctorDateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.SetDateScheduleAsync(id, request, cancellationToken);
        return OkResponse(result, "Date schedule saved.");
    }

    [HttpDelete("{id:guid}/date-schedules/{date}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> RemoveDateSchedule(
        Guid id,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        await _doctorService.RemoveDateScheduleAsync(id, date, cancellationToken);
        return OkResponse<object>(null!, "Date schedule removed.");
    }
}
