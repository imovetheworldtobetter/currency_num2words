using Microsoft.AspNetCore.Mvc;
using myCurrencyMagic.Server.Conversion;
using myCurrencyMagic.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<INumberConversionRulesProvider, JsonNumberConversionRulesProvider>();
builder.Services.AddSingleton<ICurrencyConverterService, CurrencyConverterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost(ApiRoutes.Convert, (
        HttpContext httpContext,
        [FromBody] ConvertCurrencyRequest? request,
        ICurrencyConverterService converter) =>
    {
        if (!HasValidClientHeader(httpContext.Request))
        {
            return Results.Problem(
                title: "Invalid client header.",
                detail: $"The required header '{ApiHeaders.ClientHeaderName}' is missing or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var requestValidationProblem = ValidateRequest(request);
        if (requestValidationProblem is not null)
        {
            return requestValidationProblem;
        }

        try
        {
            var response = converter.Convert(request!);
            return Results.Ok(response);
        }
        catch (CurrencyConversionException exception)
        {
            return Results.Problem(
                title: "Invalid conversion request.",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    })
    .WithName("ConvertCurrency")
    .Accepts<ConvertCurrencyRequest>("application/json")
    .Produces<ConvertCurrencyResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized);

app.Run();

static bool HasValidClientHeader(HttpRequest request)
{
    return request.Headers.TryGetValue(ApiHeaders.ClientHeaderName, out var headerValues)
        && headerValues.Count == 1
        && string.Equals(
            headerValues[0],
            ApiHeaders.DefaultClientHeaderValue,
            StringComparison.Ordinal);
}

static IResult? ValidateRequest(ConvertCurrencyRequest? request)
{
    if (request is null)
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The request body is required.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (string.IsNullOrWhiteSpace(request.Language))
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The language field is required.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (string.IsNullOrWhiteSpace(request.Currency))
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The currency field is required.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (string.IsNullOrWhiteSpace(request.Amount))
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The amount field is required.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (!IsSupportedLanguage(request.Language))
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The language field must be 'en' or 'de'.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (!IsSupportedCurrency(request.Currency))
    {
        return Results.Problem(
            title: "Invalid conversion request.",
            detail: "The currency field must be 'USD' or 'EUR'.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    return null;
}

static bool IsSupportedLanguage(string language)
{
    return string.Equals(language, LanguageCodes.English, StringComparison.OrdinalIgnoreCase)
        || string.Equals(language, LanguageCodes.German, StringComparison.OrdinalIgnoreCase);
}

static bool IsSupportedCurrency(string currency)
{
    return string.Equals(currency, CurrencyCodes.UsDollar, StringComparison.OrdinalIgnoreCase)
        || string.Equals(currency, CurrencyCodes.Euro, StringComparison.OrdinalIgnoreCase);
}

public partial class Program;
