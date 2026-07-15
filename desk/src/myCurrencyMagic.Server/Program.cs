/*
 * Purpose: Configures and hosts the local Minimal API server for currency conversion.
*/

using Microsoft.AspNetCore.Mvc;
using myCurrencyMagic.Server.Configuration;
using myCurrencyMagic.Server.Conversion;
using myCurrencyMagic.Shared.Contracts;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var apiOptions = builder.Configuration
        .GetSection(ServerApiOptions.SectionName)
        .Get<ServerApiOptions>() ?? new ServerApiOptions();

    Log.Logger = CreateLogger(builder.Configuration);

    builder.Host.UseSerilog();

    builder.Services.AddOpenApi();
    builder.Services.AddProblemDetails();
    builder.Services.AddSingleton(apiOptions);
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
            ServerApiOptions apiConfiguration,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("Convert request started. TraceId: {TraceId}", httpContext.TraceIdentifier);

            if (!HasValidClientHeader(httpContext.Request, apiConfiguration))
            {
                logger.LogWarning(
                    "Convert request rejected because client header {HeaderName} is missing or invalid. TraceId: {TraceId}",
                    apiConfiguration.ClientHeader.Name,
                    httpContext.TraceIdentifier);

                return Results.Problem(
                    title: "Invalid client header.",
                    detail: $"The required header '{apiConfiguration.ClientHeader.Name}' is missing or invalid.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var validationFailure = ValidateRequest(request, apiConfiguration);
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

static Serilog.ILogger CreateLogger(IConfiguration configuration)
{
    var loggingOptions = configuration
        .GetSection(ServerLoggingOptions.SectionName)
        .Get<ServerLoggingOptions>() ?? new ServerLoggingOptions();

    return new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: loggingOptions.FilePath,
            rollingInterval: RollingInterval.Hour,
            retainedFileCountLimit: Math.Max(1, loggingOptions.RetainedFileCountLimit),
            buffered: false,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(Math.Max(1, loggingOptions.FlushToDiskIntervalSeconds)),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
}

static bool HasValidClientHeader(HttpRequest request, ServerApiOptions apiOptions)
{
    return request.Headers.TryGetValue(apiOptions.ClientHeader.Name, out var headerValues)
        && headerValues.Count == 1
        && string.Equals(
            headerValues[0],
            apiOptions.ClientHeader.ExpectedValue,
            StringComparison.Ordinal);
}

static ValidationFailure? ValidateRequest(ConvertCurrencyRequest? request, ServerApiOptions apiOptions)
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

    if (!apiOptions.IsSupportedLanguage(request.Language))
    {
        return CreateValidationFailure($"The language field must be {apiOptions.SupportedLanguagesMessage}.");
    }

    if (!apiOptions.IsSupportedCurrency(request.Currency))
    {
        return CreateValidationFailure($"The currency field must be {apiOptions.SupportedCurrenciesMessage}.");
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

internal sealed record ValidationFailure(string Detail, IResult Result);

public partial class Program;
