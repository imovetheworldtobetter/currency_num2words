namespace myCurrencyMagic.Client.Configuration;

public sealed class ClientRuntimeOptions
{
    public const string SectionName = "ClientRuntime";

    public Uri ServerBaseAddress { get; init; } = new("http://localhost:5034");

    public string ClientHeaderName { get; init; } = "X-myCurrencyMagic-Client";

    public string ClientHeaderValue { get; init; } = "myCurrencyMagic.Client";

    public int MaxRetryAttempts { get; init; } = 2;

    public int AttemptTimeoutSeconds { get; init; } = 1;

    public int TotalRequestTimeoutSeconds { get; init; } = 5;

    public TimeSpan AttemptTimeout => TimeSpan.FromSeconds(AttemptTimeoutSeconds);

    public TimeSpan TotalRequestTimeout => TimeSpan.FromSeconds(TotalRequestTimeoutSeconds);
}
