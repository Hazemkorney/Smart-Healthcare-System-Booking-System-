using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHealthcareSystem.Application.DTOs.Department
{
    public class CreateDepartmentDto 
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is Required")]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
