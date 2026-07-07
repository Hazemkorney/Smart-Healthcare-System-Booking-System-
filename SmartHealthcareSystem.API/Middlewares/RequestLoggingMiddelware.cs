namespace SmartHealthcareSystem.API.Middlewares
{
    public class RequestLoggingMiddelware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddelware> _Logger;
        public RequestLoggingMiddelware(RequestDelegate next, ILogger<RequestLoggingMiddelware> logger)
        {
            _Logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _Logger.LogInformation("Incoming request: {Method} {Path} {Query}", context.Request.Method, context.Request.Path,context.Request.Query);
            await _next(context);
            _Logger.LogInformation("Outgoing response: {Method} {StatusCode}",context.Request.Method,context.Response.StatusCode);
        }
}}
