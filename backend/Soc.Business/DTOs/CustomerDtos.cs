namespace Soc.Business.DTOs;

/// <summary>
/// Request body for creating a customer. At least a name is required.
/// </summary>
public record CreateCustomerRequest
{
    public string Name { get; init; } = string.Empty;
}

/// <summary>
/// Response model for a customer.
/// </summary>
public record CustomerResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? DeactivatedAt { get; init; }
}
