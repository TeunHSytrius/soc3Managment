namespace Soc.Business.Exceptions;

/// <summary>
/// Thrown when a request fails business validation, e.g. a missing customer name.
/// Maps to HTTP 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
