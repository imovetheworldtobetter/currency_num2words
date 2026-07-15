/*
 * Purpose: Stores language-specific number conversion data for the server.
*/

using System.Text.Json.Serialization;

namespace myCurrencyMagic.Server.Conversion;

public sealed class NumberConversionRules
{
    [JsonPropertyName("numbers")]
    public Dictionary<string, NumberLanguageRules> Numbers { get; init; }

    public NumberConversionRules()
    {
        Numbers = new Dictionary<string, NumberLanguageRules>(StringComparer.OrdinalIgnoreCase);
    }

    public NumberLanguageRules GetLanguageRules(string language)
    {
        NumberLanguageRules? rules;
        if (!Numbers.TryGetValue(language, out rules))
        {
            throw new CurrencyConversionException($"No numeric conversion rules are configured for language '{language}'.");
        }

        return rules;
    }
}
