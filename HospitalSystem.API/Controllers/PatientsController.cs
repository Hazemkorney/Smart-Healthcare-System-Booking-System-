using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Patients;
using HospitalSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.API.Controllers;

/// <summary>Patient registration and search (Receptionist).</summary>
[Route("api/patients")]
[Authorize(Roles = "Receptionist")]
public class PatientsController : ApiControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService) => _patientService = patientService;

    /// <summary>Search patients with optional query and pagination.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PatientResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<PatientResponse>>>> Search(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _patientService.SearchAsync(query, page, pageSize, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PatientResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByIdAsync(id, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PatientResponse>>> Create(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.CreateAsync(request, cancellationToken);
        return OkResponse(result, "Patient registered.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientResponse>>> Update(
        Guid id,
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result, "Patient updated.");
    }
}
