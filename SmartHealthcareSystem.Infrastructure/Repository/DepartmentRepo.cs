using Microsoft.EntityFrameworkCore;
using SmartHealthcareSystem.Application.DTOs.Department;
using SmartHealthcareSystem.Application.Exceptions;
using SmartHealthcareSystem.Application.IRepository;
using SmartHealthcareSystem.Domain.Entitis;
using SmartHealthcareSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace SmartHealthcareSystem.Infrastructure.Repository
{
    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly AppDbContext _context;

        public DepartmentRepo(AppDbContext context)
        {
            _context = context;
            
        }
        public async Task Add(Department department)
        {
             await _context.Departments.AddAsync(department);
           
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetById(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<Department> GetByName(string name)
        {
            return await _context.Departments.FindAsync(name);
        }

        public  void  Remove(Department department)
        {
             _context.Departments.Remove(department);
           
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Department> Update(int id, Department department)
        {
            var dep = await _context.Departments.FindAsync(id);

            if (dep == null)
                throw new NotFoundException(nameof(dep), id);

            var exist = await _context.Departments.AnyAsync(c => c.Name.ToLower() == department.Name.ToLower());

            if (exist)
                throw new DuplicateException($"Departments {department.Name} is Already Exist");

            dep.Name = department.Name;

            return await _context.Departments.FindAsync(id);

        }
    }
}
