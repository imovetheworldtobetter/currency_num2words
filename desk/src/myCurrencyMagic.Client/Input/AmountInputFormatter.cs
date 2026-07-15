/*
 * Purpose: Formats, normalizes, and validates amount input text for the client UI.
*/

using myCurrencyMagic.Client.Configuration;

namespace myCurrencyMagic.Client.Input;

public sealed class AmountInputFormatter
{
    private const long MaxAmount = 999_999_999;

    /*
     * Method: Format
     * Purpose: Formats raw amount text and validates it for UI display.
     * Input: rawText, caretIndex, and localized validation texts.
     * Output: A formatted amount, a corrected caret index, and validation metadata.
    */
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

    /*
     *  Method: ContainsOnlyAllowedCharacters
     *  Purpose: Checks whether the text only contains digits, comma, or spaces.
     *  Input: Raw input text.
     *  Output: True when only allowed characters are present.
    */
    public bool ContainsOnlyAllowedCharacters(string text)
    {
        return text.All(character => char.IsDigit(character) || character == ',' || character == ' ');
    }

    /*
     *  Method: IsValidPasteText
     *  Purpose: Validates pasted content before it is accepted by the input field.
     *  Input: Text from the clipboard.
     *  Output: True when the paste content would form a valid amount.
    */
    public bool IsValidPasteText(string text)
    {
        if (!ContainsOnlyAllowedCharacters(text))
        {
            return false;
        }

        var cleaned = text.Replace(" ", string.Empty, StringComparison.Ordinal);
        return ValidateCleanedText(cleaned, ClientTextOptionsDefaults.Default) is null;
    }

    /*
     *  Method: ValidateDisplayText
     *  Purpose: Validates text already shown in the input field.
     *  Input: Display text and localized validation messages.
     *  Output: A validation message when invalid; otherwise null.
    */
    public string? ValidateDisplayText(string displayText, ClientTextOptions texts)
    {
        var cleaned = displayText.Replace(" ", string.Empty, StringComparison.Ordinal);
        return ValidateCleanedText(cleaned, texts);
    }

    /*
     *  Method: FormatCleanedText
     *  Purpose: Adds thousands separators to cleaned numeric text.
     *  Input: Amount text without spaces.
     *  Output: A display-ready formatted amount string.
    */
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

    /*
     *  Method: FormatIntegerPart
     *  Purpose: Groups the integer part into three-digit blocks.
     *  Input: Integer-only amount text.
     *  Output: Integer text with space-separated thousands groups.
    */
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

    /*
     *  Method: ValidateCleanedText
     *  Purpose: Applies numeric validation rules to cleaned amount text.
     *  Input: Cleaned amount text and localized validation texts.
     *  Output: Validation text for invalid input; otherwise null.
    */
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

    /*
     *  Method: CountContentCharacters
     *  Purpose: Counts non-space characters before the caret.
     *  Input: Raw text and caret index.
     *  Output: Number of content characters before the caret.
    */
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

    /*
     *  Method: FindCaretIndex
     *  Purpose: Maps the logical caret position back to the formatted text.
     *  Input: Formatted text and content-character count before the caret.
     *  Output: New caret index in the formatted text.
    */
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
