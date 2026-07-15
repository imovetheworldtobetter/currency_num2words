/*
 * Purpose: Represents validation and normalization failures in currency conversion.
*/

namespace myCurrencyMagic.Server.Conversion;

public sealed class CurrencyConversionException : Exception
{
    public CurrencyConversionException(string message)
        : base(message)
    {
    }
}
