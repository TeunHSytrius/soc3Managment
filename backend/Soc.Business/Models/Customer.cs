namespace Soc.Business.Models;

/// <summary>
/// Represents a customer organisation (tenant) served by the SOC.
/// Customer data lives in the platform schema. Deactivating a customer is a
/// soft operation: the record is preserved so existing data stays available.
/// </summary>
public class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this customer can still receive new alerts. Inactive customers
    /// keep their existing data but no longer accept ingestion.
    /// </summary>
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeactivatedAt { get; set; }

    /// <summary>
    /// Creates a new active customer with a generated identifier.
    /// </summary>
    public static Customer Create(string name)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            DeactivatedAt = null,
        };
    }

    /// <summary>
    /// Marks the customer as inactive. Existing data is preserved.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedAt = DateTimeOffset.UtcNow;
    }
}
