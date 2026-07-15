/*
 * Purpose: Verifies server-side currency conversion rules and normalization behavior.
*/

using myCurrencyMagic.Server.Conversion;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.UnitTests;

public sealed class CurrencyConverterServiceTests
{
    private readonly CurrencyConverterService _service = new(new TestNumberConversionRulesProvider());

    [Theory]
    [InlineData("de", "USD", "0", "null Dollar", "0")]
    [InlineData("de", "USD", "1", "ein Dollar", "1")]
    [InlineData("de", "USD", "33", "dreiunddreißig Dollar", "33")]
    [InlineData("de", "USD", "700", "siebenhundert Dollar", "700")]
    [InlineData("de", "USD", "14,05", "vierzehn Dollar und fünf Cent", "14,05")]
    [InlineData("de", "USD", "0,50", "null Dollar und fünfzig Cent", "0,50")]
    [InlineData("de", "USD", "33,00", "dreiunddreißig Dollar und null Cent", "33,00")]
    [InlineData("de", "USD", "25,1", "fünfundzwanzig Dollar und zehn Cent", "25,10")]
    [InlineData("de", "USD", "7,", "sieben Dollar und null Cent", "7,00")]
    [InlineData("de", "USD", "0 000,09", "null Dollar und neun Cent", "0,09")]
    [InlineData("de", "USD", "000 567,80", "fünfhundertsiebenundsechzig Dollar und achtzig Cent", "567,80")]
    [InlineData("de", "USD", "33 700", "dreiunddreißigtausend siebenhundert Dollar", "33700")]
    [InlineData("de", "USD", "1 000", "eintausend Dollar", "1000")]
    [InlineData("de", "USD", "2 000", "zweitausend Dollar", "2000")]
    [InlineData("de", "USD", "12 345", "zwölftausend dreihundertfünfundvierzig Dollar", "12345")]
    [InlineData("de", "USD", "1 000 000", "eine Million Dollar", "1000000")]
    [InlineData("de", "USD", "2 000 000", "zwei Millionen Dollar", "2000000")]
    [InlineData("de", "USD", "1 234 567", "eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig Dollar", "1234567")]
    public void Convert_ReturnsGermanCurrencyAmountInWords(
        string language,
        string currency,
        string amount,
        string expectedWords,
        string expectedNormalizedAmount)
    {
        var response = _service.Convert(new ConvertCurrencyRequest(language, currency, amount));

        Assert.Equal(expectedWords, response.AmountInWords);
        Assert.Equal(expectedNormalizedAmount, response.NormalizedAmount);
        Assert.Equal(LanguageCodes.German, response.Language);
        Assert.Equal(CurrencyCodes.UsDollar, response.Currency);
    }

    [Theory]
    [InlineData("en", "USD", "0", "zero dollars", "0")]
    [InlineData("en", "USD", "1", "one dollar", "1")]
    [InlineData("en", "USD", "33", "thirty-three dollars", "33")]
    [InlineData("en", "USD", "57", "fifty-seven dollars", "57")]
    [InlineData("en", "USD", "700", "seven hundred dollars", "700")]
    [InlineData("en", "USD", "14,05", "fourteen dollars and five cents", "14,05")]
    [InlineData("en", "USD", "0,50", "zero dollars and fifty cents", "0,50")]
    [InlineData("en", "USD", "25,1", "twenty-five dollars and ten cents", "25,10")]
    [InlineData("en", "USD", "7,", "seven dollars and zero cents", "7,00")]
    [InlineData("en", "USD", "33 700", "thirty-three thousand seven hundred dollars", "33700")]
    [InlineData("en", "USD", "1 234 567", "one million two hundred thirty-four thousand five hundred sixty-seven dollars", "1234567")]
    public void Convert_ReturnsEnglishCurrencyAmountInWords(
        string language,
        string currency,
        string amount,
        string expectedWords,
        string expectedNormalizedAmount)
    {
        var response = _service.Convert(new ConvertCurrencyRequest(language, currency, amount));

        Assert.Equal(expectedWords, response.AmountInWords);
        Assert.Equal(expectedNormalizedAmount, response.NormalizedAmount);
        Assert.Equal(LanguageCodes.English, response.Language);
        Assert.Equal(CurrencyCodes.UsDollar, response.Currency);
    }

