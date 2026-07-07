using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Domain.Entitis
{
    public class Department
    {

        public int Id { get; set; }
        public string Name { get; set; }
     public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    }
}
