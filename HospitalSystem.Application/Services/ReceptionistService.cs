using HospitalSystem.Application.Common;
using HospitalSystem.Application.DTOs.Receptionists;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class ReceptionistService : IReceptionistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ReceptionistService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ReceptionistResponse> CreateAsync(CreateReceptionistRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var existingUser = users.FirstOrDefault(u =>
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (existingUser is not null)
        {
            if (existingUser.IsActive || existingUser.Role != UserRole.Receptionist)
                throw new ValidationException("Email is already registered.");

            var receptionists = await _unitOfWork.Receptionists.GetAllAsync(cancellationToken);
            var receptionist = receptionists.FirstOrDefault(r => r.UserId == existingUser.Id)
                ?? throw new ValidationException("Email is already registered.");

            existingUser.Activate();
            existingUser.UpdatePassword(_passwordHasher.Hash(request.Password));
            receptionist.Activate();
            receptionist.Update(request.FullName, request.Phone);

            await _unitOfWork.Users.UpdateAsync(existingUser, cancellationToken);
            await _unitOfWork.Receptionists.UpdateAsync(receptionist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(receptionist);
        }

        var user = User.Create(request.Email, _passwordHasher.Hash(request.Password), UserRole.Receptionist);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        var newReceptionist = Receptionist.Create(user.Id, request.FullName, request.Phone);
        await _unitOfWork.Receptionists.AddAsync(newReceptionist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(newReceptionist);
    }

    public async Task<ReceptionistResponse> UpdateAsync(Guid id, UpdateReceptionistRequest request, CancellationToken cancellationToken = default)
    {
        var receptionist = await _unitOfWork.Receptionists.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Receptionist not found.");

        receptionist.Update(request.FullName, request.Phone);
        await _unitOfWork.Receptionists.UpdateAsync(receptionist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(receptionist);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var receptionist = await _unitOfWork.Receptionists.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Receptionist not found.");

        receptionist.Deactivate();
        await _unitOfWork.Receptionists.UpdateAsync(receptionist, cancellationToken);

        var user = await _unitOfWork.Users.GetByIdAsync(receptionist.UserId, cancellationToken);
        user?.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReceptionistResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var receptionist = await _unitOfWork.Receptionists.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Receptionist not found.");
        return Map(receptionist);
    }

    public async Task<PagedResponse<ReceptionistResponse>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var receptionists = (await _unitOfWork.Receptionists.GetAllAsync(cancellationToken))
            .Where(r => r.IsActive)
            .ToList();
        return Pagination.Create(receptionists.Select(Map), page, pageSize);
    }

    private static ReceptionistResponse Map(Receptionist r) =>
        new(r.Id, r.UserId, r.FullName, r.Phone, r.IsActive);
}
