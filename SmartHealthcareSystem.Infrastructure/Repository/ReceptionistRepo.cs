using Microsoft.EntityFrameworkCore;
using SmartHealthcareSystem.Domain.Entitis;
using SmartHealthcareSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Infrastructure.Repository
{
    public class ReceptionistRepo : SmartHealthcareSystem.Application.IRepository.IReceptionistRepo
    {

      private readonly AppDbContext _context;
        public ReceptionistRepo( AppDbContext  context)

        {
            
            _context = context;
        }
        public void Add(Receptionist receptionist)
        {
            _context.Receptionists.Add(receptionist);
        }

        public async Task<bool> EmailExists(string email)
        {
         return await _context.Receptionists.AnyAsync(r => r.Email == email);
        }

        public async Task<Receptionist> GetByEmail(string email)
        {
             return await _context.Receptionists.FirstOrDefaultAsync(r => r.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
