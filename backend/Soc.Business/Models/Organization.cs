namespace Soc.Business.Models;

/// <summary>
/// Represents a customer organisation (tenant) served by the SOC.
/// Tenant data lives in the platform schema. Deactivating a tenant is a
/// soft operation: the record is preserved so existing data stays available.
/// </summary>
public class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this tenant can still receive new alerts. Inactive tenants
    /// keep their existing data but no longer accept ingestion.
    /// </summary>
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeactivatedAt { get; set; }

    /// <summary>
    /// Creates a new active tenant with a generated identifier.
    /// </summary>
    public static Organization Create(string name)
    {
        return new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            DeactivatedAt = null,
        };
    }

    /// <summary>
    /// Marks the tenant as inactive. Existing data is preserved.
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
