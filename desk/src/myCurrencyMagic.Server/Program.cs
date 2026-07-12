using Microsoft.AspNetCore.Mvc;
using myCurrencyMagic.Server.Conversion;
using myCurrencyMagic.Shared.Contracts;
using Serilog;
using Serilog.Events;

Log.Logger = CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddOpenApi();
    builder.Services.AddProblemDetails();
    builder.Services.AddSingleton<INumberConversionRulesProvider, JsonNumberConversionRulesProvider>();
    builder.Services.AddSingleton<ICurrencyConverterService, CurrencyConverterService>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.Lifetime.ApplicationStarted.Register(() => Log.Information("Server listening started."));
    app.Lifetime.ApplicationStopping.Register(() => Log.Information("Server stopping."));
    app.Lifetime.ApplicationStopped.Register(() => Log.Information("Server stopped."));

    app.MapPost(ApiRoutes.Convert, (
            HttpContext httpContext,
            [FromBody] ConvertCurrencyRequest? request,
            ICurrencyConverterService converter,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("Convert request started. TraceId: {TraceId}", httpContext.TraceIdentifier);

            if (!HasValidClientHeader(httpContext.Request))
            {
                logger.LogWarning(
                    "Convert request rejected because client header {HeaderName} is missing or invalid. TraceId: {TraceId}",
                    ApiHeaders.ClientHeaderName,
                    httpContext.TraceIdentifier);

                return Results.Problem(
                    title: "Invalid client header.",
                    detail: $"The required header '{ApiHeaders.ClientHeaderName}' is missing or invalid.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var validationFailure = ValidateRequest(request);
            if (validationFailure is not null)
            {
                logger.LogWarning(
                    "Convert request rejected because validation failed: {Reason}. TraceId: {TraceId}",
                    validationFailure.Detail,
                    httpContext.TraceIdentifier);

                return validationFailure.Result;
            }

            try
            {
                var response = converter.Convert(request!);
                logger.LogInformation(
                    "Convert request completed for language {Language}, currency {Currency}, normalized amount {NormalizedAmount}. TraceId: {TraceId}",
                    response.Language,
                    response.Currency,
                    response.NormalizedAmount,
                    httpContext.TraceIdentifier);

                return Results.Ok(response);
            }
            catch (CurrencyConversionException exception)
            {
                logger.LogWarning(
                    exception,
                    "Convert request failed in conversion service for language {Language}, currency {Currency}. TraceId: {TraceId}",
                    request!.Language,
                    request.Currency,
                    httpContext.TraceIdentifier);

                return Results.Problem(
                    title: "Invalid conversion request.",
                    detail: exception.Message,
                    statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled convert request error. TraceId: {TraceId}", httpContext.TraceIdentifier);
                throw;
            }
        })
        .WithName("ConvertCurrency")
        .Accepts<ConvertCurrencyRequest>("application/json")
        .Produces<ConvertCurrencyResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Server terminated unexpectedly.");
}
finally
{
    Log.Information("Server connection ended.");
    Log.CloseAndFlush();
}

static Serilog.ILogger CreateLogger()
{
    return new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/log-.txt",
            rollingInterval: RollingInterval.Hour,
            retainedFileCountLimit: 168,
            buffered: false,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
}

static bool HasValidClientHeader(HttpRequest request)
{
    return request.Headers.TryGetValue(ApiHeaders.ClientHeaderName, out var headerValues)
        && headerValues.Count == 1
        && string.Equals(
            headerValues[0],
            ApiHeaders.DefaultClientHeaderValue,
            StringComparison.Ordinal);
}

static ValidationFailure? ValidateRequest(ConvertCurrencyRequest? request)
{
    if (request is null)
    {
        return CreateValidationFailure("The request body is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Language))
    {
        return CreateValidationFailure("The language field is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Currency))
    {
        return CreateValidationFailure("The currency field is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Amount))
    {
        return CreateValidationFailure("The amount field is required.");
    }

    if (!IsSupportedLanguage(request.Language))
    {
        return CreateValidationFailure("The language field must be 'en' or 'de'.");
    }

    if (!IsSupportedCurrency(request.Currency))
    {
        return CreateValidationFailure("The currency field must be 'USD' or 'EUR'.");
    }

    return null;
}

static ValidationFailure CreateValidationFailure(string detail)
{
    return new ValidationFailure(
        detail,
        Results.Problem(
            title: "Invalid conversion request.",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest));
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

internal sealed record ValidationFailure(string Detail, IResult Result);

public partial class Program;
