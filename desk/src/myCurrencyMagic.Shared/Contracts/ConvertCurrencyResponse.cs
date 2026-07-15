/*
 * Purpose: Represents a currency conversion response payload.
*/

namespace myCurrencyMagic.Shared.Contracts;

public sealed record ConvertCurrencyResponse(
    string AmountInWords,
    string NormalizedAmount,
    string Language,
    string Currency);
