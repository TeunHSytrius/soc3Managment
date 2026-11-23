using OtpNet;
using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Models;
using Soc.Business.Repositories;
using Soc.Business.Security;

namespace Soc.Business.Services;

public sealed class TwoFactorService : ITwoFactorService
{
    // Name shown above the code in the authenticator app.
    private const string Issuer = "SOC3";

    // 20 bytes = 160 bits, the length recommended for TOTP (RFC 4226).
    private const int SecretLengthInBytes = 20;

    private readonly ITwoFactorRepository _repository;
    private readonly ISecretProtector _secretProtector;

    public TwoFactorService(ITwoFactorRepository repository, ISecretProtector secretProtector)
    {
        _repository = repository;
        _secretProtector = secretProtector;
    }

    public async Task<TwoFactorSetupResponse> StartSetupAsync(StartTwoFactorSetupRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.", nameof(request));
        }

        var userTwoFactor = await _repository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? new UserTwoFactor { UserId = request.UserId };

        if (userTwoFactor.IsEnabled)
        {
            throw new TwoFactorAlreadyEnabledException(request.UserId);
        }

        // Cryptographically secure random secret. Authenticator apps expect it in Base32.
        byte[] secret = KeyGeneration.GenerateRandomKey(SecretLengthInBytes);
        string base32Secret = Base32Encoding.ToString(secret);

        // Only store the encrypted secret. A new setup replaces an older, unconfirmed one.
        userTwoFactor.PendingProtectedSecret = _secretProtector.Protect(base32Secret);
        await _repository.SaveAsync(userTwoFactor, cancellationToken);

        return new TwoFactorSetupResponse(
            BuildOtpAuthUri(request.Email.Trim(), base32Secret),
            FormatManualEntryKey(base32Secret));
    }

    /// <summary>
    /// Builds the standard otpauth:// link (Key Uri Format) that Microsoft Authenticator,
    /// Google Authenticator and similar apps understand.
    /// </summary>
    private static string BuildOtpAuthUri(string accountName, string base32Secret)
    {
        string label = $"{Uri.EscapeDataString(Issuer)}:{Uri.EscapeDataString(accountName)}";

        return $"otpauth://totp/{label}" +
            $"?secret={base32Secret}" +
            $"&issuer={Uri.EscapeDataString(Issuer)}" +
            "&algorithm=SHA1&digits=6&period=30";
    }

    /// <summary>
    /// Splits the secret into groups of four characters so it is easier to type over.
    /// </summary>
    private static string FormatManualEntryKey(string base32Secret)
    {
        return string.Join(' ', base32Secret.Chunk(4).Select(chunk => new string(chunk)));
    }
}
