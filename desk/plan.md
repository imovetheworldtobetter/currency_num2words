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
   - `dotnet --info` currently defaults to SDK 10.0.301 because no `global.json` exists yet.

### Next Todo

2. Create solution and projects.
   - Create classic Visual Studio compatible `myCurrencyMagic.sln`.
   - Create:

```text
src/myCurrencyMagic.Shared
src/myCurrencyMagic.Server
src/myCurrencyMagic.Client
tests/myCurrencyMagic.UnitTests
tests/myCurrencyMagic.IntegrationTests
```

   - Add project references.
   - Stop after this step so the solution can be opened and manually checked in Visual Studio Community.

3. Define shared contracts.
   - Add request and response DTOs.
   - Add shared language and currency models.
   - Keep shared code free of business logic.

4. Create Minimal API server.
   - Add `POST /convert`.
   - Accept `language`, `currency`, and `amount`.
   - Validate the lightweight client header.
   - Return `ProblemDetails` for errors.

5. Implement conversion service.
   - Add `CurrencyConverterService`.
   - Combine algorithmic conversion code with maintainable rule/table data.
   - Load rule/table data at server startup where useful.
   - Correctly handle German umlauts and sharp s.
   - Correctly handle `eintausend` and `zweitausend`.
   - Correctly handle `eine Million` and `zwei Millionen`.
   - Use US English wording.
   - Defensively normalize inputs such as `7,` to `7,00` even though the GUI should not send them.

6. Create WPF client.
   - Implement custom window chrome.
   - Implement language switch EN/DE.
   - Set currency explicitly from selected language.
   - Validate and format the amount input.
   - Send HTTP requests with the client header.
   - Use timeout and max 3 retries through `Microsoft.Extensions.Http.Resilience`.
   - Display conversion results and server connectivity errors.

7. Create tests.
   - Add unit tests for `CurrencyConverterService`.
   - Add integration tests for `/convert`.
   - Cover routing, JSON serialization, header validation, DI, success cases, and error cases.
   - Use the provided `dev_assets/test_cases.md` cases and add focused edge cases where useful.

8. First validation.
   - Run `dotnet restore`.
   - Run `dotnet build`.
   - Run `dotnet test`.
   - Manually start server and client where feasible.

11. Update README.
   - Add development environment prerequisites.
   - Add command-line startup instructions.
   - Add Visual Studio Community startup instructions.
   - Add command-line test instructions.
   - Add Visual Studio Community test instructions.

9. Add logging.
   - Add Serilog after the first validation and README update.
   - Use `Serilog.AspNetCore`.
   - Use `Serilog.Sinks.File`.
   - Write rolling hourly log files.
   - Use `logs/log-YYYY-MM-DD-HH.txt`.
   - Log request start, request end, information, warnings, and errors.

10. Externalize configuration.
   - Move server base URL, port, timeout, retry count, client header settings, supported languages, supported currencies, and UI texts into configuration files where practical.
   - Keep defaults suitable for local development.
   - Keep the design remote-server-ready without adding remote security in version 1.

## Open Points / Issues For Later

- Decide whether a future remote server uses HTTPS with authentication, certificates, Windows authentication, or another security mechanism.
- Decide whether currency selection becomes an explicit GUI control after the first version.
- Decide whether conversion rules should later become a formal versioned rules package.
