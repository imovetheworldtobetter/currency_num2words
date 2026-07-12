using System.Text.Json.Serialization;

namespace myCurrencyMagic.Server.Conversion;

public sealed class NumberConversionRules
{
    [JsonPropertyName("numbers")]
    public Dictionary<string, NumberLanguageRules> Numbers { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public NumberLanguageRules GetLanguageRules(string language)
    {
        if (!Numbers.TryGetValue(language, out var rules))
        {
            throw new CurrencyConversionException($"No numeric conversion rules are configured for language '{language}'.");
        }

        return rules;
    }
}
