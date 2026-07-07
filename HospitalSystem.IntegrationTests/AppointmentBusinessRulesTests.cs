using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace HospitalSystem.IntegrationTests;

public class AppointmentBusinessRulesTests(HospitalApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task BookPastAppointment_Returns400WithClearMessage()
    {
        var token = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        var response = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = pastDate.ToString("yyyy-MM-dd"),
            startTime = "09:00:00",
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var message = await ReadErrorMessageAsync(response);
        Assert.Contains("past", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DoubleBooking_Returns400SlotNotAvailable()
    {
        var token = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var date = NextWeekday();

        var request = new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = "09:00:00",
            notes = (string?)null
        };

        var first = await Client.PostAsJsonAsync("/api/appointments", request);
        first.EnsureSuccessStatusCode();

        var second = await Client.PostAsJsonAsync("/api/appointments", request);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);

        var message = await ReadErrorMessageAsync(second);
        Assert.Contains("Slot not available", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DoctorBookingEndpoint_Returns403Forbidden()
    {
        var token = await LoginAsync("dr.smith@hospital.com", "Doctor@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var date = NextWeekday();

        var response = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = "10:00:00",
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatientBookingEndpoint_Returns403Forbidden()
    {
        var token = await LoginAsync("patient@hospital.com", "Patient@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var date = NextWeekday();

        var response = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = "10:00:00",
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatientDoubleBookingDifferentDoctor_Returns400()
    {
        var token = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var secondDoctorId = await GetSecondDoctorIdAsync();
        var date = NextWeekday();
        await EnsureDoctorDateScheduleAsync(doctorId, date);
        await EnsureDoctorDateScheduleAsync(secondDoctorId, date);
        const string slotTime = "10:00:00";

        var first = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = slotTime,
            notes = (string?)null
        });
        first.EnsureSuccessStatusCode();

        var second = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId = secondDoctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = slotTime,
            notes = (string?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        var message = await ReadErrorMessageAsync(second);
        Assert.Contains("Patient already has another appointment at this time", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CancelledAppointmentSlot_AppearsInAvailableSlots()
    {
        var token = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var date = NextWeekday();
        await EnsureDoctorDateScheduleAsync(doctorId, date);
        const string slotTime = "11:00:00";

        var bookResponse = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = slotTime,
            notes = (string?)null
        });
        bookResponse.EnsureSuccessStatusCode();

        var booked = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var appointmentId = booked.GetProperty("data").GetProperty("id").GetGuid();

        var slotsBeforeCancel = await GetSlotStartTimesAsync(doctorId, date, patientId);
        Assert.DoesNotContain("11:00:00", slotsBeforeCancel);

        var cancelResponse = await Client.PutAsync($"/api/appointments/{appointmentId}/cancel", null);
        cancelResponse.EnsureSuccessStatusCode();

        var slotsAfterCancel = await GetSlotStartTimesAsync(doctorId, date, patientId);
        Assert.Contains("11:00:00", slotsAfterCancel);
    }

    [Fact]
    public async Task AvailableSlots_RespectDoctorWorkingHours()
    {
        var token = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(token);

        var (patientId, doctorId) = await GetSeededIdsAsync();
        var date = NextWeekday();
        await EnsureDoctorDateScheduleAsync(doctorId, date);

        var response = await Client.GetAsync(
            $"/api/appointments/available-slots?doctorId={doctorId}&date={date:yyyy-MM-dd}&patientId={patientId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var slots = json.GetProperty("data").EnumerateArray().ToList();

        Assert.NotEmpty(slots);

        foreach (var slot in slots)
        {
            var start = TimeSpan.Parse(slot.GetProperty("startTime").GetString()!);
            var end = TimeSpan.Parse(slot.GetProperty("endTime").GetString()!);

            Assert.True(start >= new TimeSpan(9, 0, 0));
            Assert.True(end <= new TimeSpan(17, 0, 0));
            Assert.Equal(TimeSpan.FromMinutes(30), end - start);
        }

        var firstStart = TimeSpan.Parse(slots[0].GetProperty("startTime").GetString()!);
        var lastEnd = TimeSpan.Parse(slots[^1].GetProperty("endTime").GetString()!);
        Assert.Equal(new TimeSpan(9, 0, 0), firstStart);
        Assert.Equal(new TimeSpan(17, 0, 0), lastEnd);
    }

    private async Task<List<string>> GetSlotStartTimesAsync(Guid doctorId, DateOnly date, Guid patientId)
    {
        var response = await Client.GetAsync(
            $"/api/appointments/available-slots?doctorId={doctorId}&date={date:yyyy-MM-dd}&patientId={patientId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("data").EnumerateArray()
            .Select(s => s.GetProperty("startTime").GetString()!)
            .ToList();
    }
}
