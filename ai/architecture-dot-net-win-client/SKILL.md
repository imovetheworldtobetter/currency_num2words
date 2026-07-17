---
name: architecture-dot-net-win-client
description: Guidance for designing, implementing, or reviewing modular, testable, extensible .NET client-server applications with a Windows WPF/WinUI client, local ASP.NET Core Minimal API server, shared contracts library, unit tests, integration tests, HTTP-only client-server communication, and externalized configuration. Use when Codex needs to create project structure, architecture recommendations, code review feedback, implementation plans, or tests for this style of .NET application.
---

# Architecture Dot Net

## Core Workflow

Apply this architecture as the target shape for .NET desktop applications where business logic runs on a local server and the UI runs on a Windows client.

1. Separate responsibilities into Client, Server, Shared Library, Unit Tests, and Integration Tests.
2. Keep the client as a presentation layer only. Do not put business logic in WPF/WinUI views, view models, or client services.
3. Put business logic in the server or server-side domain/application services.
4. Communicate from client to server exclusively through HTTP.
5. Put DTOs, interfaces, and enums that form the contract between client and server in a shared library.
6. Test business logic with isolated unit tests that do not depend on the API host.
7. Test API behavior with integration tests that cover routing, JSON serialization, dependency injection, and endpoints.
8. Move configurable values into separate configuration files instead of hard-coding them.

Read `references/architecture-guidelines.md` when you need the detailed component responsibilities, validation checklist, or common project layout.

## Design Decisions

Prefer a solution/project layout similar to:

```text
src/
  Product.Client/
  Product.Server/
  Product.Shared/
tests/
  Product.UnitTests/
  Product.IntegrationTests/
```

Use names that match the existing repository if one already exists.

## Implementation Rules

- Keep Client and Server independently runnable and independently testable.
- Keep Client and Server loosely coupled through shared contracts and HTTP APIs.
- Avoid direct references from the Client to Server implementation projects.
- Avoid sharing business logic through `Product.Shared`; shared code should primarily be stable contracts.
- Use configuration files for selectable languages, theme colors, endpoint URLs, feature options, and similar values.
- Keep logging in the server-side request/business-processing path.
- Treat tests as first-class architecture components, not optional additions.

## Review Checklist

When reviewing code or architecture, flag these issues:

- Business logic implemented in the client.
- Client calling server code directly instead of HTTP endpoints.
- DTOs duplicated across client and server.
- Shared library containing too much behavior or infrastructure.
- Missing unit tests for business logic.
- Missing integration tests for Minimal API endpoints.
- Hard-coded UI languages, colors, endpoint URLs, or other configurable values.
- Client or server cannot run or be tested standalone.
