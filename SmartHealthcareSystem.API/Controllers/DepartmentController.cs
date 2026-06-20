using Microsoft.AspNetCore.Mvc;
using SmartHealthcareSystem.Application.DTOs.Department;
using SmartHealthcareSystem.Application.Services.Department;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHealthcareSystem.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _Service;
        public DepartmentController(IDepartmentService service)
        {
            _Service = service;
        }

        // GET: api/<DepartmentController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments =await _Service.GetAllDepartments();
            return Ok(departments);

        }

        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department =  await _Service.GetById(id);
            return Ok(department);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var department = await _Service.GetByName(name);
            return Ok(department);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateDepartmentDto name)
        {
          var dep=  await _Service.Create(name);
            return Ok(dep);
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateDepartmentDto value)
        {
            var department = await _Service.GetById(id);
            await _Service.Update(id, value);
            return Ok(department);
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
           var department =  await _Service.Delete(id);
            return NoContent();
        }
    }
}
