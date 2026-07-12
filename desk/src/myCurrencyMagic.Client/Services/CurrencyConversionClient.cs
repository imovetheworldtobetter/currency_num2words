using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.Client.Services;

public sealed class CurrencyConversionClient : ICurrencyConversionClient
{
    private readonly HttpClient _httpClient;

    public CurrencyConversionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConvertCurrencyResponse> ConvertAsync(ConvertCurrencyRequest request, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Convert)
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add(ApiHeaders.ClientHeaderName, ApiHeaders.DefaultClientHeaderValue);

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var problemDetail = await ReadProblemDetailAsync(response, cancellationToken);
                throw new CurrencyConversionClientException(problemDetail);
            }

            var result = await response.Content.ReadFromJsonAsync<ConvertCurrencyResponse>(cancellationToken);
            return result ?? throw new CurrencyConversionClientException("The server returned an empty response.");
        }
        catch (CurrencyConversionClientException)
        {
            throw;
        }
        catch (TaskCanceledException exception)
        {
            throw new CurrencyConversionClientException("The server did not respond in time.", exception);
        }
        catch (HttpRequestException exception)
        {
            throw new CurrencyConversionClientException("The server is not reachable.", exception);
        }
    }

    private static async Task<string> ReadProblemDetailAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var fallback = $"The server returned HTTP {(int)response.StatusCode}.";
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.TryGetProperty("detail", out var detail)
                && detail.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return detail.GetString()!;
            }

            if (document.RootElement.TryGetProperty("title", out var title)
                && title.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(title.GetString()))
            {
                return title.GetString()!;
            }
        }
        catch (JsonException)
        {
            return fallback;
        }

        return fallback;
    }
}
