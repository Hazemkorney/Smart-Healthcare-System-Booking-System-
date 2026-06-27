using FluentValidation.AspNetCore;
using HospitalSystem.API.Middleware;
using HospitalSystem.Infrastructure;
using HospitalSystem.Infrastructure.Options;
using HospitalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hospital System API",
        Version = "v1",
        Description = "Hospital Appointment Booking System — role-based API for admin, receptionist, doctor, and patient workflows."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    options.TagActionsBy(api =>
    {
        var path = api.RelativePath ?? string.Empty;
        if (path.StartsWith("api/auth")) return ["Auth"];
        if (path.StartsWith("api/departments") || path.StartsWith("api/doctors") || path.StartsWith("api/receptionists"))
            return ["Admin"];
        if (path.StartsWith("api/patients") || path.StartsWith("api/appointments")) return ["Receptionist"];
        if (path.StartsWith("api/doctor")) return ["Doctor"];
        if (path.StartsWith("api/patient")) return ["Patient"];
        return ["Other"];
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        await db.Database.MigrateAsync();
    }

    await DatabaseSeeder.SeedDatabaseAsync(app.Services);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsEnvironment("Testing"))
    app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital System API v1");
        options.DocumentTitle = "Hospital System API";
    });
}

app.UseHttpsRedirection();
app.UseCors("ReactDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
finally
{
    if (!app.Environment.IsEnvironment("Testing"))
        Log.CloseAndFlush();
}

public partial class Program { }
