using HospitalSystem.Application.DTOs.Auth;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u =>
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && u.IsActive);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new LoginResponse(token, user.Id, user.Email, user.Role);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException("Email is already registered.");

        var user = Domain.Entities.User.Create(
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.Role);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new LoginResponse(token, user.Id, user.Email, user.Role);
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return new CurrentUserResponse(user.Id, user.Email, user.Role);
    }
}
