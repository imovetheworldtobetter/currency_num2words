/*
 * Purpose: Defines the client contract for currency conversion requests.
*/

using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.Services;

public interface ICurrencyConversionClient
{
    Task<ConvertCurrencyResponse> ConvertAsync(ConvertCurrencyRequest request, CancellationToken cancellationToken);
}
