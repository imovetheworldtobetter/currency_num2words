/*
 * Purpose: Normalizes numeric amounts and converts them into language-specific currency words.
*/

using myCurrencyMagic.Shared.Contracts;
using System.Text.RegularExpressions;

namespace myCurrencyMagic.Server.Conversion;

public sealed class CurrencyConverterService : ICurrencyConverterService
{
    private const long MaxIntegerPart = 999_999_999;
                                                        // full valid number e.g.: 123456789,12
    private static readonly Regex ValidAmountPattern = new(@"^(?:\d{1,3}(?: \d{3}){0,2})(?:,\d{0,2})?$", RegexOptions.Compiled);

    private readonly NumberConversionRules _rules;

    public CurrencyConverterService(INumberConversionRulesProvider rulesProvider)
    {
        _rules = rulesProvider.GetRules();
    }

    /*
     *  Method: Convert
     *  Purpose: Converts a validated request into a normalized response with written currency text.
     *  Input: A currency conversion request.
     *  Output: A conversion response containing amount words, normalized amount, language, and currency.
    */
    public ConvertCurrencyResponse Convert(ConvertCurrencyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedAmount = NormalizeAmount(request.Amount);
        var language = NormalizeLanguage(request.Language);
        var currency = NormalizeCurrency(request.Currency);
        var amountInWords = ConvertAmountToWords(normalizedAmount, language, currency);

        return new ConvertCurrencyResponse(
            amountInWords,
            normalizedAmount.ToDisplayValue(),
            language,
            currency);
    }

    /*
     *  Method: ConvertAmountToWords
     *  Purpose: Builds the language-specific written form for the normalized amount.
     *  Input: Normalized amount, language code, and currency code.
     *  Output: Amount text in words.
    */
    private string ConvertAmountToWords(NormalizedAmount amount, string language, string currency)
    {
        var mainCurrency = GetMainCurrencyName(language, amount.IntegerPart);

        if (language == LanguageCodes.German)
        {
            var mainWords = ConvertGermanCurrencyNumber(amount.IntegerPart);
            if (!amount.HasCentPart)
            {
                return $"{mainWords} {mainCurrency}";
            }

            var centWords = ConvertGermanCurrencyNumber(amount.CentPart);
            var centCurrency = GetCentCurrencyName(language, amount.CentPart);
            return $"{mainWords} {mainCurrency} und {centWords} {centCurrency}";
        }

        var englishMainWords = ConvertEnglishNumber(amount.IntegerPart);
        if (!amount.HasCentPart)
        {
            return $"{englishMainWords} {mainCurrency}";
        }

        var englishCentWords = ConvertEnglishNumber(amount.CentPart);
        var englishCentCurrency = GetCentCurrencyName(language, amount.CentPart);
        return $"{englishMainWords} {mainCurrency} and {englishCentWords} {englishCentCurrency}";
    }

    /*
     *  Method: NormalizeAmount
     *  Purpose: Validates and normalizes the incoming amount string.
     *  Input: Raw amount text from the request.
     *  Output: A normalized amount structure or a conversion exception for invalid input.
    */
    private NormalizedAmount NormalizeAmount(string amount)
    {
        if (string.IsNullOrWhiteSpace(amount))
        {
            throw new CurrencyConversionException("The amount field is required.");
        }

        var cleaned = amount.Replace(" ", string.Empty, StringComparison.Ordinal);
        if (cleaned.Length == 0)
        {
            throw new CurrencyConversionException("The amount field is required.");
        }

        if (cleaned.Count(character => character == ',') > 1)
        {
            throw new CurrencyConversionException("The amount field must contain at most one comma.");
        }

        var parts = cleaned.Split(',');
        var integerText = parts[0];
        var hasCentPart = parts.Length == 2;
        var centText = hasCentPart ? parts[1] : string.Empty;

        if (integerText.Length == 0 || !integerText.All(char.IsDigit))
        {
            throw new CurrencyConversionException("The amount field must start with at least one digit.");
        }

        if (hasCentPart && !centText.All(char.IsDigit))
        {
            throw new CurrencyConversionException("The cent part must contain digits only.");
        }

        if (hasCentPart && centText.Length > 2)
        {
            throw new CurrencyConversionException("The cent part must contain at most two digits.");
        }

        var normalizedIntegerText = integerText.TrimStart('0');
        if (normalizedIntegerText.Length == 0)
        {
            normalizedIntegerText = "0";
        }

        if (!long.TryParse(normalizedIntegerText, out var integerPart))
        {
            throw new CurrencyConversionException("The integer part is too large.");
        }

        if (integerPart > MaxIntegerPart)
        {
            throw new CurrencyConversionException("The amount must not be greater than 999 999 999,99.");
        }

        if (!ValidAmountPattern.IsMatch(amount.Trim()))
        {
            throw new CurrencyConversionException("The amount field has an invalid format.");
        }

        var centPart = 0;
        if (hasCentPart)
        {
            var normalizedCentText = centText.PadRight(2, '0');
            centPart = int.Parse(normalizedCentText);
        }

        return new NormalizedAmount(integerPart, centPart, hasCentPart);
    }

