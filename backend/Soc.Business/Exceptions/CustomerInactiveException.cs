namespace Soc.Business.Exceptions;

/// <summary>
/// Thrown when an action is attempted against a customer that is deactivated,
/// for example ingesting a new alert for an inactive customer.
/// Maps to HTTP 409 Conflict.
/// </summary>
public class CustomerInactiveException : Exception
{
    public CustomerInactiveException(Guid id)
        : base($"Klant met id '{id}' is gedeactiveerd en kan geen nieuwe alerts ontvangen.")
    {
        Id = id;
    }

    public Guid Id { get; }
}
