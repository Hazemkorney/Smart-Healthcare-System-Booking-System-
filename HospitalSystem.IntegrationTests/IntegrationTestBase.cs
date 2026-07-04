using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalSystem.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<HospitalApiFactory>, IAsyncLifetime
{
    protected readonly HospitalApiFactory Factory;
    protected HttpClient Client = null!;

    protected IntegrationTestBase(HospitalApiFactory factory) => Factory = factory;

    public async Task InitializeAsync()
    {
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        Client.DefaultRequestHeaders.Clear();
        await Factory.ResetAndSeedAsync();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<string> LoginAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("data").GetProperty("token").GetString()!;
    }

    protected void Authenticate(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<(Guid PatientId, Guid DoctorId)> GetSeededIdsAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

        var patient = db.Patients.First(p => p.FullName == "Alice Johnson");
        var doctor = db.Doctors.First(d => d.FullName == "Dr. John Smith");

        return (patient.Id, doctor.Id);
    }

    protected async Task<Guid> GetSecondDoctorIdAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        return db.Doctors.First(d => d.FullName == "Dr. Sarah Jones").Id;
    }

    protected async Task EnsureDoctorDateScheduleAsync(Guid doctorId, DateOnly date)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        if (db.DoctorDateSchedules.Any(s => s.DoctorId == doctorId && s.ScheduleDate == date))
            return;

        db.DoctorDateSchedules.Add(DoctorDateSchedule.Create(
            doctorId,
            date,
            new TimeSpan(9, 0, 0),
            new TimeSpan(17, 0, 0),
            30));
        await db.SaveChangesAsync();
    }

    protected static DateOnly NextWeekday()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        while (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            date = date.AddDays(1);
        return date;
    }

    protected static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (json.TryGetProperty("message", out var message))
            return message.GetString() ?? string.Empty;
        return await response.Content.ReadAsStringAsync();
    }
}
