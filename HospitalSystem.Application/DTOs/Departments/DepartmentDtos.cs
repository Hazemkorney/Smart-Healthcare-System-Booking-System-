namespace HospitalSystem.Application.DTOs.Departments;

public record CreateDepartmentRequest(string Name, string? Description);

public record UpdateDepartmentRequest(string Name, string? Description);

public record DepartmentResponse(Guid Id, string Name, string? Description, bool IsActive);
