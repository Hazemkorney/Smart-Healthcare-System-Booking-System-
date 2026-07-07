using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Domain.Entitis
{
    public enum StatuS
    {
        Pending, 
        Completed,
        Cancelled
    }

    public class Appointment
        {
            public int Id { get; set; }

            public DateTime AppointmentDate { get; set; }

            public StatuS Status { get; set; }
            public string PatientId { get; set; }

            public string DoctorId { get; set; }
            public Patient Patient { get; set; }
            public Doctor Doctor { get; set; }
        }
    }

