namespace myCurrencyMagic.Shared.Contracts;

public sealed record ConvertCurrencyRequest(
    string Language,
    string Currency,
    string Amount);
