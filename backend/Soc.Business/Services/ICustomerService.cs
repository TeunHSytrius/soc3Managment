using Soc.Business.DTOs;

namespace Soc.Business.Services;

/// <summary>
/// Business operations for managing customers (tenants) of the SOC.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Creates a new active customer. At least a name is required.
    /// </summary>
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a customer. Existing data is preserved (soft state change).
    /// Calling this on an already inactive customer is idempotent.
    /// </summary>
    Task<CustomerResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Guard used by alert ingestion: throws when the customer does not exist or
    /// is deactivated, so inactive customers cannot receive new alerts.
    /// </summary>
    Task EnsureCanReceiveAlertsAsync(Guid customerId, CancellationToken cancellationToken = default);
}
