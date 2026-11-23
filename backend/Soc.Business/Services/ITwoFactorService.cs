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

    /// <summary>
    /// Enables 2FA only when the code matches the pending secret (#164) and
    /// returns new one-time recovery codes (#166).
    /// </summary>
    Task<EnableTwoFactorResponse> EnableAsync(EnableTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the second factor during login (#165): a code from the authenticator app
    /// or an unused recovery code. To be called by the login flow after the password check.
    /// </summary>
    Task<TwoFactorVerificationResult> VerifyLoginAsync(int userId, string code, CancellationToken cancellationToken = default);

    Task<bool> IsEnabledAsync(int userId, CancellationToken cancellationToken = default);
}
