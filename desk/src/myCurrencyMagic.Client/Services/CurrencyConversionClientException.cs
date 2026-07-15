/*
 * Purpose: Represents client-side failures when calling the conversion server.
*/

namespace myCurrencyMagic.Client.Services;

public sealed class CurrencyConversionClientException : Exception
{
    public CurrencyConversionClientException(string message)
        : base(message)
    {
    }

    public CurrencyConversionClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
