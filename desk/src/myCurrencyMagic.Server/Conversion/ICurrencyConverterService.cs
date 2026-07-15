/*
 * Purpose: Defines the server-side currency conversion contract.
*/

using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Server.Conversion;

public interface ICurrencyConverterService
{
    ConvertCurrencyResponse Convert(ConvertCurrencyRequest request);
}