    [Theory]
    [InlineData("de", "USD", "1", "ein Dollar")]
    [InlineData("de", "USD", "2", "zwei Dollar")]
    [InlineData("de", "USD", "33", "dreiunddreißig Dollar")]
    public void Convert_ReturnsGermanDollarWithNullPlural(
        string language,
        string currency,
        string amount,
        string expectedWords)
    {
        var response = _service.Convert(new ConvertCurrencyRequest(language, currency, amount));

        Assert.Equal(expectedWords, response.AmountInWords);
        Assert.Equal(currency, response.Currency);
    }

    [Theory]
    [InlineData("en", "USD", "", "The amount field is required.")]
    [InlineData("en", "USD", " ", "The amount field is required.")]
    [InlineData("en", "USD", "12,,34", "The amount field must contain at most one comma.")]
    [InlineData("en", "USD", "abc", "The amount field must start with at least one digit.")]
    [InlineData("en", "USD", "7,3r", "The cent part must contain digits only.")]
    [InlineData("en", "USD", "88,345", "The cent part must contain at most two digits.")]
    [InlineData("en", "USD", "999 999 999 999 999 999 999", "The integer part is too large.")]
    [InlineData("en", "USD", "1 000 000 000", "The amount must not be greater than 999 999 999,99.")]
    [InlineData("en", "USD", "12 34", "The amount field has an invalid format.")]
    [InlineData("fr", "USD", "1", "The language field must be 'en' or 'de'.")]
    [InlineData("en", "GBP", "1", "The currency field must be 'USD'.")]
    public void Convert_ThrowsForInvalidRequests(string language, string currency, string amount, string expectedException)
    {
        var request = new ConvertCurrencyRequest(language, currency, amount);
        var exception = Assert.Throws<CurrencyConversionException>(() => _service.Convert(request));

        Assert.Equal(expectedException, exception.Message);
    }

    private sealed class TestNumberConversionRulesProvider : INumberConversionRulesProvider
    {
        public NumberConversionRules GetRules()
        {
            return new NumberConversionRules
            {
                Numbers = new Dictionary<string, NumberLanguageRules>(StringComparer.OrdinalIgnoreCase)
                {
                    [LanguageCodes.German] = new()
                    {
                        Small = new Dictionary<int, string>
                        {
                            [0] = "null",
                            [1] = "ein",
                            [2] = "zwei",
                            [3] = "drei",
                            [4] = "vier",
                            [5] = "fünf",
                            [6] = "sechs",
                            [7] = "sieben",
                            [8] = "acht",
                            [9] = "neun",
                            [10] = "zehn",
                            [11] = "elf",
                            [12] = "zwölf",
                            [13] = "dreizehn",
                            [14] = "vierzehn",
                            [15] = "fünfzehn",
                            [16] = "sechzehn",
                            [17] = "siebzehn",
                            [18] = "achtzehn",
                            [19] = "neunzehn"
                        },
                        Tens = new Dictionary<int, string>
                        {
                            [20] = "zwanzig",
                            [30] = "dreißig",
                            [40] = "vierzig",
                            [50] = "fünfzig",
                            [60] = "sechzig",
                            [70] = "siebzig",
                            [80] = "achtzig",
                            [90] = "neunzig"
                        },
                        Magnitudes = new Dictionary<int, string>
                        {
                            [100] = "hundert",
                            [1_000] = "tausend",
                            [1_000_000] = "Million",
                            [2_000_000] = "Millionen"
                        }
                    },
                    [LanguageCodes.English] = new()
                    {
                        Small = new Dictionary<int, string>
                        {
                            [0] = "zero",
                            [1] = "one",
                            [2] = "two",
                            [3] = "three",
                            [4] = "four",
                            [5] = "five",
                            [6] = "six",
                            [7] = "seven",
                            [8] = "eight",
                            [9] = "nine",
                            [10] = "ten",
                            [11] = "eleven",
                            [12] = "twelve",
                            [13] = "thirteen",
                            [14] = "fourteen",
                            [15] = "fifteen",
                            [16] = "sixteen",
                            [17] = "seventeen",
                            [18] = "eighteen",
                            [19] = "nineteen"
                        },
                        Tens = new Dictionary<int, string>
                        {
                            [20] = "twenty",
                            [30] = "thirty",
                            [40] = "forty",
                            [50] = "fifty",
                            [60] = "sixty",
                            [70] = "seventy",
                            [80] = "eighty",
                            [90] = "ninety"
                        },
                        Magnitudes = new Dictionary<int, string>
                        {
                            [100] = "hundred",
                            [1_000] = "thousand",
                            [1_000_000] = "million"
                        }
                    }
                }
            };
        }
    }
}
