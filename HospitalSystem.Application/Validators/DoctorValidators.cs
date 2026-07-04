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
        RuleFor(x => x.Phone)
            .Must(PhoneNumberRules.IsValidOptional)
            .WithMessage(PhoneNumberRules.ErrorMessage);
    }
}

public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorRequest>
{
    public UpdateDoctorValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Specialization).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.Phone)
            .Must(PhoneNumberRules.IsValidOptional)
            .WithMessage(PhoneNumberRules.ErrorMessage);
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

public class DoctorDateScheduleValidator : AbstractValidator<SetDoctorDateScheduleRequest>
{
    public DoctorDateScheduleValidator()
    {
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time.");
        RuleFor(x => x.AppointmentDurationMinutes)
            .InclusiveBetween(10, 120)
            .WithMessage("Appointment duration must be between 10 and 120 minutes.");
        RuleFor(x => x.Date)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Cannot set schedule for a past date.");
        RuleFor(x => x)
            .Must(x => !IsToday(x.Date) || x.StartTime > DateTime.UtcNow.TimeOfDay)
            .WithMessage("Start time cannot be in the past for today.");
        RuleFor(x => x)
            .Must(x => !IsToday(x.Date) || x.EndTime > DateTime.UtcNow.TimeOfDay)
            .WithMessage("End time cannot be in the past for today.");
    }

    private static bool IsToday(DateOnly date) =>
        date == DateOnly.FromDateTime(DateTime.UtcNow);
}
