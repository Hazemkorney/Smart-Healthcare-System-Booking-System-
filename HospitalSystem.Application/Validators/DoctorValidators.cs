using FluentValidation;
using HospitalSystem.Application.DTOs.Doctors;

namespace HospitalSystem.Application.Validators;

public class CreateDoctorValidator : AbstractValidator<CreateDoctorRequest>
{
    public CreateDoctorValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Specialization).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class DoctorScheduleValidator : AbstractValidator<DoctorScheduleRequest>
{
    public DoctorScheduleValidator()
    {
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time.");
        RuleFor(x => x.AppointmentDurationMinutes)
            .InclusiveBetween(10, 120)
            .WithMessage("Appointment duration must be between 10 and 120 minutes.");
    }
}
