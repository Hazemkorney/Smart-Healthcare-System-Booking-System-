using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, UserRole Role);

public record LoginResponse(string Token, Guid UserId, string Email, UserRole Role);

public record CurrentUserResponse(Guid UserId, string Email, UserRole Role);
