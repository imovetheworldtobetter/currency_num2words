using System.Windows.Input;
using myCurrencyMagic.Client.Infrastructure;
using myCurrencyMagic.Client.Input;
using myCurrencyMagic.Client.Services;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly ICurrencyConversionClient _client;
    private readonly AmountInputFormatter _formatter;
    private string _language = LanguageCodes.English;
    private string _currency = CurrencyCodes.UsDollar;
    private string _amountText = string.Empty;
    private string? _validationMessage;
    private string? _serverMessage;
    private string _outputText = string.Empty;
    private bool _isConverting;

    public MainWindowViewModel(ICurrencyConversionClient client, AmountInputFormatter formatter)
    {
        _client = client;
        _formatter = formatter;
        SwitchToEnglishCommand = new RelayCommand(SwitchToEnglish);
        SwitchToGermanCommand = new RelayCommand(SwitchToGerman);
        ConvertCommand = new AsyncRelayCommand(ConvertAsync, CanConvert);
    }

    public ICommand SwitchToEnglishCommand { get; }

    public ICommand SwitchToGermanCommand { get; }

    public AsyncRelayCommand ConvertCommand { get; }

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

    public string CurrencySymbol => _currency == CurrencyCodes.Euro ? "€" : "$";

    public bool IsCurrencyLeading => _currency == CurrencyCodes.UsDollar;

    public bool IsCurrencyTrailing => _currency == CurrencyCodes.Euro;

    public string IntroText => IsGermanActive
        ? "Mit dieser Anwendung können Sie einen Geldbetrag als Zahl eingeben und in Worte umwandeln. Tippen Sie eine Zahl in das Eingabefeld. Wenn Sie fertig sind, drücken Sie Umwandeln. Ihren Geldbetrag in Worten finden Sie anschließend im Ausgabefeld."
        : "Use this application to convert a numeric currency amount into words. Type a number into the input field. When you are done, press Convert. The amount in words appears in the output field.";

    public string InputTitle => IsGermanActive ? "Eingabe Geldbetrag als Zahl" : "Input currency amount as number";

    public string OutputTitle => IsGermanActive ? "Ausgabe Geldbetrag in Worten" : "Output currency amount in words";

    public string ConvertButtonText => IsGermanActive ? "Umwandeln" : "Convert";

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
        return _formatter.Format(rawText, caretIndex);
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
        ValidationMessage = "Allowed characters: digits 0-9 and comma.";
        ServerMessage = null;
        ConvertCommand.RaiseCanExecuteChanged();
    }

    public void ShowInvalidFormatMessage()
    {
        ValidationMessage = "Use the format 123 456 789,12.";
        ServerMessage = null;
        ConvertCommand.RaiseCanExecuteChanged();
    }

    private bool CanConvert()
    {
        return !IsConverting
            && !string.IsNullOrWhiteSpace(AmountText)
            && _formatter.ValidateDisplayText(AmountText) is null;
    }

    private async Task ConvertAsync()
    {
        IsConverting = true;
        ServerMessage = null;
        OutputText = string.Empty;

        try
        {
            var request = new ConvertCurrencyRequest(_language, _currency, AmountText);
            var response = await _client.ConvertAsync(request, CancellationToken.None);
            OutputText = response.AmountInWords;
        }
        catch (CurrencyConversionClientException)
        {
            ServerMessage = IsGermanActive
                ? "Der Server ist nicht erreichbar oder konnte die Anfrage nicht verarbeiten."
                : "The server is not reachable or could not process the request.";
        }
        finally
        {
            IsConverting = false;
        }
    }

    private void SwitchToEnglish()
    {
        SetLanguage(LanguageCodes.English, CurrencyCodes.UsDollar);
    }

    private void SwitchToGerman()
    {
        SetLanguage(LanguageCodes.German, CurrencyCodes.Euro);
    }

    private void SetLanguage(string language, string currency)
    {
        _language = language;
        _currency = currency;
        OutputText = string.Empty;
        ServerMessage = null;

        OnPropertyChanged(nameof(IsEnglishActive));
        OnPropertyChanged(nameof(IsGermanActive));
        OnPropertyChanged(nameof(CurrencySymbol));
        OnPropertyChanged(nameof(IsCurrencyLeading));
        OnPropertyChanged(nameof(IsCurrencyTrailing));
        OnPropertyChanged(nameof(IntroText));
        OnPropertyChanged(nameof(InputTitle));
        OnPropertyChanged(nameof(OutputTitle));
        OnPropertyChanged(nameof(ConvertButtonText));

        ((RelayCommand)SwitchToEnglishCommand).RaiseCanExecuteChanged();
        ((RelayCommand)SwitchToGermanCommand).RaiseCanExecuteChanged();
        ConvertCommand.RaiseCanExecuteChanged();
    }
}
