using FluentValidation;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IConsultationService, ConsultationService>();
        services.AddScoped<IReceptionistService, ReceptionistService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        return services;
    }
}
