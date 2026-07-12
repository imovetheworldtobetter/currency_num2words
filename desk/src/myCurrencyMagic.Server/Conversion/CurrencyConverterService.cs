using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Server.Conversion;

public sealed class CurrencyConverterService : ICurrencyConverterService
{
    private const long MaxIntegerPart = 999_999_999;

    private readonly NumberConversionRules _rules;

    public CurrencyConverterService(INumberConversionRulesProvider rulesProvider)
    {
        _rules = rulesProvider.GetRules();
    }

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

        var centPart = 0;
        if (hasCentPart)
        {
            var normalizedCentText = centText.PadRight(2, '0');
            centPart = int.Parse(normalizedCentText);
        }

        return new NormalizedAmount(integerPart, centPart, hasCentPart);
    }

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

    private static string NormalizeCurrency(string currency)
    {
        if (string.Equals(currency, CurrencyCodes.UsDollar, StringComparison.OrdinalIgnoreCase))
        {
            return CurrencyCodes.UsDollar;
        }

        throw new CurrencyConversionException("The currency field must be 'USD'.");
    }

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

    private string ConvertEnglishScale(long number, int scale, string scaleName)
    {
        var scalePart = number / scale;
        var remainder = number % scale;
        var prefix = $"{ConvertEnglishNumber(scalePart)} {scaleName}";
        return remainder == 0 ? prefix : $"{prefix} {ConvertEnglishNumber(remainder)}";
    }

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

    private string ConvertGermanCurrencyNumber(long number)
    {
        return number == 1 ? "ein" : ConvertGermanNumber(number);
    }

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

    private string ConvertGermanSmallScale(long number)
    {
        return number == 1 ? "ein" : ConvertGermanUnderMillion(number);
    }

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

    private string ConvertGermanSmall(int number, bool finalPosition)
    {
        var germanRules = _rules.GetLanguageRules(LanguageCodes.German);
        if (number == 1 && finalPosition)
        {
            return "eins";
        }

        return germanRules.Small[number];
    }

    private static string GetMainCurrencyName(string language, long value)
    {
        if (language == LanguageCodes.German)
        {
            return "Dollar";
        }

        return GetEnglishPlural("dollar", value);
    }

    private static string GetCentCurrencyName(string language, int value)
    {
        return language == LanguageCodes.German ? "Cent" : GetEnglishPlural("cent", value);
    }

    private static string GetEnglishPlural(string singular, long value)
    {
        return value == 1 ? singular : $"{singular}s";
    }

    private readonly record struct NormalizedAmount(long IntegerPart, int CentPart, bool HasCentPart)
    {
        public string ToDisplayValue()
        {
            return HasCentPart
                ? $"{IntegerPart},{CentPart:00}"
                : IntegerPart.ToString();
        }
    }
}
