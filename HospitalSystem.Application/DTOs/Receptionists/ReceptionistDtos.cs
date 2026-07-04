namespace HospitalSystem.Application.DTOs.Receptionists;

public record CreateReceptionistRequest(
    string Email,
    string Password,
    string FullName,
    string? Phone);

public record UpdateReceptionistRequest(string FullName, string? Phone);

public record ReceptionistResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string? Phone,
    bool IsActive);
