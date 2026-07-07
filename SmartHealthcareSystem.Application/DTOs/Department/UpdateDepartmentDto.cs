using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHealthcareSystem.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(100)]
        public string Name { get; set; }

    }
}
