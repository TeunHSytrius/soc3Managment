namespace Soc.Business.DTOs;

/// <summary>
/// Request to start setting up 2FA.
/// TODO: remove UserId and Email once login exists; take them from the authenticated user instead.
/// </summary>
public sealed record StartTwoFactorSetupRequest(int UserId, string Email);

/// <summary>
/// Data the frontend needs to show the QR code.
/// </summary>
/// <param name="OtpAuthUri">otpauth:// link that is shown as a QR code and scanned by the authenticator app.</param>
/// <param name="ManualEntryKey">Same secret in readable groups, for users who cannot scan the QR code.</param>
public sealed record TwoFactorSetupResponse(string OtpAuthUri, string ManualEntryKey);
