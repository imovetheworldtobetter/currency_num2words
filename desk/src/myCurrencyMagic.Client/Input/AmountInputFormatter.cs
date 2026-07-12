namespace myCurrencyMagic.Client.Input;

public sealed class AmountInputFormatter
{
    private const long MaxAmount = 999_999_999;

    public AmountInputFormatResult Format(string rawText, int caretIndex)
    {
        var contentCharsBeforeCaret = CountContentCharacters(rawText, caretIndex);
        var cleaned = rawText.Replace(" ", string.Empty, StringComparison.Ordinal);
        var displayText = FormatCleanedText(cleaned);
        var validationMessage = ValidateCleanedText(cleaned);
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
        return ValidateCleanedText(cleaned) is null;
    }

    public string? ValidateDisplayText(string displayText)
    {
        var cleaned = displayText.Replace(" ", string.Empty, StringComparison.Ordinal);
        return ValidateCleanedText(cleaned);
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

    private static string? ValidateCleanedText(string cleaned)
    {
        if (cleaned.Length == 0)
        {
            return null;
        }

        if (cleaned.Any(character => !char.IsDigit(character) && character != ','))
        {
            return "Allowed characters: digits 0-9 and comma.";
        }

        if (cleaned.Count(character => character == ',') > 1)
        {
            return "Use the format 123 456 789,12.";
        }

        var parts = cleaned.Split(',');
        var integerPart = parts[0];
        var hasComma = parts.Length == 2;
        var decimalPart = hasComma ? parts[1] : string.Empty;

        if (integerPart.Length == 0)
        {
            return "Use the format 123 456 789,12.";
        }

        if (integerPart.Length > 9)
        {
            return "The maximum value is 999 999 999,99.";
        }

        if (hasComma && decimalPart.Length == 0)
        {
            return "A comma must be followed by one or two digits.";
        }

        if (hasComma && decimalPart.Length > 2)
        {
            return "Use one or two decimal digits after the comma.";
        }

        var normalizedInteger = integerPart.TrimStart('0');
        if (normalizedInteger.Length == 0)
        {
            normalizedInteger = "0";
        }

        if (!long.TryParse(normalizedInteger, out var integerValue) || integerValue > MaxAmount)
        {
            return "The maximum value is 999 999 999,99.";
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
