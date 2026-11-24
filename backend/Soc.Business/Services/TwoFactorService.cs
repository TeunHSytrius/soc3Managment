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

    private const int CodeLength = 6;

    // Brute-force protection: after this many wrong codes the login is blocked for a while,
    // and an unconfirmed setup is discarded.
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    // An unconfirmed setup (QR code scanned, no code entered yet) is only valid this long.
    public static readonly TimeSpan PendingSetupLifetime = TimeSpan.FromMinutes(10);

    // Also accept the code of 30 seconds before and after, in case the phone clock is slightly off.
    private static readonly VerificationWindow ClockDriftWindow = new(previous: 1, future: 1);

    private readonly ITwoFactorRepository _repository;
    private readonly ISecretProtector _secretProtector;
    private readonly TimeProvider _timeProvider;

    public TwoFactorService(ITwoFactorRepository repository, ISecretProtector secretProtector, TimeProvider timeProvider)
    {
        _repository = repository;
        _secretProtector = secretProtector;
        _timeProvider = timeProvider;
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
        userTwoFactor.PendingSetupExpiresAt = _timeProvider.GetUtcNow() + PendingSetupLifetime;
        userTwoFactor.FailedAttempts = 0;
        await _repository.SaveAsync(userTwoFactor, cancellationToken);

        return new TwoFactorSetupResponse(
            BuildOtpAuthUri(request.Email.Trim(), base32Secret),
            FormatManualEntryKey(base32Secret));
    }

    public async Task<EnableTwoFactorResponse> EnableAsync(EnableTwoFactorRequest request, CancellationToken cancellationToken = default)
    {
        var userTwoFactor = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (userTwoFactor is { IsEnabled: true })
        {
            throw new TwoFactorAlreadyEnabledException(request.UserId);
        }

        if (userTwoFactor?.PendingProtectedSecret is null)
        {
            throw new TwoFactorSetupNotStartedException(request.UserId);
        }

        // An expired setup can no longer be confirmed; the user has to scan a new QR code.
        if (userTwoFactor.PendingSetupExpiresAt <= _timeProvider.GetUtcNow())
        {
            ClearPendingSetup(userTwoFactor);
            await _repository.SaveAsync(userTwoFactor, cancellationToken);
            throw new TwoFactorSetupNotStartedException(request.UserId);
        }

        // #164: only enable when the user proves the authenticator app produces correct codes.
        if (!TryVerifyTotp(userTwoFactor.PendingProtectedSecret, request.Code, lastUsedTimeStep: 0, out long timeStep))
        {
            userTwoFactor.FailedAttempts++;

            // Too many wrong codes: discard the setup so the code cannot be guessed.
            if (userTwoFactor.FailedAttempts >= MaxFailedAttempts)
            {
                ClearPendingSetup(userTwoFactor);
            }

            await _repository.SaveAsync(userTwoFactor, cancellationToken);
            throw new InvalidTwoFactorCodeException();
        }

        // #166: generate recovery codes; store only hashes, return the plain codes once.
        IReadOnlyList<string> recoveryCodes = RecoveryCodes.Generate();

        userTwoFactor.ProtectedSecret = userTwoFactor.PendingProtectedSecret;
        ClearPendingSetup(userTwoFactor);
        userTwoFactor.IsEnabled = true;
        userTwoFactor.LastUsedTimeStep = timeStep;
        userTwoFactor.RecoveryCodeHashes = recoveryCodes.Select(RecoveryCodes.Hash).ToList();
        userTwoFactor.LockedUntil = null;

        await _repository.SaveAsync(userTwoFactor, cancellationToken);

        return new EnableTwoFactorResponse(recoveryCodes);
    }

    public async Task<TwoFactorVerificationResult> VerifyLoginAsync(int userId, string code, CancellationToken cancellationToken = default)
    {
        var userTwoFactor = await _repository.GetByUserIdAsync(userId, cancellationToken);

        if (userTwoFactor is not { IsEnabled: true, ProtectedSecret: not null })
        {
            return TwoFactorVerificationResult.NotEnabled;
        }

        DateTimeOffset now = _timeProvider.GetUtcNow();

        if (userTwoFactor.LockedUntil > now)
        {
            return TwoFactorVerificationResult.LockedOut;
        }

        bool isValid = false;

        if (TryVerifyTotp(userTwoFactor.ProtectedSecret, code, userTwoFactor.LastUsedTimeStep, out long timeStep))
        {
            userTwoFactor.LastUsedTimeStep = timeStep;
            isValid = true;
        }
        else if (RecoveryCodes.LooksLikeRecoveryCode(code)
            && await _repository.TryConsumeRecoveryCodeAsync(userId, RecoveryCodes.Hash(code), cancellationToken))
        {
            isValid = true;
        }

        if (isValid)
        {
            userTwoFactor.FailedAttempts = 0;
            userTwoFactor.LockedUntil = null;
            await _repository.SaveAsync(userTwoFactor, cancellationToken);
            return TwoFactorVerificationResult.Success;
        }

        userTwoFactor.FailedAttempts++;

        if (userTwoFactor.FailedAttempts >= MaxFailedAttempts)
        {
            userTwoFactor.FailedAttempts = 0;
            userTwoFactor.LockedUntil = now + LockoutDuration;
            await _repository.SaveAsync(userTwoFactor, cancellationToken);
            return TwoFactorVerificationResult.LockedOut;
        }

        await _repository.SaveAsync(userTwoFactor, cancellationToken);
        return TwoFactorVerificationResult.InvalidCode;
    }

    public async Task<bool> IsEnabledAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userTwoFactor = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return userTwoFactor?.IsEnabled == true;
    }

    /// <summary>
    /// Checks a 6-digit code against the secret. A code whose time step was already used
    /// (or is older) is rejected, so every code works only once.
    /// </summary>
    private bool TryVerifyTotp(string protectedSecret, string code, long lastUsedTimeStep, out long timeStep)
    {
        timeStep = 0;
        string normalizedCode = code.Replace(" ", string.Empty).Trim();

        if (normalizedCode.Length != CodeLength || !normalizedCode.All(char.IsAsciiDigit))
        {
            return false;
        }

        byte[] secret = Base32Encoding.ToBytes(_secretProtector.Unprotect(protectedSecret));
        var totp = new Totp(secret);
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        return totp.VerifyTotp(now, normalizedCode, out timeStep, ClockDriftWindow)
            && timeStep > lastUsedTimeStep;
    }

    private static void ClearPendingSetup(UserTwoFactor userTwoFactor)
    {
        userTwoFactor.PendingProtectedSecret = null;
        userTwoFactor.PendingSetupExpiresAt = null;
        userTwoFactor.FailedAttempts = 0;
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
