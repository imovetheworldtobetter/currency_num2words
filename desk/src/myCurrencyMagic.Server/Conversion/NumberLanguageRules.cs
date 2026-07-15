/*
 * Purpose: Holds language-specific small numbers, tens, and magnitude words.
*/

using System.Text.Json.Serialization;

namespace myCurrencyMagic.Server.Conversion;

public sealed class NumberLanguageRules
{
    [JsonPropertyName("small")]
    public Dictionary<int, string> Small { get; init; } = [];

    [JsonPropertyName("tens")]
    public Dictionary<int, string> Tens { get; init; } = [];

    [JsonPropertyName("magnitudes")]
    public Dictionary<int, string> Magnitudes { get; init; } = [];
}
