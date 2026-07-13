using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.Configuration;

public sealed class ClientUiOptions
{
    public const string SectionName = "ClientUi";

    public string DefaultLanguage { get; init; } = LanguageCodes.English;

    public Dictionary<string, ClientLanguageUiOptions> Languages { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public ClientLanguageUiOptions GetLanguage(string language)
    {
        if (Languages.TryGetValue(language, out var languageOptions))
        {
            return languageOptions;
        }

        if (Languages.TryGetValue(DefaultLanguage, out var defaultLanguageOptions))
        {
            return defaultLanguageOptions;
        }

        return Languages.Values.FirstOrDefault() ?? new ClientLanguageUiOptions();
    }

    public static Dictionary<string, ClientLanguageUiOptions> CreateDefaultLanguages()
    {
        return new Dictionary<string, ClientLanguageUiOptions>(StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class ClientLanguageUiOptions
{
    public string DisplayName { get; init; } = string.Empty;

    public string Currency { get; init; } = CurrencyCodes.UsDollar;

    public string CurrencySymbol { get; init; } = "$";

    public bool IsCurrencyLeading { get; init; } = true;

    public ClientTextOptions Texts { get; init; } = new();
}

public sealed class ClientTextOptions
{
    public string IntroText { get; init; } = string.Empty;

    public string InputTitle { get; init; } = string.Empty;

    public string OutputTitle { get; init; } = string.Empty;

    public string ConvertButtonText { get; init; } = string.Empty;

    public string InvalidCharacterMessage { get; init; } = string.Empty;

    public string InvalidFormatMessage { get; init; } = string.Empty;

    public string DecimalDigitsMessage { get; init; } = string.Empty;

    public string MaximumValueMessage { get; init; } = string.Empty;

    public string ServerErrorMessage { get; init; } = string.Empty;
}
