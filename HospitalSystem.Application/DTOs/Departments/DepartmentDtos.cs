using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.Application.DTOs.Departments;

public record CreateDepartmentRequest(
    [Required(ErrorMessage = "Department Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Department Name cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
    string Name,

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    string? Description
);

public record UpdateDepartmentRequest([Required(ErrorMessage = "Department Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Department Name cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
    string Name,

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    string? Description
);

public record DepartmentResponse(Guid Id, string Name, string? Description, bool IsActive);
