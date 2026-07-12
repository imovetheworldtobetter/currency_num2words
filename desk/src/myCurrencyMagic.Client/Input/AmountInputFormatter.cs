using myCurrencyMagic.Client.Configuration;

namespace myCurrencyMagic.Client.Input;

public sealed class AmountInputFormatter
{
    private const long MaxAmount = 999_999_999;

    public AmountInputFormatResult Format(string rawText, int caretIndex, ClientTextOptions texts)
    {
        var contentCharsBeforeCaret = CountContentCharacters(rawText, caretIndex);
        var cleaned = rawText.Replace(" ", string.Empty, StringComparison.Ordinal);
        var displayText = FormatCleanedText(cleaned);
        var validationMessage = ValidateCleanedText(cleaned, texts);
        var newCaretIndex = FindCaretIndex(displayText, contentCharsBeforeCaret);

        return new AmountInputFormatResult(
            displayText,
            newCaretIndex,
            validationMessage is null,
            validationMessage);
    }

    public bool ContainsOnlyAllowedCharacters(string text)
    {
        return text.All(character => char.IsDigit(character) || character == ',' || character == ' ');
    }

    public bool IsValidPasteText(string text)
    {
        if (!ContainsOnlyAllowedCharacters(text))
        {
            return false;
        }

        var cleaned = text.Replace(" ", string.Empty, StringComparison.Ordinal);
        return ValidateCleanedText(cleaned, ClientTextOptionsDefaults.Default) is null;
    }

    public string? ValidateDisplayText(string displayText, ClientTextOptions texts)
    {
        var cleaned = displayText.Replace(" ", string.Empty, StringComparison.Ordinal);
        return ValidateCleanedText(cleaned, texts);
    }

    private static string FormatCleanedText(string cleaned)
    {
        if (cleaned.Length == 0)
        {
            return string.Empty;
        }

        var commaIndex = cleaned.IndexOf(',', StringComparison.Ordinal);
        var integerPart = commaIndex >= 0 ? cleaned[..commaIndex] : cleaned;
        var decimalPart = commaIndex >= 0 ? cleaned[(commaIndex + 1)..] : string.Empty;
        var formattedInteger = FormatIntegerPart(integerPart);

        return commaIndex >= 0 ? $"{formattedInteger},{decimalPart}" : formattedInteger;
    }

    private static string FormatIntegerPart(string integerPart)
    {
        if (integerPart.Length <= 3)
        {
            return integerPart;
        }

        var groups = new Stack<string>();
        for (var end = integerPart.Length; end > 0; end -= 3)
        {
            var start = Math.Max(0, end - 3);
            groups.Push(integerPart[start..end]);
        }

        return string.Join(' ', groups);
    }

    private static string? ValidateCleanedText(string cleaned, ClientTextOptions texts)
    {
        if (cleaned.Length == 0)
        {
            return null;
        }

        if (cleaned.Any(character => !char.IsDigit(character) && character != ','))
        {
            return texts.InvalidCharacterMessage;
        }

        if (cleaned.Count(character => character == ',') > 1)
        {
            return texts.InvalidFormatMessage;
        }

        var parts = cleaned.Split(',');
        var integerPart = parts[0];
        var hasComma = parts.Length == 2;
        var decimalPart = hasComma ? parts[1] : string.Empty;

        if (integerPart.Length == 0)
        {
            return texts.InvalidFormatMessage;
        }

        if (integerPart.Length > 9)
        {
            return texts.MaximumValueMessage;
        }

        if (hasComma && decimalPart.Length == 0)
        {
            return texts.DecimalDigitsMessage;
        }

        if (hasComma && decimalPart.Length > 2)
        {
            return texts.DecimalDigitsMessage;
        }

        var normalizedInteger = integerPart.TrimStart('0');
        if (normalizedInteger.Length == 0)
        {
            normalizedInteger = "0";
        }

        if (!long.TryParse(normalizedInteger, out var integerValue) || integerValue > MaxAmount)
        {
            return texts.MaximumValueMessage;
        }

        return null;
    }

    private static int CountContentCharacters(string text, int caretIndex)
    {
        var boundedCaretIndex = Math.Clamp(caretIndex, 0, text.Length);
        var count = 0;
        for (var index = 0; index < boundedCaretIndex; index++)
        {
            if (text[index] != ' ')
            {
                count++;
            }
        }

        return count;
    }

    private static int FindCaretIndex(string formattedText, int contentCharactersBeforeCaret)
    {
        if (contentCharactersBeforeCaret <= 0)
        {
            return 0;
        }

        var seen = 0;
        for (var index = 0; index < formattedText.Length; index++)
        {
            if (formattedText[index] == ' ')
            {
                continue;
            }

            seen++;
            if (seen == contentCharactersBeforeCaret)
            {
                return index + 1;
            }
        }

        return formattedText.Length;
    }
}

internal static class ClientTextOptionsDefaults
{
    public static ClientTextOptions Default { get; } = new();
}
