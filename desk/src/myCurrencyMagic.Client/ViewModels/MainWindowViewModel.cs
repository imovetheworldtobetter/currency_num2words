using System.Windows.Input;
using myCurrencyMagic.Client.Configuration;
using myCurrencyMagic.Client.Infrastructure;
using myCurrencyMagic.Client.Input;
using myCurrencyMagic.Client.Services;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly ICurrencyConversionClient _client;
    private readonly AmountInputFormatter _formatter;
    private readonly ClientUiOptions _uiOptions;
    private string _language;
    private string _currency;
    private string _amountText = string.Empty;
    private string _lastSubmittedRequestSignature = string.Empty;
    private string? _validationMessage;
    private string? _serverMessage;
    private string _outputText = string.Empty;
    private bool _isConverting;

    public MainWindowViewModel(
        ICurrencyConversionClient client,
        AmountInputFormatter formatter,
        ClientUiOptions uiOptions)
    {
        _client = client;
        _formatter = formatter;
        _uiOptions = uiOptions;
        _language = _uiOptions.DefaultLanguage;
        _currency = CurrentLanguageOptions.Currency;
        SwitchToEnglishCommand = new RelayCommand(SwitchToEnglish);
        SwitchToGermanCommand = new RelayCommand(SwitchToGerman);
        ConvertCommand = new AsyncRelayCommand(() => ConvertAsync(forceRequest: false), CanConvert);
    }

    public ICommand SwitchToEnglishCommand { get; }

    public ICommand SwitchToGermanCommand { get; }

    public AsyncRelayCommand ConvertCommand { get; }

    public string EnglishLanguageLabel => _uiOptions.GetLanguage(LanguageCodes.English).DisplayName;

    public string GermanLanguageLabel => _uiOptions.GetLanguage(LanguageCodes.German).DisplayName;

    public string AmountText
    {
        get => _amountText;
        private set
        {
            if (SetProperty(ref _amountText, value))
            {
                OnPropertyChanged(nameof(IsInputEmpty));
            }
        }
    }

    public string? ValidationMessage
    {
        get => _validationMessage;
        private set
        {
            if (SetProperty(ref _validationMessage, value))
            {
                OnPropertyChanged(nameof(HasValidationMessage));
            }
        }
    }

    public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

    public string? ServerMessage
    {
        get => _serverMessage;
        private set
        {
            if (SetProperty(ref _serverMessage, value))
            {
                OnPropertyChanged(nameof(HasServerMessage));
            }
        }
    }

    public bool HasServerMessage => !string.IsNullOrWhiteSpace(ServerMessage);

    public string OutputText
    {
        get => _outputText;
        private set => SetProperty(ref _outputText, value);
    }

    public bool IsConverting
    {
        get => _isConverting;
        private set
        {
            if (SetProperty(ref _isConverting, value))
            {
                ConvertCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsEnglishActive => _language == LanguageCodes.English;

    public bool IsGermanActive => _language == LanguageCodes.German;

    public bool IsInputEmpty => string.IsNullOrEmpty(AmountText);

    public string CurrencySymbol => CurrentLanguageOptions.CurrencySymbol;

    public bool IsCurrencyLeading => CurrentLanguageOptions.IsCurrencyLeading;

    public bool IsCurrencyTrailing => !CurrentLanguageOptions.IsCurrencyLeading;

    public string IntroText => CurrentTexts.IntroText;

    public string InputTitle => CurrentTexts.InputTitle;

    public string OutputTitle => CurrentTexts.OutputTitle;

    public string ConvertButtonText => CurrentTexts.ConvertButtonText;

    public string PlaceholderText => "123 456 789,12";

    public void SetFormattedAmount(AmountInputFormatResult result)
    {
        AmountText = result.DisplayText;
        ValidationMessage = result.ValidationMessage;
        ServerMessage = null;
        ConvertCommand.RaiseCanExecuteChanged();
    }

    public AmountInputFormatResult FormatAmountText(string rawText, int caretIndex)
    {
        return _formatter.Format(rawText, caretIndex, CurrentTexts);
    }

    public bool ContainsOnlyAllowedAmountCharacters(string text)
    {
        return _formatter.ContainsOnlyAllowedCharacters(text);
    }

    public bool IsValidAmountPasteText(string text)
    {
        return _formatter.IsValidPasteText(text);
    }

    public void ShowInvalidCharacterMessage()
    {
        ValidationMessage = CurrentTexts.InvalidCharacterMessage;
        ServerMessage = null;
        ConvertCommand.RaiseCanExecuteChanged();
    }

    public void ShowInvalidFormatMessage()
    {
        ValidationMessage = CurrentTexts.InvalidFormatMessage;
        ServerMessage = null;
        ConvertCommand.RaiseCanExecuteChanged();
    }

    private bool CanConvert()
    {
        return !IsConverting
            && !string.IsNullOrWhiteSpace(AmountText)
            && _formatter.ValidateDisplayText(AmountText, CurrentTexts) is null;
    }

    private async Task ConvertAsync(bool forceRequest)
    {
        var currentRequestSignature = BuildRequestSignature();
        if (!forceRequest
            && string.Equals(currentRequestSignature, _lastSubmittedRequestSignature, StringComparison.Ordinal))
        {
            return;
        }

        IsConverting = true;
        ServerMessage = null;
        OutputText = string.Empty;
        _lastSubmittedRequestSignature = currentRequestSignature;

        try
        {
            var request = new ConvertCurrencyRequest(_language, _currency, AmountText);
            var response = await _client.ConvertAsync(request, CancellationToken.None);
            OutputText = response.AmountInWords;
        }
        catch (CurrencyConversionClientException)
        {
            ServerMessage = CurrentTexts.ServerErrorMessage;
        }
        finally
        {
            IsConverting = false;
        }
    }

    private void SwitchToEnglish()
    {
        SetLanguage(LanguageCodes.English);
    }

    private void SwitchToGerman()
    {
        SetLanguage(LanguageCodes.German);
    }

    private void SetLanguage(string language)
    {
        if (string.Equals(_language, language, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _language = language;
        _currency = CurrentLanguageOptions.Currency;
        ServerMessage = null;

        OnPropertyChanged(nameof(IsEnglishActive));
        OnPropertyChanged(nameof(IsGermanActive));
        OnPropertyChanged(nameof(EnglishLanguageLabel));
        OnPropertyChanged(nameof(GermanLanguageLabel));
        OnPropertyChanged(nameof(CurrencySymbol));
        OnPropertyChanged(nameof(IsCurrencyLeading));
        OnPropertyChanged(nameof(IsCurrencyTrailing));
        OnPropertyChanged(nameof(IntroText));
        OnPropertyChanged(nameof(InputTitle));
        OnPropertyChanged(nameof(OutputTitle));
        OnPropertyChanged(nameof(ConvertButtonText));

        ValidationMessage = string.IsNullOrWhiteSpace(AmountText)
            ? null
            : _formatter.ValidateDisplayText(AmountText, CurrentTexts);

        if (ValidationMessage is null && !string.IsNullOrWhiteSpace(AmountText))
        {
            _ = ConvertAsync(forceRequest: true);
        }
        else
        {
            OutputText = string.Empty;
        }

        ((RelayCommand)SwitchToEnglishCommand).RaiseCanExecuteChanged();
        ((RelayCommand)SwitchToGermanCommand).RaiseCanExecuteChanged();
        ConvertCommand.RaiseCanExecuteChanged();
    }

    private ClientLanguageUiOptions CurrentLanguageOptions => _uiOptions.GetLanguage(_language);

    private ClientTextOptions CurrentTexts => CurrentLanguageOptions.Texts;

    private string BuildRequestSignature()
    {
        return string.Join('|', _language, _currency, AmountText);
    }
}
