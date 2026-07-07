using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Receptionists;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

/// <summary>Manage receptionist accounts (Admin).</summary>
[Route("api/receptionists")]
[Authorize(Roles = "Admin")]
public class ReceptionistsController : ApiControllerBase
{
    private readonly IReceptionistService _receptionistService;

    public ReceptionistsController(IReceptionistService receptionistService) =>
        _receptionistService = receptionistService;

    /// <summary>List receptionists with pagination.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ReceptionistResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<ReceptionistResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _receptionistService.GetAllAsync(page, pageSize, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ReceptionistResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReceptionistResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _receptionistService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReceptionistResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReceptionistResponse>>> Create(
        [FromBody] CreateReceptionistRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _receptionistService.CreateAsync(request, cancellationToken);
        return OkResponse(result, "Receptionist created.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ReceptionistResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReceptionistResponse>>> Update(
        Guid id,
        [FromBody] UpdateReceptionistRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _receptionistService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result, "Receptionist updated.");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _receptionistService.DeleteAsync(id, cancellationToken);
        return OkResponse<object>(null!, "Receptionist deleted.");
    }
}
