using SmartHealthcareSystem.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHealthcareSystem.Application.Services.Auth
{
    public interface IAuthService
    {

        Task<AuthDto> Register(RegisterDto dto);
        Task<AuthDto> Login(LoginDto dto);

    }
}
