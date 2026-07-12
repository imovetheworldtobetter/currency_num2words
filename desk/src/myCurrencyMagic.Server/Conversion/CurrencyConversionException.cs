namespace myCurrencyMagic.Server.Conversion;

public sealed class CurrencyConversionException : Exception
{
    public CurrencyConversionException(string message)
        : base(message)
    {
    }
}
