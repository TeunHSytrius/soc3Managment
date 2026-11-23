namespace Soc.Business.Models;

/// <summary>
/// Two-factor (TOTP) state of a single user. Secrets are only ever stored in
/// protected (encrypted) form and recovery codes only as hashes.
/// </summary>
public sealed class UserTwoFactor
{
    public int UserId { get; init; }

    /// <summary>
    /// Secret that is being set up, but not yet confirmed with a first valid code (US-25).
    /// </summary>
    public string? PendingProtectedSecret { get; set; }

    /// <summary>
    /// Confirmed secret that is used during login once 2FA is enabled.
    /// </summary>
    public string? ProtectedSecret { get; set; }

    public bool IsEnabled { get; set; }

    /// <summary>
    /// TOTP time step of the last accepted code. A code is only accepted once,
    /// so an intercepted code cannot be replayed.
    /// </summary>
    public long LastUsedTimeStep { get; set; }

    /// <summary>
    /// SHA-256 hashes of the recovery codes that have not been used yet.
    /// </summary>
    public List<string> RecoveryCodeHashes { get; set; } = [];

    public int FailedAttempts { get; set; }

    public DateTimeOffset? LockedUntil { get; set; }
}
