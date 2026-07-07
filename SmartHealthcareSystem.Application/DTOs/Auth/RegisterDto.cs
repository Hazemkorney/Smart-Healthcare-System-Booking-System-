using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHealthcareSystem.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required,MaxLength(25)]
        public string FirstName { get; set; }
        [Required, MaxLength(25)]
        public string LastName { get; set; }

        [Required, EmailAddress,MaxLength(45)]

        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
