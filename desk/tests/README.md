# Tests

## Test Projects

`myCurrencyMagic.UnitTests`

- Tests `CurrencyConverterService`.
- Does not host the API.
- Does not use HTTP.
- Covers German and English conversion, normalization, currency wording, German special cases, and invalid service requests.

`myCurrencyMagic.IntegrationTests`

- Starts the Minimal API in memory.
- Tests `POST /convert`.
- Covers routing, JSON serialization, required client header, validation errors, dependency injection, and success responses.

`Manual server checks`
- For manual server checks in Visual Studio, the file `src\myCurrencyMagic.Server\myCurrencyMagic.Server.http` contains a ready-to-run request that mirrors the API contract.

## Run Tests From Command Line

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

## Run Tests In Visual Studio Community

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
43 unit tests
12 integration tests
```