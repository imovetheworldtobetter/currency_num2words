# myCurrencyMagic Development Plan

## Goal

Build a modular, testable, and extensible .NET 9 client-server desktop application named `myCurrencyMagic`.

The Windows client provides the user interface. The local ASP.NET Core Minimal API server owns the currency conversion business logic. Client and server communicate only through HTTP.

## Confirmed Architecture Decisions

- Use .NET 9.
- Use WPF for the Windows desktop client.
- Use ASP.NET Core Minimal API for the server.
- Use a Visual Studio Community compatible classic `.sln` file.
- Keep the client as presentation layer only.
- Keep conversion logic on the server.
- Share contracts through a dedicated shared project.
- Use isolated unit tests for `CurrencyConverterService`.
- Use in-memory integration tests for the Minimal API.
- Use `Microsoft.Extensions.Http.Resilience` for HTTP timeout and retry behavior.
- Use a maximum of 3 retries.
- Use real German umlauts and sharp s in German conversion output.
- Use US English number wording.
- Send `language`, `currency`, and `amount` from client to server.
- Set `currency` explicitly in client code based on the selected UI language.
- Keep the currency model extensible so a later UI control can choose currency independently.
- Use the client header `X-myCurrencyMagic-Client`.
- Do not implement loopback-only request checks in the first version.
- Treat the client header as a lightweight marker, not as strong security.
- Plan stronger security separately if the server is later exposed remotely.

## Planned HTTP Contract

Endpoint:

```http
POST /convert
Content-Type: application/json
Accept: application/json
X-myCurrencyMagic-Client: <configured-value>
```

Request:

```json
{
  "language": "de",
  "currency": "EUR",
  "amount": "1 234,56"
}
```

Successful response:

```json
{
  "amountInWords": "eintausend zweihundertvierunddreißig Euro und sechsundfünfzig Cent",
  "normalizedAmount": "1234,56",
  "language": "de",
  "currency": "EUR"
}
```

Implementation note: German conversion output must use correct German characters such as umlauts and sharp s.

Errors use ASP.NET Core `ProblemDetails`.

## Implementation Plan

### Fulfilled

0. Transfer GUI requirements.
   - Create `dev_assets/requirements.md`.
   - Add section `GUI`.
   - Translate the GUI requirements from `dev_assets/GUI_requirements.txt` into English.
   - Add clarified requirements for sending `language`, `currency`, and `X-myCurrencyMagic-Client`.

1. Secure current state and verify setup.
   - Git status checked.
   - .NET SDK 9.0.315 is installed.
   - .NET 9 ASP.NET Core, .NET runtime, and Windows Desktop runtime are installed.
   - WPF, Web API, and xUnit templates are available.
   - Workspace structure and key assets checked.
   - JSON assets parse successfully.
   - UTF-8 handling for German umlauts and sharp s is confirmed.
   - Before `global.json` was created, `dotnet --info` defaulted to SDK 10.0.301.

2. Create solution and projects.
   - Created `global.json` in the workspace root to pin SDK 9.0.315.
   - Confirmed `dotnet --version` resolves to 9.0.315 in the workspace.
   - Created classic Visual Studio compatible `myCurrencyMagic.sln`.
   - Created:

```text
src/myCurrencyMagic.Shared
src/myCurrencyMagic.Server
src/myCurrencyMagic.Client
tests/myCurrencyMagic.UnitTests
tests/myCurrencyMagic.IntegrationTests
```

   - Added all projects to the solution.
   - Added project references:
     - `myCurrencyMagic.Server` -> `myCurrencyMagic.Shared`
     - `myCurrencyMagic.Client` -> `myCurrencyMagic.Shared`
     - `myCurrencyMagic.UnitTests` -> `myCurrencyMagic.Server`
     - `myCurrencyMagic.IntegrationTests` -> `myCurrencyMagic.Server`
   - Verified `myCurrencyMagic.Client` targets `net9.0-windows` and uses WPF.
   - Ran `dotnet restore myCurrencyMagic.sln` successfully.
   - Ran `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.
   - Stop after this step so the solution can be opened and manually checked in Visual Studio Community.

3. Define shared contracts.
   - Added `ConvertCurrencyRequest` with `Language`, `Currency`, and `Amount`.
   - Added `ConvertCurrencyResponse` with `AmountInWords`, `NormalizedAmount`, `Language`, and `Currency`.
   - Added shared contract constants for API route, client header name, language codes, and currency codes.
   - Removed the generated `Class1.cs` placeholder from the Shared project.
   - Kept shared code free of business logic.
   - Ran `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.

4. Create Minimal API server.
   - Replaced the template `/weatherforecast` endpoint with `POST /convert`.
   - The endpoint accepts `language`, `currency`, and `amount` through `ConvertCurrencyRequest`.
   - The endpoint validates the lightweight client header `X-myCurrencyMagic-Client`.
   - The endpoint validates required fields and supported initial language/currency codes.
   - The endpoint returns `ProblemDetails` for validation and header errors.
   - The endpoint deliberately returns `501 Not Implemented` until `CurrencyConverterService` is implemented in step 5.
   - Updated `myCurrencyMagic.Server.http` with a `POST /convert` example.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.

