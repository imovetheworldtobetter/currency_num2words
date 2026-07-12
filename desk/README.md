# myCurrencyMagic

`myCurrencyMagic` is a .NET 9 WPF desktop client and local ASP.NET Core Minimal API server for converting numeric currency amounts into words.

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
dotnet test .\myCurrencyMagic.sln --no-build --logger "console;verbosity=minimal"
```

If the solution has not been built yet:

```powershell
dotnet restore .\myCurrencyMagic.sln
dotnet build .\myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false
dotnet test .\myCurrencyMagic.sln --no-build --logger "console;verbosity=minimal"
```

### Run Tests In Visual Studio Community

1. Open `myCurrencyMagic.sln`.
2. Build the solution.
3. Open Test Explorer.
4. Run all tests or select a specific test project.

Expected current test count:

```text
38 unit tests
8 integration tests
```

## Known Build Issue

If WPF build artifacts are locked, the command-line build can fail with access denied errors in generated files under `bin` or `obj`.

See `knownproblems.md` for the cleanup command and details.
