/*
 * Purpose: Loads number conversion rules from JSON files at server startup.
*/

using System.Text.Json;

namespace myCurrencyMagic.Server.Conversion;

public sealed class JsonNumberConversionRulesProvider : INumberConversionRulesProvider
{
    private const string RulesRelativePath = "dev_assets/rules_numeric_conversion.json";

    private readonly Lazy<NumberConversionRules> _rules = new(LoadRules);

    public NumberConversionRules GetRules()
    {
        return _rules.Value;
    }

    private static NumberConversionRules LoadRules()
    {
        var rulesPath = Path.Combine(AppContext.BaseDirectory, RulesRelativePath);
        if (!File.Exists(rulesPath))
        {
            throw new FileNotFoundException("Numeric conversion rules file was not found.", rulesPath);
        }

        using var stream = File.OpenRead(rulesPath);
        var rules = JsonSerializer.Deserialize<NumberConversionRules>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return rules ?? throw new InvalidOperationException("Numeric conversion rules file is empty.");
    }
}
