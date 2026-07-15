/*
 * Purpose: Holds Serilog file sink settings for server logging.
*/

namespace myCurrencyMagic.Server.Configuration;

public sealed class ServerLoggingOptions
{
    public const string SectionName = "ServerLogging";

    public string FilePath { get; init; } = "logs/log-.txt";

    public int RetainedFileCountLimit { get; init; } = 168;

    public int FlushToDiskIntervalSeconds { get; init; } = 1;
}
