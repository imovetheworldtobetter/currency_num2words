# .NET Client-Server Architecture Guidelines

## Goal

Create a modular, testable, and extensible client-server application:

- Business logic runs on a local server.
- The user interface runs on a Windows client.
- Client and server communicate only through HTTP.

## Components

### Client: WPF or WinUI

Use the client as the presentation layer.

- Accept user input.
- Display results.
- Call server endpoints through HTTP.
- Keep view logic, UI state, and presentation concerns here.
- Do not implement business rules here.
- Do not directly reference server implementation projects.

### Server: ASP.NET Core Minimal API

Use the server as the business-logic host.

- Provide HTTP endpoints.
- Process requests.
- Execute business logic.
- Perform logging.
- Compose services through dependency injection.
- Own server-side configuration and runtime behavior.

### Shared Library

Use the shared library as the contract between client and server.

- Place DTOs here.
- Place shared interfaces here only when they are true cross-boundary contracts.
- Place enums here when both sides need the same values.
- Avoid putting business logic, persistence logic, UI logic, or server infrastructure in this project.

### Unit Tests

Use unit tests for isolated business-logic validation.

- Test domain/application services without hosting the API.
- Avoid network calls.
- Avoid dependency on routing, JSON serialization, or HTTP concerns.
- Mock or fake infrastructure dependencies as needed.

### Integration Tests

Use integration tests for end-to-end API behavior within the server boundary.

- Test routing.
- Test JSON request and response behavior.
- Test dependency injection wiring.
- Test Minimal API endpoints.
- Prefer in-memory hosting or test server patterns where practical.

## Best Practices

### Modularity

Client and server must work standalone:

- The client should be runnable and testable without embedding server implementation logic.
- The server should be runnable and testable without the client.
- Contracts should be explicit and stable.

### Configuration

Move configurable values into separate configuration files.

Typical configuration values include:

- Selectable languages.
- Client design colors.
- Theme values.
- Server URL or endpoint base address.
- Feature flags.
- Environment-specific values.

Avoid hard-coding these values directly into UI code, server code, or tests unless a test explicitly verifies a default.

## Suggested Project Dependencies

Use a dependency direction like this:

```text
Product.Client -> Product.Shared
Product.Server -> Product.Shared
Product.UnitTests -> Product.Server or business-logic projects
Product.IntegrationTests -> Product.Server
```

Avoid this dependency:

```text
Product.Client -> Product.Server
```

## Practical Validation

Before considering the architecture complete, verify:

- The client can be started independently.
- The server can be started independently.
- Client-server communication goes through HTTP.
- Business logic is covered by isolated unit tests.
- API behavior is covered by integration tests.
- DTOs and shared enums live in the shared library.
- Configuration values are externalized.
