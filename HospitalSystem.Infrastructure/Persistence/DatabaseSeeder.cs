using HospitalSystem.Application.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HospitalSystem.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly HospitalDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        HospitalDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded. Ensuring demo accounts exist.");
            await EnsurePatientLoginAsync(cancellationToken);
            await EnsureDefaultDoctorScheduleAsync(cancellationToken);
            return;
        }

        _logger.LogInformation("Seeding database...");

        var admin = User.Create("admin@hospital.com", _passwordHasher.Hash("Admin@123"), UserRole.Admin);
        await _context.Users.AddAsync(admin, cancellationToken);

        var cardiology = Department.Create("Cardiology", "Heart and cardiovascular care");
        var orthopedics = Department.Create("Orthopedics", "Bone and joint care");
        await _context.Departments.AddAsync(cardiology, cancellationToken);
        await _context.Departments.AddAsync(orthopedics, cancellationToken);

        var doctorUser1 = User.Create("dr.smith@hospital.com", _passwordHasher.Hash("Doctor@123"), UserRole.Doctor);
        var doctorUser2 = User.Create("dr.jones@hospital.com", _passwordHasher.Hash("Doctor@123"), UserRole.Doctor);
        await _context.Users.AddAsync(doctorUser1, cancellationToken);
        await _context.Users.AddAsync(doctorUser2, cancellationToken);

        var doctor1 = Doctor.Create(doctorUser1.Id, cardiology.Id, "Dr. John Smith", "Cardiologist", "555-0101");
        var doctor2 = Doctor.Create(doctorUser2.Id, orthopedics.Id, "Dr. Sarah Jones", "Orthopedic Surgeon", "555-0102");
        await _context.Doctors.AddAsync(doctor1, cancellationToken);
        await _context.Doctors.AddAsync(doctor2, cancellationToken);

        var workDays = new[]
        {
            DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
        };

        foreach (var day in workDays)
        {
            var defaultSchedule = DefaultDoctorSchedule.Create(
                day,
                new TimeSpan(9, 0, 0),
                new TimeSpan(17, 0, 0),
                30);
            await _context.DefaultDoctorSchedules.AddAsync(defaultSchedule, cancellationToken);
        }

        foreach (var doctor in new[] { doctor1, doctor2 })
        {
            foreach (var day in workDays)
            {
                var schedule = DoctorSchedule.Create(
                    doctor.Id, day,
                    new TimeSpan(9, 0, 0),
                    new TimeSpan(17, 0, 0),
                    30);
                await _context.DoctorSchedules.AddAsync(schedule, cancellationToken);
            }
        }

        var receptionistUser = User.Create("reception@hospital.com", _passwordHasher.Hash("Reception@123"), UserRole.Receptionist);
        await _context.Users.AddAsync(receptionistUser, cancellationToken);

        var receptionist = Receptionist.Create(receptionistUser.Id, "Jane Wilson", "555-0200");
        await _context.Receptionists.AddAsync(receptionist, cancellationToken);

        var patientUser = User.Create("patient@hospital.com", _passwordHasher.Hash("Patient@123"), UserRole.Patient);
        await _context.Users.AddAsync(patientUser, cancellationToken);

        var patients = new[]
        {
            Patient.Create("Alice Johnson", new DateTime(1990, 5, 15), Gender.Female, "555-1001", "alice@email.com", bloodType: "A+", userId: patientUser.Id),
            Patient.Create("Bob Williams", new DateTime(1985, 8, 22), Gender.Male, "555-1002", "bob@email.com", bloodType: "O+"),
            Patient.Create("Carol Davis", new DateTime(1978, 12, 3), Gender.Female, "555-1003", bloodType: "B+")
        };
        foreach (var patient in patients)
            await _context.Patients.AddAsync(patient, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Database seeded successfully.");
    }

    private async Task EnsurePatientLoginAsync(CancellationToken cancellationToken)
    {
        const string patientEmail = "patient@hospital.com";
        const string patientPassword = "Patient@123";

        var patientUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == patientEmail, cancellationToken);

        if (patientUser is null)
        {
            _logger.LogInformation("Adding missing demo patient user {Email}.", patientEmail);
            patientUser = User.Create(patientEmail, _passwordHasher.Hash(patientPassword), UserRole.Patient);
            await _context.Users.AddAsync(patientUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var linkedPatient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == patientUser.Id, cancellationToken);

        if (linkedPatient is not null)
            return;

        var alice = await _context.Patients
            .FirstOrDefaultAsync(p => p.FullName == "Alice Johnson", cancellationToken);

        if (alice is not null)
        {
            alice.LinkUser(patientUser.Id);
            _logger.LogInformation("Linked demo patient user to Alice Johnson profile.");
        }
        else
        {
            var profile = Patient.Create(
                "Alice Johnson",
                new DateTime(1990, 5, 15),
                Gender.Female,
                "555-1001",
                "alice@email.com",
                bloodType: "A+",
                userId: patientUser.Id);
            await _context.Patients.AddAsync(profile, cancellationToken);
            _logger.LogInformation("Created Alice Johnson profile for demo patient user.");
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDefaultDoctorScheduleAsync(CancellationToken cancellationToken)
    {
        if (await _context.DefaultDoctorSchedules.AnyAsync(cancellationToken))
            return;

        _logger.LogInformation("Seeding default doctor schedule.");
        var workDays = new[]
        {
            DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
        };

        foreach (var day in workDays)
        {
            var defaultSchedule = DefaultDoctorSchedule.Create(
                day,
                new TimeSpan(9, 0, 0),
                new TimeSpan(17, 0, 0),
                30);
            await _context.DefaultDoctorSchedules.AddAsync(defaultSchedule, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public static async Task SeedDatabaseAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}
