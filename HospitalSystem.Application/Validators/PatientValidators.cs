using FluentValidation;
using HospitalSystem.Application.DTOs.Patients;

namespace HospitalSystem.Application.Validators;

public class CreatePatientValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Must(PhoneNumberRules.IsValid)
            .WithMessage(PhoneNumberRules.ErrorMessage);
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.NationalId).NotEmpty();
        RuleFor(x => x.BloodType)
            .NotEmpty()
            .Must(BloodTypeRules.IsValidOptional)
            .WithMessage(BloodTypeRules.ErrorMessage);
    }
}

public class UpdatePatientValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Must(PhoneNumberRules.IsValid)
            .WithMessage(PhoneNumberRules.ErrorMessage);
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.NationalId).NotEmpty();
        RuleFor(x => x.BloodType)
            .NotEmpty()
            .Must(BloodTypeRules.IsValidOptional)
            .WithMessage(BloodTypeRules.ErrorMessage);
    }
}
