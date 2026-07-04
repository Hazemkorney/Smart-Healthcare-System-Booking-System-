using FluentValidation;
using HospitalSystem.Application.DTOs.Consultations;

namespace HospitalSystem.Application.Validators;

public class AddDiagnosisValidator : AbstractValidator<AddDiagnosisRequest>
{
    public AddDiagnosisValidator()
    {
        RuleFor(x => x.Diagnosis).NotEmpty().WithMessage("Diagnosis is required.");
    }
}

public class CreatePrescriptionValidator : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionValidator()
    {
        RuleFor(x => x.MedicationName).NotEmpty();
        RuleFor(x => x.Dosage).NotEmpty();
        RuleFor(x => x.Frequency).NotEmpty();
        RuleFor(x => x.Duration).NotEmpty();
    }
}
