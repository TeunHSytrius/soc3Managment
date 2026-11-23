namespace Soc.Business.Exceptions;

public sealed class TwoFactorSetupNotStartedException : Exception
{
    public TwoFactorSetupNotStartedException(int userId)
        : base($"Two-factor setup has not been started for user {userId}.")
    {
    }
}
