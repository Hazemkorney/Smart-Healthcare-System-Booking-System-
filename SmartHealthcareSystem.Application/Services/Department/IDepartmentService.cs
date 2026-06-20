using SmartHealthcareSystem.Application.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.Services.Department
{
    public interface IDepartmentService
    {
        Task<List<ResponseDepartmentDto>> GetAllDepartments();
        Task<ResponseDepartmentDto> GetById(int id);
        Task<ResponseDepartmentDto> GetByName(string name);
        Task<ResponseDepartmentDto> Create(CreateDepartmentDto dto);
        Task<ResponseDepartmentDto> Update(int id ,UpdateDepartmentDto dto);
        Task<bool> Delete(int id);



    }
}
