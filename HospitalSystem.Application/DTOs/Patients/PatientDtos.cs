using HospitalSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.Application.DTOs.Patients;

public record CreatePatientRequest(
[Required(ErrorMessage = "Full Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100)]
    string FullName,

    [Required(ErrorMessage = "Date of Birth is required.")]
    DateTime DateOfBirth,

    [Required(ErrorMessage = "Gender is required.")]
    Gender Gender,

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
    string Phone,

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string? Email,
        [Required(ErrorMessage = "Address is required.")]
    string? Address,

    [RegularExpression(@"^[23][0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])(0[1-9]|[12][0-9]|3[1-5]|88)[0-9]{4}$", ErrorMessage = "Invalid Egyptian National ID format.")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits.")]
    string? NationalId,
    string? BloodType
);

public record UpdatePatientRequest(
 [Required(ErrorMessage = "Full Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100)]
    string FullName,

    [Required(ErrorMessage = "Date of Birth is required.")]
    DateTime DateOfBirth,

    [Required(ErrorMessage = "Gender is required.")]
    Gender Gender,

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
    string Phone,

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string? Email,
        [Required(ErrorMessage = "Address is required.")]
    string? Address,

    [RegularExpression(@"^[23][0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])(0[1-9]|[12][0-9]|3[1-5]|88)[0-9]{4}$", ErrorMessage = "Invalid Egyptian National ID format.")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits.")]
    string? NationalId,
    string? BloodType
);

public record PatientResponse(
    Guid Id,
    Guid? UserId,
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string Phone,
    string? Email,
    string? Address,
    string? NationalId,
    string? BloodType,
    DateTime CreatedAt);
