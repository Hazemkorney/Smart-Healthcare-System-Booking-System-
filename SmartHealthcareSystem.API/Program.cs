using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartHealthcareSystem.API.Middlewares;
using SmartHealthcareSystem.Application.IRepository;
using SmartHealthcareSystem.Application.Services.Auth;
using SmartHealthcareSystem.Application.Services.Department;
using SmartHealthcareSystem.Domain.Entities;
using SmartHealthcareSystem.Infrastructure.Data;
using SmartHealthcareSystem.Infrastructure.Repository;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDepartmentRepo, DepartmentRepo>();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<AppUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [
            new OpenApiSecuritySchemeReference("bearer", document)
        ] = []
    });
});




builder.Services.AddControllers();
builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDepartmentRepo, DepartmentRepo>();
var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddelware>();
using (var scope = app.Services.CreateScope())
{
    var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Receptionist", "Admin", "Doctor" };
    foreach (var role in roles)
    {
        if (!await RoleManager.RoleExistsAsync(role))
        {
            await RoleManager.CreateAsync(new IdentityRole(role));
        }
    }
}




app.UseMiddleware<RequestLoggingMiddelware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
