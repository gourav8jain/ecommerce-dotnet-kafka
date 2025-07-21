namespace ECommerce.Common.Exceptions;

public class BusinessException : Exception
{
    public int StatusCode { get; }

    public BusinessException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}

public class ValidationException : BusinessException
{
    public ValidationException(string message) : base(message, 400)
    {
    }
}

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message) : base(message, 401)
    {
    }
}

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message) : base(message, 403)
    {
    }
} 