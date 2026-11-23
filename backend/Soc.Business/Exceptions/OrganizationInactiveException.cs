namespace Soc.Business.Exceptions;

/// <summary>
/// Thrown when an action is attempted against a tenant organisation that is
/// deactivated, for example ingesting a new alert for an inactive tenant.
/// Maps to HTTP 409 Conflict.
/// </summary>
public class OrganizationInactiveException : Exception
{
    public OrganizationInactiveException(Guid id)
        : base($"Organisatie met id '{id}' is gedeactiveerd en kan geen nieuwe alerts ontvangen.")
    {
        Id = id;
    }

    public Guid Id { get; }
}