    /*
     *  Method: NormalizeLanguage
     *  Purpose: Ensures the request language is supported.
     *  Input: Raw language code.
     *  Output: A normalized supported language code or a conversion exception.
    */
    private static string NormalizeLanguage(string language)
    {
        if (string.Equals(language, LanguageCodes.German, StringComparison.OrdinalIgnoreCase))
        {
            return LanguageCodes.German;
        }

        if (string.Equals(language, LanguageCodes.English, StringComparison.OrdinalIgnoreCase))
        {
            return LanguageCodes.English;
        }

        throw new CurrencyConversionException("The language field must be 'en' or 'de'.");
    }

    /*
     *  Method: NormalizeCurrency
     *  Purpose: Ensures the request currency is supported.
     *  Input: Raw currency code.
     *  Output: A normalized supported currency code or a conversion exception.
    */
    private static string NormalizeCurrency(string currency)
    {
        if (string.Equals(currency, CurrencyCodes.UsDollar, StringComparison.OrdinalIgnoreCase))
        {
            return CurrencyCodes.UsDollar;
        }

        throw new CurrencyConversionException("The currency field must be 'USD'.");
    }

    /*
     *  Method: ConvertEnglishNumber
     *  Purpose: Converts a non-negative number into English words.
     *  Input: A non-negative integer number.
     *  Output: The number written in English words.
    */
    private string ConvertEnglishNumber(long number)
    {
        var englishRules = _rules.GetLanguageRules(LanguageCodes.English);

        if (number < 20)
        {
            return englishRules.Small[(int)number];
        }

        if (number < 100)
        {
            var tens = (int)(number / 10 * 10);
            var ones = (int)(number % 10);
            return ones == 0
                ? englishRules.Tens[tens]
                : $"{englishRules.Tens[tens]}-{englishRules.Small[ones]}";
        }

        if (number < 1_000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;
            var prefix = $"{englishRules.Small[(int)hundreds]} {englishRules.Magnitudes[100]}";
            return remainder == 0 ? prefix : $"{prefix} {ConvertEnglishNumber(remainder)}";
        }

        if (number < 1_000_000)
        {
            return ConvertEnglishScale(number, 1_000, englishRules.Magnitudes[1_000]);
        }

        return ConvertEnglishScale(number, 1_000_000, englishRules.Magnitudes[1_000_000]);
    }

    /*
     *  Method: ConvertEnglishScale
     *  Purpose: Composes an English scale word such as thousand or million.
     *  Input: Number, scale factor, and scale label.
     *  Output: A scaled English representation for thousands or millions.
    */
    private string ConvertEnglishScale(long number, int scale, string scaleName)
    {
        var scalePart = number / scale;
        var remainder = number % scale;
        var prefix = $"{ConvertEnglishNumber(scalePart)} {scaleName}";
        return remainder == 0 ? prefix : $"{prefix} {ConvertEnglishNumber(remainder)}";
    }

    /*
     *  Method: ConvertGermanNumber
     *  Purpose: Converts a non-negative number into German words.
     *  Input: A non-negative integer number.
     *  Output: The number written in German words.
    */
    private string ConvertGermanNumber(long number)
    {
        if (number < 1_000_000)
        {
            return ConvertGermanUnderMillion(number);
        }

        var germanRules = _rules.GetLanguageRules(LanguageCodes.German);
        var millions = number / 1_000_000;
        var remainder = number % 1_000_000;
        var millionWord = millions == 1
            ? $"eine {germanRules.Magnitudes[1_000_000]}"
            : $"{ConvertGermanUnderMillion(millions)} {germanRules.Magnitudes[2_000_000]}";

        return remainder == 0 ? millionWord : $"{millionWord} {ConvertGermanUnderMillion(remainder)}";
    }

    /*
     *  Method: ConvertGermanCurrencyNumber
     *  Purpose: Applies the German currency-specific singular form for one unit.
     *  Input: A German integer amount.
     *  Output: A German currency-friendly number representation.
    */
    private string ConvertGermanCurrencyNumber(long number)
    {
        return number == 1 ? "ein" : ConvertGermanNumber(number);
    }

