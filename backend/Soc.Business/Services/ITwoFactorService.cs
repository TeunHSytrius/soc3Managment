using Soc.Business.DTOs;

namespace Soc.Business.Services;

/// <summary>
/// Two-factor authentication with an authenticator app such as Microsoft Authenticator (US-25).
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Generates a new secret, stores it as pending and returns the data for the QR code.
    /// 2FA is not active until the user confirms it with a first valid code.
    /// </summary>
    Task<TwoFactorSetupResponse> StartSetupAsync(StartTwoFactorSetupRequest request, CancellationToken cancellationToken = default);
}
