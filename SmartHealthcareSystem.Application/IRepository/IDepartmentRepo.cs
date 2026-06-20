using SmartHealthcareSystem.Application.DTOs.Department;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.IRepository
{
    public interface IDepartmentRepo
    {
        Task<IEnumerable<Department>> GetAll();
        Task<Department> GetById(int id);
        Task<Department> GetByName(string name);
        Task Add(Department department);
        void Remove( Department department);
        Task<Department> Update(int id, Department department);
        Task SaveChangeAsync();

    }
}
