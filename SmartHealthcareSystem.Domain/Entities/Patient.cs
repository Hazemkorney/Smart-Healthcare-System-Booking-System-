using SmartHealthcareSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Domain.Entitis
{
    public enum Gender
    {
        Female,
        Male
    }
     
    public class Patient: AppUser
    {
      
        public DateOnly BirthDay { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public ICollection<Appointment> Appointments { get; set; } 
    }
}