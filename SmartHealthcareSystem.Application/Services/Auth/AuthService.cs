using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHealthcareSystem.Application.DTOs.Auth;
using SmartHealthcareSystem.Application.Exceptions;
using SmartHealthcareSystem.Application.IRepository;
using SmartHealthcareSystem.Domain.Entities;
using SmartHealthcareSystem.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHealthcareSystem.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public AuthService(IConfiguration config,UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager  = userManager;
        }
        public async Task<AuthDto> Register(RegisterDto dto)
        {
            var exit= await _userManager.FindByEmailAsync(dto.Email);
            if(exit != null)
                throw new DuplicateException("Email already exists");
            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = (dto.FirstName + dto.LastName).ToLower()
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            // 1. لازم تفحص النتيجة هنا قبل ما تدي أي Role
            if (!result.Succeeded)
            {
                // ده هيجمع كل الأخطاء (زي: الباسورد ضعيف) ويرميها عشان تشوفها في Postman
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"فشل في إنشاء المستخدم: {errors}");
            }

            // 2. السطر ده مش هيشتغل أبداً إلا لو المستخدم فعلاً نزل في الداتابيز بنجاح
            await _userManager.AddToRoleAsync(user, "Receptionist");

            return new AuthDto {
                Token = await GenerateToken(user)
            };


        }
        public  async Task<AuthDto> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new BadRequestException("Invalid Email Or password");

            var validpass = await _userManager.CheckPasswordAsync(user, dto.Password);
            if(!validpass)
             throw new BadRequestException("Invalid Email Or password");
            return new AuthDto
            {
                Token = await GenerateToken(user)
            };



        }

        private async Task<string> GenerateToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var claims =  new List<Claim>
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

        new Claim(ClaimTypes.Email, user.Email)
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    int.Parse(_config["Jwt:ExpirationHours"]!)
                ),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha512Signature
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }





    }
}
