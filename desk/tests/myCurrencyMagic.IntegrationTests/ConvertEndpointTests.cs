/*
 * Purpose: Verifies the HTTP API endpoint contract and routing behavior.
*/

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using myCurrencyMagic.Shared.Contracts;

namespace myCurrencyMagic.IntegrationTests;

public sealed class ConvertEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ConvertEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Convert_WithValidRequest_ReturnsSerializedConversionResponse()
    {
        using var client = _factory.CreateClient();
        using var request = CreateConvertRequest(new ConvertCurrencyRequest("de", "USD", "1 234,56"));

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("amountInWords").GetString()
            .Should().Be("eintausendzweihundertvierunddreißig Dollar und sechsundfünfzig Cent");
        document.RootElement.GetProperty("normalizedAmount").GetString().Should().Be("1234,56");
        document.RootElement.GetProperty("language").GetString().Should().Be("de");
        document.RootElement.GetProperty("currency").GetString().Should().Be("USD");

        var typedResponse = await response.Content.ReadFromJsonAsync<ConvertCurrencyResponse>();
        typedResponse.Should().NotBeNull();
        typedResponse!.AmountInWords.Should().Be("eintausendzweihundertvierunddreißig Dollar und sechsundfünfzig Cent");
    }

    [Fact]
    public async Task Convert_WithoutClientHeader_ReturnsProblemDetails()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Convert)
        {
            Content = JsonContent.Create(new ConvertCurrencyRequest("en", "USD", "57"))
        };

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        problem.GetProperty("title").GetString().Should().Be("Invalid client header.");
        problem.GetProperty("status").GetInt32().Should().Be(401);
    }

    [Theory]
    [InlineData("", "USD", "57", "The language field is required.")]
    [InlineData("fr", "USD", "57", "The language field must be 'en' or 'de'.")]
    [InlineData("en", "", "57", "The currency field is required.")]
    [InlineData("en", "GBP", "57", "The currency field must be 'USD'.")]
    [InlineData("en", "USD", "", "The amount field is required.")]
    public async Task Convert_WithInvalidApiRequest_ReturnsBadRequestProblemDetails(
        string language,
        string currency,
        string amount,
        string expectedDetail)
    {
        using var client = _factory.CreateClient();
        using var request = CreateConvertRequest(new ConvertCurrencyRequest(language, currency, amount));

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        problem.GetProperty("title").GetString().Should().Be("Invalid conversion request.");
        problem.GetProperty("detail").GetString().Should().Be(expectedDetail);
        problem.GetProperty("status").GetInt32().Should().Be(400);
    }

    [Fact]
    public async Task Convert_WithInvalidAmountFormat_ReturnsBadRequestFromConversionService()
    {
        using var client = _factory.CreateClient();
        using var request = CreateConvertRequest(new ConvertCurrencyRequest("en", "USD", "12,,34"));

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        problem.GetProperty("title").GetString().Should().Be("Invalid conversion request.");
        problem.GetProperty("detail").GetString().Should().Be("The amount field must contain at most one comma.");
    }

    private static HttpRequestMessage CreateConvertRequest(ConvertCurrencyRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Convert)
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add(ApiHeaders.ClientHeaderName, ApiHeaders.DefaultClientHeaderValue);
        return httpRequest;
    }
}
