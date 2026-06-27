using System.Net.Http.Json;
using System.Text.Json;

namespace HospitalSystem.IntegrationTests;

public class EndToEndFlowTests(HospitalApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task FullAppointmentWorkflow_Succeeds()
    {
        // 1. Admin creates department and doctor with schedule (use seeded data)
        var adminToken = await LoginAsync("admin@hospital.com", "Admin@123");
        Authenticate(adminToken);

        var deptResponse = await Client.PostAsJsonAsync("/api/departments", new
        {
            name = "Neurology",
            description = "Brain care"
        });
        deptResponse.EnsureSuccessStatusCode();

        // 2. Receptionist registers patient and books appointment
        var receptionToken = await LoginAsync("reception@hospital.com", "Reception@123");
        Authenticate(receptionToken);

        var patientResponse = await Client.PostAsJsonAsync("/api/patients", new
        {
            fullName = "Test Patient E2E",
            dateOfBirth = "1995-03-15",
            gender = "Male",
            phone = "555-9999",
            email = "e2e@test.com"
        });
        patientResponse.EnsureSuccessStatusCode();
        var patientJson = await patientResponse.Content.ReadFromJsonAsync<JsonElement>();
        var patientId = patientJson.GetProperty("data").GetProperty("id").GetGuid();

        var doctorsResponse = await Client.GetAsync("/api/doctors");
        doctorsResponse.EnsureSuccessStatusCode();
        var doctorsJson = await doctorsResponse.Content.ReadFromJsonAsync<JsonElement>();
        var doctorId = doctorsJson.GetProperty("data").GetProperty("data").EnumerateArray().First().GetProperty("id").GetGuid();

        var date = NextWeekday();
        var bookResponse = await Client.PostAsJsonAsync("/api/appointments", new
        {
            patientId,
            doctorId,
            appointmentDate = date.ToString("yyyy-MM-dd"),
            startTime = "14:00:00",
            notes = "E2E test"
        });
        bookResponse.EnsureSuccessStatusCode();
        var bookJson = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var appointmentId = bookJson.GetProperty("data").GetProperty("id").GetGuid();

        // 3. Check-in
        var checkInResponse = await Client.PutAsync($"/api/appointments/{appointmentId}/checkin", null);
        checkInResponse.EnsureSuccessStatusCode();

        // 4. Doctor consultation workflow
        var doctorToken = await LoginAsync("dr.smith@hospital.com", "Doctor@123");
        Authenticate(doctorToken);

        var startResponse = await Client.PostAsync($"/api/doctor/appointments/{appointmentId}/start", null);
        startResponse.EnsureSuccessStatusCode();

        var diagnosisResponse = await Client.PutAsJsonAsync($"/api/doctor/appointments/{appointmentId}/diagnosis", new
        {
            diagnosis = "Hypertension",
            notes = "Monitor blood pressure"
        });
        diagnosisResponse.EnsureSuccessStatusCode();

        var prescriptionResponse = await Client.PostAsJsonAsync($"/api/doctor/appointments/{appointmentId}/prescriptions", new
        {
            medicationName = "Lisinopril",
            dosage = "10mg",
            frequency = "Once daily",
            duration = "30 days"
        });
        prescriptionResponse.EnsureSuccessStatusCode();

        var completeResponse = await Client.PutAsync($"/api/doctor/appointments/{appointmentId}/complete", null);
        completeResponse.EnsureSuccessStatusCode();

        // 5. Patient views history (seeded patient user)
        var patientToken = await LoginAsync("patient@hospital.com", "Patient@123");
        Authenticate(patientToken);

        var historyResponse = await Client.GetAsync("/api/patient/appointments");
        historyResponse.EnsureSuccessStatusCode();
    }
}
