using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SmartHealthcareSystem.Application.DTOs.Auth;
using SmartHealthcareSystem.Application.Services.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHealthcareSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _service.Register(dto);

            return Ok(token);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _service.Login(dto);

            return Ok(token);
        }
    }
}
