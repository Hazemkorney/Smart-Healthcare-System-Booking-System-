using FluentValidation;
using HospitalSystem.Application.DTOs.Appointments;

namespace HospitalSystem.Application.Validators;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.AppointmentDate)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Appointment date cannot be in the past.");
        RuleFor(x => x.StartTime).NotEmpty();
    }
}

public class RescheduleValidator : AbstractValidator<RescheduleRequest>
{
    public RescheduleValidator()
    {
        RuleFor(x => x.NewDate)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("New appointment date cannot be in the past.");
        RuleFor(x => x.NewStartTime).NotEmpty();
    }
}
