namespace Soc.Business.Models;

/// <summary>
/// Two-factor (TOTP) state of a single user. Secrets are only ever stored in
/// protected (encrypted) form; the plain secret never leaves the service.
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
}
