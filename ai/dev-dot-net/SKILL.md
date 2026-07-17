---
name: dev-dot-net
description: Practical .NET development setup and troubleshooting guidance for Codex. Use when creating, modifying, or debugging .NET solutions and projects, especially Visual Studio-compatible solution files, WPF projects targeting modern Windows frameworks such as net9.0-windows, and repository file edits that must remain ASCII-clean and free of shell escape artifacts.
---

# Dev Dot Net

## Core Workflow

Use these rules when setting up or troubleshooting .NET projects in this workspace style.

1. Prefer Visual Studio Community compatibility when creating solution files.
2. Treat WPF project templates and target frameworks as separate concerns when template creation rejects a requested framework.
3. Use `apply_patch` for manual file edits to avoid accidental encoding, escaping, or shell residue in source files.
4. Verify generated project files after creation instead of assuming CLI defaults match the requested format.

## Solution Files

When creating a solution intended for Visual Studio Community, create the classic `.sln` format explicitly:

```powershell
dotnet new sln --format sln --force
```

Use this when `dotnet new sln` produces `.slnx` or when the expected output is a traditional `.sln` file. After creation, check that the expected `.sln` file exists before adding projects.

## WPF Target Frameworks

When `dotnet new wpf -f net9.0-windows` or a similar command fails because the template does not accept the framework:

1. Create the WPF project with the default template options.
2. Edit the generated `.csproj`.
3. Set the target framework manually, for example:

```xml
<TargetFramework>net9.0-windows</TargetFramework>
```

After editing the project file, run a restore or build command appropriate for the repo to verify that the SDK accepts the target framework.

## File Editing

Use `apply_patch` for manual file changes. Avoid writing source files through shell commands when that risks introducing:

- Non-ASCII artifacts.
- Escaped quote or newline remnants.
- Encoding damage.
- PowerShell interpolation side effects.

Keep newly created or modified project and source files ASCII-clean unless the repository already uses non-ASCII content or the task explicitly requires it.

## Validation Checklist

Before handing off .NET setup work, verify:

- A Visual Studio-compatible `.sln` exists when Visual Studio Community usage is expected.
- WPF projects have the intended `TargetFramework` in the `.csproj`.
- Project files contain no accidental shell escape sequences.
- Manual edits were made through patch-style edits or another safe formatter.
- The relevant `dotnet restore`, `dotnet build`, or targeted validation command has been run when feasible.