5. Implement conversion service.
   - Added server-side `CurrencyConverterService`.
   - Added conversion service interfaces and JSON numeric-rule provider.
   - Linked `dev_assets/rules_numeric_conversion.json` into the server output as UTF-8 content.
   - Combined algorithmic conversion code with loaded numeric rule tables.
   - Wired `/convert` to return `ConvertCurrencyResponse` instead of the temporary `501 Not Implemented`.
   - Applied semantic normalization rules for spaces, leading zeros, and decimal zero-padding.
   - Correctly handles German umlauts and sharp s from the UTF-8 rule file.
   - Correctly handles `ein Euro`, `eintausend`, `zweitausend`, `eine Million`, and `zwei Millionen`.
   - Uses US English wording with hyphens for compound tens.
   - Defensively normalizes inputs such as `7,` to `7,00` even though the GUI should not send them.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.
   - Smoke-tested `/convert` on a temporary local port for German and English conversion cases.

6. Create WPF client.
   - Added WPF dependency injection through `Microsoft.Extensions.Hosting`.
   - Added HTTP client factory and `Microsoft.Extensions.Http.Resilience`.
   - Implemented custom window chrome with purple-blue-violet gradient header.
   - Implemented EN/DE language switch in the top-right header area.
   - Set currency explicitly from selected language: EN -> USD, DE -> EUR.
   - Implemented amount input formatting with spaces as thousands separators.
   - Implemented input validation for allowed characters, format, comma rules, decimal length, and maximum value.
   - Implemented paste rejection for invalid character and invalid format cases.
   - Implemented client HTTP calls to `/convert` with `X-myCurrencyMagic-Client`.
   - Configured timeout behavior and max 3 retries through `Microsoft.Extensions.Http.Resilience`.
   - Implemented result display and server/connectivity error messaging.
   - Kept conversion business logic on the server.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.

7. Create tests.
   - Added unit tests for `CurrencyConverterService`.
   - Unit tests avoid API hosting and HTTP calls.
   - Unit tests cover German and English conversion cases from `dev_assets/test_cases.md`.
   - Unit tests cover normalization, leading zeros, decimal zero-padding, German umlauts, sharp s, currency singular/plural behavior, and German thousand/million special cases.
   - Unit tests cover invalid service requests.
   - Added `Microsoft.AspNetCore.Mvc.Testing` and `FluentAssertions` to the integration test project.
   - Added integration tests for `POST /convert`.
   - Integration tests start the API in-memory with `WebApplicationFactory<Program>`.
   - Integration tests cover routing, JSON serialization, header validation, DI wiring, success responses, API validation errors, and conversion-service validation errors.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.
   - Ran the requested escalated `dotnet test myCurrencyMagic.sln --no-build --logger "console;verbosity=normal"` successfully.
   - Test result: 38 unit tests passed and 8 integration tests passed.

8. First validation.
   - Ran `dotnet restore myCurrencyMagic.sln` successfully.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.
   - Ran the requested escalated `dotnet test myCurrencyMagic.sln --no-build --logger "console;verbosity=minimal"` successfully.
   - Test result: 38 unit tests passed and 8 integration tests passed.
   - Smoke-tested the real server process on a temporary local port with `POST /convert`.
   - Smoke-test response confirmed JSON response with German umlauts and sharp s.
   - Manual Visual Studio and GUI startup was reported successful by the user before this validation step.

11. Update README.
   - Added development environment prerequisites.
   - Documented SDK pinning through root `global.json`.
   - Added command-line restore and build instructions.
   - Added command-line server and client startup instructions.
   - Added Visual Studio Community startup instructions with multiple startup projects.
   - Added command-line test instructions.
   - Added Visual Studio Community test instructions.
   - Documented current expected test count.
   - Linked the known WPF build artifact issue to `knownproblems.md`.

9. Add logging.
   - Added `Serilog.AspNetCore` 9.0.0 to the server project.
   - Added `Serilog.Sinks.File` 7.0.0 to the server project.
   - Configured Serilog for console logging and rolling hourly file logging.
   - Configured immediate file writes with `buffered: false`.
   - Configured `flushToDiskInterval` to reduce log loss risk on crashes.
   - Configured log retention for 168 hourly files.
   - Added server lifecycle logging for start, stopping, stopped, fatal termination, and final flush.
   - Added conversion request start logging.
   - Added validation/header warning logging.
   - Added conversion-service warning logging.
   - Added unhandled conversion error logging.
   - Added Serilog HTTP request completion logging through `UseSerilogRequestLogging`.
   - Verified a real server request writes log entries to `logs/log-YYYYMMDDHH.txt` under the server content root.
   - Ran `dotnet restore myCurrencyMagic.sln` successfully.
   - Ran the requested escalated `dotnet build myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false` successfully.
   - Ran the requested escalated `dotnet test myCurrencyMagic.sln --no-build --logger "console;verbosity=minimal"` successfully.
   - Test result: 38 unit tests passed and 8 integration tests passed.

### Next Todo

10. Externalize configuration.
   - Move server base URL, port, timeout, retry count, client header settings, supported languages, supported currencies, and UI texts into configuration files where practical.
   - Keep defaults suitable for local development.
   - Keep the design remote-server-ready without adding remote security in version 1.

## Open Points / Issues For Later

- Decide whether a future remote server uses HTTPS with authentication, certificates, Windows authentication, or another security mechanism.
- Decide whether currency selection becomes an explicit GUI control after the first version.
- Decide whether conversion rules should later become a formal versioned rules package.
- Decide whether the Serilog rolling file name `log-YYYYMMDDHH.txt` is acceptable or whether exact hyphenated names like `log-YYYY-MM-DD-HH.txt` require a custom naming approach.
