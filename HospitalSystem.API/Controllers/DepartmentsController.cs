using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Departments;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

/// <summary>Manage hospital departments (Admin).</summary>
[Route("api/departments")]
[Authorize]
public class DepartmentsController : ApiControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService) =>
        _departmentService = departmentService;

    /// <summary>List departments with pagination.</summary>
    /// <response code="200">Paged list of departments.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<DepartmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<DepartmentResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetAllAsync(page, pageSize, cancellationToken);
        return OkResponse(result);
    }

    /// <summary>Get a department by id.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Receptionist")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    /// <summary>Create a department.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _departmentService.CreateAsync(request, cancellationToken);
        return OkResponse(result, "Department created.");
    }

    /// <summary>Update a department.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> Update(
        Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _departmentService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result, "Department updated.");
    }

    /// <summary>Soft-delete a department.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _departmentService.DeleteAsync(id, cancellationToken);
        return OkResponse<object>(null!, "Department deleted.");
    }
}
