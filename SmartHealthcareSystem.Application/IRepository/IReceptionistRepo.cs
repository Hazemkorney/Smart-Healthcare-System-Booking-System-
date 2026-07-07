using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.IRepository
{
    public interface IReceptionistRepo
    {
        void Add(Domain.Entitis.Receptionist receptionist);
        Task SaveChangesAsync();
        Task<Domain.Entitis.Receptionist> GetByEmail(string email);
        Task<bool>EmailExists(string email);
    }
}
