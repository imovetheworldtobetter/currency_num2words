# User Configuration

Configuration changes are read when the client or server starts. Restart the affected process after changing a configuration file.

## Client Configuration

Client settings are stored in:

```text
src/myCurrencyMagic.Client/appsettings.json
```

The file is copied to the WPF output directory during build. If you change the source file, rebuild or ensure the updated file is copied before starting the client.

Configurable client values:

- `ClientRuntime:ServerBaseAddress`: HTTP base address of the server, for example `http://localhost:5034`.
- `ClientRuntime:ClientHeaderName`: request header name sent by the client.
- `ClientRuntime:ClientHeaderValue`: request header value sent by the client.
- `ClientRuntime:MaxRetryAttempts`: maximum retry attempts for failed HTTP requests. Current default is `2`.
- `ClientRuntime:AttemptTimeoutSeconds`: timeout for one request attempt.
- `ClientRuntime:TotalRequestTimeoutSeconds`: total timeout across retries.
- `ClientUi:DefaultLanguage`: initial UI language.
- `ClientUi:Languages:<language>:Currency`: currency sent for that language. The current default is `USD` for both `en` and `de`.
- `ClientUi:Languages:<language>:CurrencySymbol`: symbol shown in the UI.
- `ClientUi:Languages:<language>:IsCurrencyLeading`: controls whether the currency symbol is shown before or after the amount input.
- `ClientUi:Languages:<language>:Texts`: UI texts, validation messages, button text, and server error message.

Example client server change:

```json
{
  "ClientRuntime": {
    "ServerBaseAddress": "http://localhost:5034"
  }
}
```

## Server Configuration

Server settings are stored in:

```text
src/myCurrencyMagic.Server/appsettings.json
src/myCurrencyMagic.Server/appsettings.Development.json
```

Configurable server values:

- `Urls`: local server URL and port, for example `http://localhost:5034`.
- `Api:ClientHeader:Name`: required request header name.
- `Api:ClientHeader:ExpectedValue`: required request header value.
- `Api:SupportedLanguages`: language codes accepted by `POST /convert`.
- `Api:SupportedCurrencies`: currency codes accepted by `POST /convert`.
- `ServerLogging:FilePath`: Serilog rolling file path. Current default is `logs/log-.txt`.
- `ServerLogging:RetainedFileCountLimit`: number of retained hourly log files.
- `ServerLogging:FlushToDiskIntervalSeconds`: interval for flushing logs to disk.

Client and server header settings must match. If `ClientRuntime:ClientHeaderName` or `ClientRuntime:ClientHeaderValue` is changed, update `Api:ClientHeader:Name` or `Api:ClientHeader:ExpectedValue` accordingly.

The current Serilog file sink rolls hourly and appends the timestamp as `yyyyMMddHH`, for example:

```text
logs/log-2026071214.txt
```
