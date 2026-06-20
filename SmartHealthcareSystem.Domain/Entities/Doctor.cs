using SmartHealthcareSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Domain.Entitis
{
    public class Doctor: AppUser
    {
       
        public string Specialization { get; set; }

        public decimal ConsultationFee { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
