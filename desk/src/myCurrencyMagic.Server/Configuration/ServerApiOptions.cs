using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Server.Configuration;

public sealed class ServerApiOptions
{
    public const string SectionName = "Api";

    public ClientHeaderOptions ClientHeader { get; init; } = new();

    public string[] SupportedLanguages { get; init; } =
    [
        LanguageCodes.English,
        LanguageCodes.German
    ];

    public string[] SupportedCurrencies { get; init; } =
    [
        CurrencyCodes.UsDollar,
        CurrencyCodes.Euro
    ];

    public bool IsSupportedLanguage(string language)
    {
        return GetDistinctValues(SupportedLanguages).Contains(language, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsSupportedCurrency(string currency)
    {
        return GetDistinctValues(SupportedCurrencies).Contains(currency, StringComparer.OrdinalIgnoreCase);
    }

    public string SupportedLanguagesMessage => FormatAllowedValues(GetDistinctValues(SupportedLanguages));

    public string SupportedCurrenciesMessage => FormatAllowedValues(GetDistinctValues(SupportedCurrencies));

    private static string[] GetDistinctValues(IEnumerable<string> values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string FormatAllowedValues(IReadOnlyList<string> values)
    {
        if (values.Count == 0)
        {
            return "no values";
        }

        if (values.Count == 1)
        {
            return $"'{values[0]}'";
        }

        return string.Join(", ", values.Take(values.Count - 1).Select(value => $"'{value}'"))
            + $" or '{values[^1]}'";
    }
}

public sealed class ClientHeaderOptions
{
    public string Name { get; init; } = ApiHeaders.ClientHeaderName;

    public string ExpectedValue { get; init; } = ApiHeaders.DefaultClientHeaderValue;
}
