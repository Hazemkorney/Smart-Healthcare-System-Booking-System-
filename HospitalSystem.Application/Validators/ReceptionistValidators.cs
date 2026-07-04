using FluentValidation;
using HospitalSystem.Application.DTOs.Receptionists;

namespace HospitalSystem.Application.Validators;

public class CreateReceptionistValidator : AbstractValidator<CreateReceptionistRequest>
{
    public CreateReceptionistValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Phone)
            .Must(PhoneNumberRules.IsValidOptional)
            .WithMessage(PhoneNumberRules.ErrorMessage);
    }
}

public class UpdateReceptionistValidator : AbstractValidator<UpdateReceptionistRequest>
{
    public UpdateReceptionistValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Phone)
            .Must(PhoneNumberRules.IsValidOptional)
            .WithMessage(PhoneNumberRules.ErrorMessage);
    }
}
