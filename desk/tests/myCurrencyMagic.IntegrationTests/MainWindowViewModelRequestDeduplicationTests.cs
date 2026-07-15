/*
 * Purpose: Verifies request deduplication behavior in the client view model.
*/

using FluentAssertions;
using System.Reflection;
using myCurrencyMagic.Client.Configuration;
using myCurrencyMagic.Client.Input;
using myCurrencyMagic.Client.Services;
using myCurrencyMagic.Client.ViewModels;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.IntegrationTests;

public sealed class MainWindowViewModelRequestDeduplicationTests
{
    [Fact]
    public async Task ConvertCommand_WithSameAmountTwice_SendsOnlyOneRequest()
    {
        var client = new RecordingCurrencyConversionClient();
        var viewModel = CreateViewModel(client);
        viewModel.SetFormattedAmount(new AmountInputFormatResult("57", 2, true, null));

        viewModel.ConvertCommand.Execute(null);
        await WaitForCallCountAsync(client, 1);

        viewModel.ConvertCommand.Execute(null);
        await Task.Delay(100);

        client.CallCount.Should().Be(1);
        viewModel.OutputText.Should().Be("en:57");
    }

    [Fact]
    public async Task SwitchLanguage_WithSameAmount_SendsFreshRequestForNewLanguage()
    {
        var client = new RecordingCurrencyConversionClient();
        var viewModel = CreateViewModel(client);
        viewModel.SetFormattedAmount(new AmountInputFormatResult("57", 2, true, null));

        viewModel.ConvertCommand.Execute(null);
        await WaitForCallCountAsync(client, 1);

        viewModel.SwitchToGermanCommand.Execute(null);
        await WaitForCallCountAsync(client, 2);

        client.Requests.Should().HaveCount(2);
        client.Requests[0].Language.Should().Be(LanguageCodes.English);
        client.Requests[1].Language.Should().Be(LanguageCodes.German);
        client.Requests[0].Amount.Should().Be("57");
        client.Requests[1].Amount.Should().Be("57");
    }

    [Fact]
    public async Task ConvertCommand_WithSameAmountButDifferentCurrency_SendsAnotherRequest()
    {
        var client = new RecordingCurrencyConversionClient();
        var viewModel = CreateViewModel(client);
        viewModel.SetFormattedAmount(new AmountInputFormatResult("57", 2, true, null));

        viewModel.ConvertCommand.Execute(null);
        await WaitForCallCountAsync(client, 1);

        SetPrivateField(viewModel, "_currency", "EUR");
        viewModel.ConvertCommand.Execute(null);
        await WaitForCallCountAsync(client, 2);

        client.Requests.Should().HaveCount(2);
        client.Requests[0].Currency.Should().Be(CurrencyCodes.UsDollar);
        client.Requests[1].Currency.Should().Be("EUR");
    }

    private static MainWindowViewModel CreateViewModel(RecordingCurrencyConversionClient client)
    {
        return new MainWindowViewModel(client, new AmountInputFormatter(), CreateUiOptions());
    }

    private static ClientUiOptions CreateUiOptions()
    {
        return new ClientUiOptions
        {
            DefaultLanguage = LanguageCodes.English,
            Languages = new Dictionary<string, ClientLanguageUiOptions>(StringComparer.OrdinalIgnoreCase)
            {
                [LanguageCodes.English] = new ClientLanguageUiOptions
                {
                    DisplayName = "US-EN",
                    Currency = CurrencyCodes.UsDollar,
                    CurrencySymbol = "$",
                    IsCurrencyLeading = true,
                    Texts = CreateTexts()
                },
                [LanguageCodes.German] = new ClientLanguageUiOptions
                {
                    DisplayName = "DE",
                    Currency = CurrencyCodes.UsDollar,
                    CurrencySymbol = "$",
                    IsCurrencyLeading = true,
                    Texts = CreateTexts()
                }
            }
        };
    }

    private static ClientTextOptions CreateTexts()
    {
        return new ClientTextOptions
        {
            IntroText = "Intro",
            InputTitle = "Input",
            OutputTitle = "Output",
            ConvertButtonText = "Convert",
            InvalidCharacterMessage = "Invalid characters.",
            InvalidFormatMessage = "Invalid format.",
            DecimalDigitsMessage = "Invalid decimals.",
            MaximumValueMessage = "Too large.",
            ServerErrorMessage = "Server error."
        };
    }

    private static async Task WaitForCallCountAsync(RecordingCurrencyConversionClient client, int expectedCallCount)
    {
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (client.CallCount < expectedCallCount && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }

        client.CallCount.Should().Be(expectedCallCount);
    }

    private static void SetPrivateField<T>(T instance, string fieldName, object value)
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field.Should().NotBeNull();
        field!.SetValue(instance, value);
    }

    private sealed class RecordingCurrencyConversionClient : ICurrencyConversionClient
    {
        public int CallCount { get; private set; }

        public List<ConvertCurrencyRequest> Requests { get; } = new();

        public Task<ConvertCurrencyResponse> ConvertAsync(ConvertCurrencyRequest request, CancellationToken cancellationToken)
        {
            CallCount++;
            Requests.Add(request);
            return Task.FromResult(new ConvertCurrencyResponse($"{request.Language}:{request.Amount}", request.Amount, request.Language, request.Currency));
        }
    }
}
