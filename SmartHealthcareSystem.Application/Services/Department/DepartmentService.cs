using Microsoft.EntityFrameworkCore;
using SmartHealthcareSystem.Application.DTOs.Department;
using SmartHealthcareSystem.Application.Exceptions;
using SmartHealthcareSystem.Application.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.Services.Department
{

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepo _repo;
        public DepartmentService(IDepartmentRepo repo)
        {
            _repo = repo;
        }

        public async Task<ResponseDepartmentDto> Create(CreateDepartmentDto dto)
        {
            var existed = await _repo.GetAll();
            if (existed.Any(c => c.Name.ToLower() == dto.Name.ToLower()))
                throw new DuplicateException($"Department {dto.Name} is already Exist");

            var c = new SmartHealthcareSystem.Domain.Entitis.Department
            {
                Name = dto.Name
            };
            _repo.Add(c);
            await _repo.SaveChangeAsync();
            return new ResponseDepartmentDto
            {
                DepartmentName = dto.Name

            };

        }

        public async Task<bool> Delete(int id)
        {
            var dep = await _repo.GetById(id);
            if (dep is null)
                throw new NotFoundException(nameof(dep), id);

              _repo.Remove(dep);
               await _repo.SaveChangeAsync();
            return true;
        }

        public async Task<List<ResponseDepartmentDto>> GetAllDepartments()
        {
            var departments = await _repo.GetAll();
            var res = departments.Select(c => new ResponseDepartmentDto
            {
                Id = c.Id,
                DepartmentName = c.Name

            });
            return res.ToList();
            
        }

        public async Task<ResponseDepartmentDto> GetById(int dto)
        {
          var dep= await _repo.GetById(dto);
            if (dep == null)
                throw new NotFoundException($"Department with id {dto} and is not found");
            return new ResponseDepartmentDto
            {
                Id = dep.Id,
                DepartmentName = dep.Name
            };

        }

        public  async Task<ResponseDepartmentDto> GetByName(string name)
        {
            var dep = await _repo.GetByName(name);
            if (dep == null)
                throw new NotFoundException($"Department with this Name {name} is not found");
            return new ResponseDepartmentDto
            {
                Id = dep.Id,
                DepartmentName = dep.Name
            };
        }

        public async Task<ResponseDepartmentDto> Update(int id, UpdateDepartmentDto dto)
        {
            var department = await _repo.GetById(id);

            if (department is null)
                throw new NotFoundException("Department", id);

            department.Name = dto.Name;

            var updatedDepartment = await _repo.Update(id, department);

            await _repo.SaveChangeAsync();
                
            return new ResponseDepartmentDto
            {
                Id = updatedDepartment.Id,
                DepartmentName = updatedDepartment.Name
            };
        }
    }
}
