using System.Net.Http;
using FluentAssertions;
using myCurrencyMagic.Client.Configuration;
using myCurrencyMagic.Client.Services;
using myCurrencyMagic.Shared.Contracts;
using Polly.Timeout;

namespace myCurrencyMagic.IntegrationTests;

public sealed class CurrencyConversionClientTimeoutTests
{
    [Fact]
    public async Task ConvertAsync_WhenHandlerTimesOut_ThrowsClientExceptionWithTimeoutMessage()
    {
        var httpClient = new HttpClient(new TimeoutThrowingHandler())
        {
            BaseAddress = new Uri("http://localhost")
        };
        var options = new ClientRuntimeOptions();
        var client = new CurrencyConversionClient(httpClient, options);

        var act = async () => await client.ConvertAsync(
            new ConvertCurrencyRequest("en", "USD", "57"),
            CancellationToken.None);

        var exception = await act.Should().ThrowAsync<CurrencyConversionClientException>();
        exception.Which.Message.Should().Be("The server did not respond in time.");
        exception.Which.InnerException.Should().BeOfType<TimeoutRejectedException>();
    }

    private sealed class TimeoutThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new TimeoutRejectedException("The operation didn't complete within the allowed timeout.");
        }
    }
}
