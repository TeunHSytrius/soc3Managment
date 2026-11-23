namespace Soc.Business.Exceptions;

public sealed class InvalidTwoFactorCodeException : Exception
{
    public InvalidTwoFactorCodeException()
        : base("The verification code is invalid or has expired.")
    {
    }
}
