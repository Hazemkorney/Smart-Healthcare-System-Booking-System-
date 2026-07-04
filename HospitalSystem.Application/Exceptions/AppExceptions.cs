namespace HospitalSystem.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = [message];
    }

    public ValidationException(IEnumerable<string> errors)
        : base(string.Join("; ", errors))
    {
        Errors = errors.ToList();
    }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized.") : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbidden.") : base(message) { }
}
