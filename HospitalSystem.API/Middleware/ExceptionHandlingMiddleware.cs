using System.Net;
using System.Text.Json;
using HospitalSystem.Application.Common;
using AppValidationException = HospitalSystem.Application.Exceptions.ValidationException;
using FluentValidationException = FluentValidation.ValidationException;

namespace HospitalSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            AppValidationException appValidation => (HttpStatusCode.BadRequest, appValidation.Message, appValidation.Errors.ToList()),
            FluentValidationException fluentValidation => (HttpStatusCode.BadRequest, "Validation failed.", fluentValidation.Errors.Select(e => e.ErrorMessage).ToList()),
            Application.Exceptions.NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message, new List<string>()),
            Application.Exceptions.UnauthorizedException unauthorized => (HttpStatusCode.Unauthorized, unauthorized.Message, new List<string>()),
            Application.Exceptions.ForbiddenException forbidden => (HttpStatusCode.Forbidden, forbidden.Message, new List<string>()),
            UnauthorizedAccessException unauthorizedAccess => (HttpStatusCode.Unauthorized, unauthorizedAccess.Message, new List<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", new List<string>())
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception occurred.");
        else
            _logger.LogWarning(exception, "Request failed with {StatusCode}", statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiErrorResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