    /*
     *  Method: ConvertGermanUnderMillion
     *  Purpose: Converts German numbers below one million.
     *  Input: A number below one million.
     *  Output: The number written in German words for the sub-million range.
    */
    private string ConvertGermanUnderMillion(long number)
    {
        var germanRules = _rules.GetLanguageRules(LanguageCodes.German);

        if (number < 20)
        {
            return ConvertGermanSmall((int)number, finalPosition: true);
        }

        if (number < 100)
        {
            return ConvertGermanUnderHundred((int)number);
        }

        if (number < 1_000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;
            var prefix = $"{ConvertGermanSmall((int)hundreds, finalPosition: false)}{germanRules.Magnitudes[100]}";
            return remainder == 0 ? prefix : $"{prefix}{ConvertGermanUnderMillion(remainder)}";
        }

        var thousands = number / 1_000;
        var thousandRemainder = number % 1_000;
        var thousandPrefix = $"{ConvertGermanSmallScale(thousands)}{germanRules.Magnitudes[1_000]}";
        if (thousandRemainder == 0)
        {
            return thousandPrefix;
        }

        var separator = thousands >= 10 ? " " : string.Empty;
        return $"{thousandPrefix}{separator}{ConvertGermanUnderMillion(thousandRemainder)}";
    }

    /*
     *  Method: ConvertGermanSmallScale
     *  Purpose: Normalizes the small-scale German prefix for thousands and similar groups.
     *  Input: A small German scale value.
     *  Output: The scale value normalized for German number composition.
    */
    private string ConvertGermanSmallScale(long number)
    {
        return number == 1 ? "ein" : ConvertGermanUnderMillion(number);
    }

    /*
     *  Method: ConvertGermanUnderHundred
     *  Purpose: Converts German numbers below one hundred.
     *  Input: A number below one hundred.
     *  Output: The number written in German under-hundred composition.
    */
    private string ConvertGermanUnderHundred(int number)
    {
        var germanRules = _rules.GetLanguageRules(LanguageCodes.German);

        if (number < 20)
        {
            return ConvertGermanSmall(number, finalPosition: true);
        }

        var tens = number / 10 * 10;
        var ones = number % 10;
        return ones == 0
            ? germanRules.Tens[tens]
            : $"{ConvertGermanSmall(ones, finalPosition: false)}und{germanRules.Tens[tens]}";
    }

    /*
     *  Method: ConvertGermanSmall
     *  Purpose: Returns the correct German word for small numbers, including final-position rules.
     *  Input: A small German number and whether it appears in final position.
     *  Output: The correct German word form for the small number.
    */
    private string ConvertGermanSmall(int number, bool finalPosition)
    {
        var germanRules = _rules.GetLanguageRules(LanguageCodes.German);
        if (number == 1 && finalPosition)
        {
            return "eins";
        }

        return germanRules.Small[number];
    }

    /*
     *  Method: GetMainCurrencyName
     *  Purpose: Selects the main currency word for the configured language.
     *  Input: Language code and integer value.
     *  Output: The currency name to use for the main amount.
    */
    private static string GetMainCurrencyName(string language, long value)
    {
        if (language == LanguageCodes.German)
        {
            return "Dollar";
        }

        return GetEnglishPlural("dollar", value);
    }

    /*
     *  Method: GetCentCurrencyName
     *  Purpose: Selects the cent currency word for the configured language.
     *  Input: Language code and cent value.
     *  Output: The currency name to use for the cent part.
    */
    private static string GetCentCurrencyName(string language, int value)
    {
        return language == LanguageCodes.German ? "Cent" : GetEnglishPlural("cent", value);
    }

    /*
     *  Method: GetEnglishPlural
     *  Purpose: Applies a simple singular or plural English noun form.
     *  Input: Singular noun and numeric value.
     *  Output: Singular or plural English noun form.
    */
    private static string GetEnglishPlural(string singular, long value)
    {
        return value == 1 ? singular : $"{singular}s";
    }

    private readonly record struct NormalizedAmount(long IntegerPart, int CentPart, bool HasCentPart)
    {
        /*
         *  Method: ToDisplayValue
         *  Purpose: Formats the normalized amount back into display form.
         *  Input: Normalized integer and cent values.
         *  Output: A display-friendly amount string matching the normalized value.
        */
        public string ToDisplayValue()
        {
            return HasCentPart
                ? $"{IntegerPart},{CentPart:00}"
                : IntegerPart.ToString();
        }
    }
}
