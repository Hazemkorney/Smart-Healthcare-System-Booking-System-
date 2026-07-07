using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Configurations
{
    public class ReceptionistsConfigurations: IEntityTypeConfiguration<Receptionist>
    {
        public void Configure(EntityTypeBuilder<Receptionist> builder)
        {

           

        }



    }
}
