namespace Soc.Business.Exceptions;

/// <summary>
/// Thrown when a tenant organisation cannot be found by its identifier.
/// Maps to HTTP 404 Not Found.
/// </summary>
public class OrganizationNotFoundException : Exception
{
    public OrganizationNotFoundException(Guid id)
        : base($"Organisatie met id '{id}' is niet gevonden.")
    {
        Id = id;
    }

    public Guid Id { get; }
}
