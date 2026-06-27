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
}
