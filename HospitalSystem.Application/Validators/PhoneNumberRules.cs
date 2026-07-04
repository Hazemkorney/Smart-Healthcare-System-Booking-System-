using System.Text.RegularExpressions;

namespace HospitalSystem.Application.Validators;

public static partial class PhoneNumberRules
{
    public const string ErrorMessage = "Enter a valid phone number (7–15 digits, e.g. 01xxxxxxxxx).";

    [GeneratedRegex(@"^\+?[0-9\s\-().]+$")]
    private static partial Regex AllowedFormatRegex();

    public static bool IsValid(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        if (!AllowedFormatRegex().IsMatch(phone))
            return false;

        var digitCount = phone.Count(char.IsDigit);
        return digitCount is >= 7 and <= 15;
    }

    public static bool IsValidOptional(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;

        return IsValid(phone);
    }
}
