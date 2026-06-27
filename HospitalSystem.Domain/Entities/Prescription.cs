namespace HospitalSystem.Domain.Entities;

public class Prescription
{
    public Guid Id { get; private set; }
    public Guid ConsultationId { get; private set; }
    public string MedicationName { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public string Frequency { get; private set; } = string.Empty;
    public string Duration { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public Consultation Consultation { get; private set; } = null!;

    private Prescription() { }

    public static Prescription Create(
        Guid consultationId,
        string medicationName,
        string dosage,
        string frequency,
        string duration,
        string? notes = null)
    {
        return new Prescription
        {
            Id = Guid.NewGuid(),
            ConsultationId = consultationId,
            MedicationName = medicationName,
            Dosage = dosage,
            Frequency = frequency,
            Duration = duration,
            Notes = notes
        };
    }
}
