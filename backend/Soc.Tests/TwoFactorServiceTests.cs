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
    private readonly InMemoryTwoFactorRepository _repository = new();
    private readonly FakeSecretProtector _secretProtector = new();
    private readonly TwoFactorService _service;

    public TwoFactorServiceTests()
    {
        _service = new TwoFactorService(_repository, _secretProtector);
    }

    [Fact]
    public async Task StartSetup_ReturnsOtpAuthUriForAuthenticatorApp()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl"));

        Assert.StartsWith("otpauth://totp/SOC3:analist%40soc3.nl?secret=", response.OtpAuthUri);
        Assert.Contains("&issuer=SOC3", response.OtpAuthUri);
        Assert.Contains("&digits=6&period=30", response.OtpAuthUri);
    }

    [Fact]
    public async Task StartSetup_StoresOnlyProtectedPendingSecret_AndDoesNotEnable2fa()
    {
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl"));

        var stored = await _repository.GetByUserIdAsync(1);
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
        var response = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl"));

        string plainSecret = response.ManualEntryKey.Replace(" ", string.Empty);
        Assert.Equal(20, Base32Encoding.ToBytes(plainSecret).Length);
        Assert.Contains($"secret={plainSecret}&", response.OtpAuthUri);
    }

    [Fact]
    public async Task StartSetup_GeneratesNewSecretEachTime()
    {
        var first = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl"));
        var second = await _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl"));

        Assert.NotEqual(first.ManualEntryKey, second.ManualEntryKey);
    }

    [Fact]
    public async Task StartSetup_WhenAlreadyEnabled_Throws()
    {
        await _repository.SaveAsync(new UserTwoFactor { UserId = 1, IsEnabled = true, ProtectedSecret = "existing" });

        await Assert.ThrowsAsync<TwoFactorAlreadyEnabledException>(
            () => _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, "analist@soc3.nl")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task StartSetup_WithoutEmail_Throws(string email)
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.StartSetupAsync(new StartTwoFactorSetupRequest(1, email)));
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
}
