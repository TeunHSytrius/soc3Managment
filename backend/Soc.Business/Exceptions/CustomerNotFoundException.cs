namespace Soc.Business.Exceptions;

/// <summary>
/// Thrown when a customer cannot be found by its identifier.
/// Maps to HTTP 404 Not Found.
/// </summary>
public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(Guid id)
        : base($"Klant met id '{id}' is niet gevonden.")
    {
        Id = id;
    }

    public Guid Id { get; }
}
