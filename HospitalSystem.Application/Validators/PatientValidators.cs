using FluentValidation;
using HospitalSystem.Application.DTOs.Patients;

namespace HospitalSystem.Application.Validators;

public class CreatePatientValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");
    }
}
