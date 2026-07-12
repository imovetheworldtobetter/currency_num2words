namespace myCurrencyMagic.Client.Input;

public sealed record AmountInputFormatResult(
    string DisplayText,
    int CaretIndex,
    bool IsValid,
    string? ValidationMessage);
