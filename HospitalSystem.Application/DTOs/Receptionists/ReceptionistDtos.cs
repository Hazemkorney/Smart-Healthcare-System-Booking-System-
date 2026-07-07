using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.Application.DTOs.Receptionists;

public record CreateReceptionistRequest(
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,
    [Required(ErrorMessage ="password is Required")]
    string Password,

 [Required(ErrorMessage = "Full Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100)]
    string FullName,

      [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
    string? Phone);

public record UpdateReceptionistRequest([RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100)]
    string FullName,

      [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
    string? Phone);

public record ReceptionistResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string? Phone,
    bool IsActive);
