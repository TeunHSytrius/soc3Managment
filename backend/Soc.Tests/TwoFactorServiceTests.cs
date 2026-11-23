using OtpNet;
using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Models;
using Soc.Business.Security;
using Soc.Business.Services;
using Soc.Data.Repositories;

namespace Soc.Tests;

public sealed class TwoFactorServiceTests
{
    private const int UserId = 1;
    private const string Email = "analist@soc3.nl";

    // Start of a 30-second TOTP step, so adding 30 seconds moves exactly one step.
    private static readonly DateTimeOffset StartTime = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

    private readonly InMemoryTwoFactorRepository _repository = new();
    private readonly FakeSecretProtector _secretProtector = new();
    private readonly FakeTimeProvider _time = new(StartTime);
    private readonly TwoFactorService _service;

    public TwoFactorServiceTests()
    {
        _service = new TwoFactorService(_repository, _secretProtector, _time);
    }

    // ---------- #163: setup with QR code ----------

    [Fact]
    public async Task StartSetup_ReturnsOtpAuthUriForAuthenticatorApp()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));

        Assert.StartsWith("otpauth://totp/SOC3:analist%40soc3.nl?secret=", response.OtpAuthUri);
        Assert.Contains("&issuer=SOC3", response.OtpAuthUri);
        Assert.Contains("&digits=6&period=30", response.OtpAuthUri);
    }

    [Fact]
    public async Task StartSetup_StoresOnlyProtectedPendingSecret_AndDoesNotEnable2fa()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));

        var stored = await _repository.GetByUserIdAsync(UserId);
        Assert.NotNull(stored);
        Assert.False(stored.IsEnabled);
        Assert.Null(stored.ProtectedSecret);

        string plainSecret = response.ManualEntryKey.Replace(" ", string.Empty);
        Assert.NotEqual(plainSecret, stored.PendingProtectedSecret);
        Assert.Equal(plainSecret, _secretProtector.Unprotect(stored.PendingProtectedSecret!));
    }

    [Fact]
    public async Task StartSetup_SecretIs160BitsAndMatchesQrCode()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));

        string plainSecret = response.ManualEntryKey.Replace(" ", string.Empty);
        Assert.Equal(20, Base32Encoding.ToBytes(plainSecret).Length);
        Assert.Contains($"secret={plainSecret}&", response.OtpAuthUri);
    }

    [Fact]
    public async Task StartSetup_GeneratesNewSecretEachTime()
    {
        var first = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));
        var second = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));

        Assert.NotEqual(first.ManualEntryKey, second.ManualEntryKey);
    }

    [Fact]
    public async Task StartSetup_WhenAlreadyEnabled_Throws()
    {
        await _repository.SaveAsync(new UserTwoFactor { UserId = UserId, IsEnabled = true, ProtectedSecret = "existing" });

        await Assert.ThrowsAsync<TwoFactorAlreadyEnabledException>(
            () => _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task StartSetup_WithoutEmail_Throws(string email)
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, email)));
    }

    // ---------- #164: enable only after a correct first code ----------

    [Fact]
    public async Task Enable_WithValidCode_Enables2fa()
    {
        string secret = await StartSetupAsync();

        await _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime)));

        var stored = await _repository.GetByUserIdAsync(UserId);
        Assert.True(stored!.IsEnabled);
        Assert.Null(stored.PendingProtectedSecret);
        Assert.Equal(secret, _secretProtector.Unprotect(stored.ProtectedSecret!));
        Assert.True(await _service.IsEnabledAsync(UserId));
    }

    [Fact]
    public async Task Enable_WithWrongCode_ThrowsAndStaysDisabled()
    {
        string secret = await StartSetupAsync();
        string wrongCode = CodeAt(secret, StartTime) == "000000" ? "111111" : "000000";

        await Assert.ThrowsAsync<InvalidTwoFactorCodeException>(
            () => _service.EnableAsync(new EnableTwoFactorRequest(UserId, wrongCode)));

        Assert.False(await _service.IsEnabledAsync(UserId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("abcdef")]
    public async Task Enable_WithMalformedCode_Throws(string code)
    {
        await StartSetupAsync();

        await Assert.ThrowsAsync<InvalidTwoFactorCodeException>(
            () => _service.EnableAsync(new EnableTwoFactorRequest(UserId, code)));
    }

    [Fact]
    public async Task Enable_WithoutSetup_Throws()
    {
        await Assert.ThrowsAsync<TwoFactorSetupNotStartedException>(
            () => _service.EnableAsync(new EnableTwoFactorRequest(UserId, "123456")));
    }

    [Fact]
    public async Task Enable_WhenAlreadyEnabled_Throws()
    {
        string secret = await EnableTwoFactorAsync();

        await Assert.ThrowsAsync<TwoFactorAlreadyEnabledException>(
            () => _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime))));
    }

    [Fact]
    public async Task Enable_AcceptsCodeFromPreviousStep_ForClockDrift()
    {
        string secret = await StartSetupAsync();

        await _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime.AddSeconds(-30))));

        Assert.True(await _service.IsEnabledAsync(UserId));
    }

    [Fact]
    public async Task Enable_RejectsExpiredCode()
    {
        string secret = await StartSetupAsync();

        await Assert.ThrowsAsync<InvalidTwoFactorCodeException>(
            () => _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime.AddMinutes(-2)))));
    }

    // ---------- #166: recovery codes ----------

    [Fact]
    public async Task Enable_ReturnsTenUniqueRecoveryCodes_StoredOnlyAsHashes()
    {
        string secret = await StartSetupAsync();

        var response = await _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime)));

        Assert.Equal(RecoveryCodes.Count, response.RecoveryCodes.Count);
        Assert.Equal(RecoveryCodes.Count, response.RecoveryCodes.Distinct().Count());
        Assert.All(response.RecoveryCodes, code => Assert.Matches("^[A-Z2-9]{4}-[A-Z2-9]{4}$", code));

        var stored = await _repository.GetByUserIdAsync(UserId);
        Assert.Equal(RecoveryCodes.Count, stored!.RecoveryCodeHashes.Count);
        Assert.DoesNotContain(response.RecoveryCodes[0], stored.RecoveryCodeHashes);
    }

    [Fact]
    public async Task VerifyLogin_WithRecoveryCode_WorksOnlyOnce()
    {
        var recoveryCodes = await EnableAndGetRecoveryCodesAsync();

        Assert.Equal(TwoFactorVerificationResult.Success, await _service.VerifyLoginAsync(UserId, recoveryCodes[0]));
        Assert.Equal(TwoFactorVerificationResult.InvalidCode, await _service.VerifyLoginAsync(UserId, recoveryCodes[0]));

        var stored = await _repository.GetByUserIdAsync(UserId);
        Assert.Equal(RecoveryCodes.Count - 1, stored!.RecoveryCodeHashes.Count);
    }

    [Fact]
    public async Task VerifyLogin_RecoveryCodeIgnoresCaseAndDash()
    {
        var recoveryCodes = await EnableAndGetRecoveryCodesAsync();
        string typedByUser = " " + recoveryCodes[0].Replace("-", string.Empty).ToLowerInvariant() + " ";

        Assert.Equal(TwoFactorVerificationResult.Success, await _service.VerifyLoginAsync(UserId, typedByUser));
    }

    // ---------- #165: second factor during login ----------

    [Fact]
    public async Task VerifyLogin_WithValidCode_Succeeds()
    {
        string secret = await EnableTwoFactorAsync();
        _time.Advance(TimeSpan.FromSeconds(30));

        var result = await _service.VerifyLoginAsync(UserId, CodeAt(secret, _time.GetUtcNow()));

        Assert.Equal(TwoFactorVerificationResult.Success, result);
    }

    [Fact]
    public async Task VerifyLogin_SameCodeTwice_SecondIsRejected()
    {
        string secret = await EnableTwoFactorAsync();
        _time.Advance(TimeSpan.FromSeconds(30));
        string code = CodeAt(secret, _time.GetUtcNow());

        Assert.Equal(TwoFactorVerificationResult.Success, await _service.VerifyLoginAsync(UserId, code));
        Assert.Equal(TwoFactorVerificationResult.InvalidCode, await _service.VerifyLoginAsync(UserId, code));
    }

    [Fact]
    public async Task VerifyLogin_CodeUsedToEnable_CannotBeReused()
    {
        string secret = await EnableTwoFactorAsync();

        var result = await _service.VerifyLoginAsync(UserId, CodeAt(secret, StartTime));

        Assert.Equal(TwoFactorVerificationResult.InvalidCode, result);
    }

    [Fact]
    public async Task VerifyLogin_WhenNotEnabled_ReturnsNotEnabled()
    {
        await StartSetupAsync();

        Assert.Equal(TwoFactorVerificationResult.NotEnabled, await _service.VerifyLoginAsync(UserId, "123456"));
        Assert.Equal(TwoFactorVerificationResult.NotEnabled, await _service.VerifyLoginAsync(99, "123456"));
    }

    [Fact]
    public async Task VerifyLogin_AfterTooManyWrongCodes_LocksOutEvenForValidCode()
    {
        string secret = await EnableTwoFactorAsync();
        _time.Advance(TimeSpan.FromSeconds(30));
        string validCode = CodeAt(secret, _time.GetUtcNow());
        string wrongCode = validCode == "000000" ? "111111" : "000000";

        for (int i = 1; i < TwoFactorService.MaxFailedAttempts; i++)
        {
            Assert.Equal(TwoFactorVerificationResult.InvalidCode, await _service.VerifyLoginAsync(UserId, wrongCode));
        }

        Assert.Equal(TwoFactorVerificationResult.LockedOut, await _service.VerifyLoginAsync(UserId, wrongCode));
        Assert.Equal(TwoFactorVerificationResult.LockedOut, await _service.VerifyLoginAsync(UserId, validCode));
    }

    [Fact]
    public async Task VerifyLogin_AfterLockoutExpires_ValidCodeWorksAgain()
    {
        string secret = await EnableTwoFactorAsync();

        for (int i = 0; i < TwoFactorService.MaxFailedAttempts; i++)
        {
            await _service.VerifyLoginAsync(UserId, "abcdef");
        }

        _time.Advance(TwoFactorService.LockoutDuration + TimeSpan.FromSeconds(30));

        var result = await _service.VerifyLoginAsync(UserId, CodeAt(secret, _time.GetUtcNow()));
        Assert.Equal(TwoFactorVerificationResult.Success, result);
    }

    [Fact]
    public async Task VerifyLogin_SuccessResetsFailedAttempts()
    {
        string secret = await EnableTwoFactorAsync();
        _time.Advance(TimeSpan.FromSeconds(30));

        await _service.VerifyLoginAsync(UserId, "abcdef");
        await _service.VerifyLoginAsync(UserId, CodeAt(secret, _time.GetUtcNow()));

        var stored = await _repository.GetByUserIdAsync(UserId);
        Assert.Equal(0, stored!.FailedAttempts);
    }

    // ---------- helpers ----------

    private async Task<string> StartSetupAsync()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(UserId, Email));
        return response.ManualEntryKey.Replace(" ", string.Empty);
    }

    private async Task<string> EnableTwoFactorAsync()
    {
        string secret = await StartSetupAsync();
        await _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime)));
        return secret;
    }

    private async Task<IReadOnlyList<string>> EnableAndGetRecoveryCodesAsync()
    {
        string secret = await StartSetupAsync();
        var response = await _service.EnableAsync(new EnableTwoFactorRequest(UserId, CodeAt(secret, StartTime)));
        return response.RecoveryCodes;
    }

    /// <summary>
    /// Calculates the code the authenticator app would show at the given moment.
    /// </summary>
    private static string CodeAt(string base32Secret, DateTimeOffset moment)
    {
        return new Totp(Base32Encoding.ToBytes(base32Secret)).ComputeTotp(moment.UtcDateTime);
    }

    /// <summary>
    /// Simple reversible stand-in for Data Protection, so tests can check that
    /// the stored value differs from the plain secret.
    /// </summary>
    private sealed class FakeSecretProtector : ISecretProtector
    {
        private const string Prefix = "protected:";

        public string Protect(string plainText) => Prefix + plainText;

        public string Unprotect(string protectedText) => protectedText[Prefix.Length..];
    }

    /// <summary>
    /// Clock that only moves when the test says so.
    /// </summary>
    private sealed class FakeTimeProvider : TimeProvider
    {
        private DateTimeOffset _now;

        public FakeTimeProvider(DateTimeOffset now)
        {
            _now = now;
        }

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan duration) => _now += duration;
    }
}
