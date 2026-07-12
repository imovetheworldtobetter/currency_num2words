namespace myCurrencyMagic.Client.Configuration;

public sealed class ClientRuntimeOptions
{
    public Uri ServerBaseAddress { get; init; } = new("http://localhost:5034");

    public TimeSpan AttemptTimeout { get; init; } = TimeSpan.FromSeconds(5);

    public int MaxRetryAttempts { get; init; } = 3;
}
