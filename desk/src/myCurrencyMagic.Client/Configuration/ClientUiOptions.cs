using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.Configuration;

public sealed class ClientUiOptions
{
    public const string SectionName = "ClientUi";

    public string DefaultLanguage { get; init; } = LanguageCodes.English;

    public Dictionary<string, ClientLanguageUiOptions> Languages { get; init; } = CreateDefaultLanguages();

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
        return new Dictionary<string, ClientLanguageUiOptions>(StringComparer.OrdinalIgnoreCase)
        {
            [LanguageCodes.English] = new()
            {
                DisplayName = "US-EN",
                Currency = CurrencyCodes.UsDollar,
                CurrencySymbol = "$",
                IsCurrencyLeading = true,
                Texts = new ClientTextOptions
                {
                    IntroText = "Use this application to convert a typed US dollar amount into words. Type a number into the input field. When you are done, press Convert. The amount in words appears in the output field.",
                    InputTitle = "Input currency amount as number",
                    OutputTitle = "Output currency amount in words",
                    ConvertButtonText = "Convert",
                    InvalidCharacterMessage = "Allowed characters: digits 0-9 and comma.",
                    InvalidFormatMessage = "Use the format 123 456 789,12.",
                    DecimalDigitsMessage = "A comma must be followed by one or two digits.",
                    MaximumValueMessage = "The maximum value is 999 999 999,99.",
                    ServerErrorMessage = "The server is not reachable or could not process the request."
                }
            },
            [LanguageCodes.German] = new()
            {
                DisplayName = "DE",
                Currency = CurrencyCodes.UsDollar,
                CurrencySymbol = "$",
                IsCurrencyLeading = true,
                Texts = new ClientTextOptions
                {
                    IntroText = "Mit dieser Anwendung können Sie einen US-Dollar-Betrag als Zahl eingeben und in Worte umwandeln. Tippen Sie eine Zahl in das Eingabefeld. Wenn Sie fertig sind, drücken Sie Umwandeln. Ihren Geldbetrag in Worten finden Sie anschließend im Ausgabefeld.",
                    InputTitle = "Eingabe Geldbetrag als Zahl",
                    OutputTitle = "Ausgabe Geldbetrag in Worten",
                    ConvertButtonText = "Umwandeln",
                    InvalidCharacterMessage = "Erlaubte Zeichen: Ziffern 0-9 und Komma.",
                    InvalidFormatMessage = "Verwenden Sie das Format 123 456 789,12.",
                    DecimalDigitsMessage = "Auf ein Komma müssen ein oder zwei Ziffern folgen.",
                    MaximumValueMessage = "Der maximale Wert ist 999 999 999,99.",
                    ServerErrorMessage = "Der Server ist nicht erreichbar oder konnte die Anfrage nicht verarbeiten."
                }
            }
        };
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
