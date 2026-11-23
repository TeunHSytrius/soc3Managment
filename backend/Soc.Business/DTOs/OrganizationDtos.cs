namespace Soc.Business.DTOs;

/// <summary>
/// Request body for creating a tenant organisation. At least a name is required.
/// </summary>
public record CreateOrganizationRequest
{
    public string Name { get; init; } = string.Empty;
}

/// <summary>
/// Response model for a tenant organisation.
/// </summary>
public record OrganizationResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? DeactivatedAt { get; init; }
}
