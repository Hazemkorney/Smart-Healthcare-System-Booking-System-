namespace HospitalSystem.Application.Validators;

public static class BloodTypeRules
{
    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
    };

    public const string ErrorMessage = "Select a valid blood type (A+, A-, B+, B-, AB+, AB-, O+, O-).";

    public static bool IsValidOptional(string? bloodType)
    {
        if (string.IsNullOrWhiteSpace(bloodType))
            return true;

        return ValidTypes.Contains(bloodType.Trim());
    }
}
