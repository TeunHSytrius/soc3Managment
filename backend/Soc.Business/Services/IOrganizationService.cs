using Soc.Business.DTOs;

namespace Soc.Business.Services;

/// <summary>
/// Business operations for managing tenant organisations of the SOC.
/// </summary>
public interface IOrganizationService
{
    /// <summary>
    /// Creates a new active tenant organisation. At least a name is required.
    /// </summary>
    Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a tenant organisation. Existing data is preserved (soft state change).
    /// Calling this on an already inactive tenant is idempotent.
    /// </summary>
    Task<OrganizationResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrganizationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Guard used by alert ingestion: throws when the tenant does not exist or
    /// is deactivated, so inactive tenants cannot receive new alerts.
    /// </summary>
    Task EnsureCanReceiveAlertsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
