# myCurrencyMagic

`myCurrencyMagic` is a .NET 9 WPF desktop client and local ASP.NET Core Minimal API server for converting numeric currency amounts in USD into words.

The WPF client is the presentation layer. The server owns the conversion logic and is called through HTTP.

## Solution Structure

```text
src/
  myCurrencyMagic.Client
  myCurrencyMagic.Server
  myCurrencyMagic.Shared
tests/
  myCurrencyMagic.UnitTests
  myCurrencyMagic.IntegrationTests
dev_assets/
  rules_numeric_conversion.json
  rules_semantic_conversion.json
  test_cases.md
  requirements.md
```

## Development Environment

### Prerequisites

- Windows with Visual Studio Community 2022.
- .NET 9 SDK.
- ASP.NET Core 9 runtime.
- .NET 9 Windows Desktop runtime.

The repository contains a root-level `global.json` that pins the SDK to:

```text
9.0.315
```

Verify the SDK from the repository root:

```powershell
dotnet --version
```

Expected:

```text
9.0.315
```

### Restore And Build From Command Line

Run from the repository root:

```powershell
dotnet restore .\myCurrencyMagic.sln
dotnet build .\myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false
```

The additional build flags avoid known WPF temporary-file locking issues in some local environments.

### Start From Command Line

Open two terminals from the repository root.

Terminal 1, start the server:

```powershell
dotnet run --project .\src\myCurrencyMagic.Server\myCurrencyMagic.Server.csproj --launch-profile http
```

The server listens on:

```text
http://localhost:5034
```

Terminal 2, start the WPF client:

```powershell
dotnet run --project .\src\myCurrencyMagic.Client\myCurrencyMagic.Client.csproj
```

The client calls the server endpoint:

```text
POST http://localhost:5034/convert
```

The client sends the required header:

```text
X-myCurrencyMagic-Client: myCurrencyMagic.Client
```

### Start In Visual Studio Community

1. Open `myCurrencyMagic.sln`.
2. Set the startup configuration to multiple startup projects.
3. Start `myCurrencyMagic.Server` and `myCurrencyMagic.Client`.
4. Use the `http` launch profile for the server.
5. Start debugging or start without debugging.

The client expects the server at:

```text
http://localhost:5034
```

If Visual Studio shows unwanted IIS-related prompts, use the server `http` profile and keep Hot Reload disabled for the server profile if preferred.

### Build and run standalone executable

Open a terminal and run the following command from the client root to build the client executable, and from the server root to build the server executable.

```powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true
```

Start both created executables to run the application.


## User Configuration

Configuration changes are read when the client or server starts. Restart the affected process after changing a configuration file.

### Client Configuration

Client settings are stored in:

```text
src/myCurrencyMagic.Client/appsettings.json
```

The file is copied to the WPF output directory during build. If you change the source file, rebuild or ensure the updated file is copied before starting the client.

Configurable client values:

- `ClientRuntime:ServerBaseAddress`: HTTP base address of the server, for example `http://localhost:5034`.
- `ClientRuntime:ClientHeaderName`: request header name sent by the client.
- `ClientRuntime:ClientHeaderValue`: request header value sent by the client.
- `ClientRuntime:MaxRetryAttempts`: maximum retry attempts for failed HTTP requests. Current default is `3`.
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

### Server Configuration

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

## Tests

### Test Projects

`myCurrencyMagic.UnitTests`

- Tests `CurrencyConverterService`.
- Does not host the API.
- Does not use HTTP.
- Covers German and English conversion, normalization, currency wording, German special cases, and invalid service requests.

`myCurrencyMagic.IntegrationTests`

- Starts the Minimal API in memory.
- Tests `POST /convert`.
- Covers routing, JSON serialization, required client header, validation errors, dependency injection, and success responses.

### Run Tests From Command Line

Run from the repository root:

```powershell
dotnet test .\myCurrencyMagic.sln --no-build --settings .\myCurrencyMagic.tests.runsettings --logger "console;verbosity=minimal"
```

The runsettings file enables native VSTest TRX result files under:

```text
tests\TestResults
```

TRX files are XML files and include the executed tests, outcomes, timing, and run summary counters such as total, passed, and failed.

If the solution has not been built yet:

```powershell
dotnet restore .\myCurrencyMagic.sln
dotnet build .\myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false
dotnet test .\myCurrencyMagic.sln --no-build --settings .\myCurrencyMagic.tests.runsettings --logger "console;verbosity=minimal"
```

### Run Tests In Visual Studio Community

1. Open `myCurrencyMagic.sln`.
2. Build the solution.
3. Select `myCurrencyMagic.tests.runsettings` as the solution-wide runsettings file.
4. Open Test Explorer.
5. Run all tests or select a specific test project.

Visual Studio writes the same native TRX result files to:

```text
tests\TestResults
```

Expected current test count:

```text
37 unit tests
8 integration tests
```

## Known Build Issue

If WPF build artifacts are locked, the command-line build can fail with access denied errors in generated files under `bin` or `obj`.

See `knownproblems.md` for the cleanup command and details.
