# Known Problems

## WPF Build Fails With Access Denied In Generated Files

### Symptom

During a command-line build, the WPF client project can fail with access denied errors in generated WPF build files.

Observed examples:

```text
Access to the path 'src\myCurrencyMagic.Client\myCurrencyMagic.Client_<id>_wpftmp.csproj' is denied.
```

```text
Access to the path 'src\myCurrencyMagic.Client\obj\Debug\net9.0-windows\MainWindow.g.cs' is denied.
```

```text
Access to the path 'src\myCurrencyMagic.Client\obj\Debug\net9.0-windows\myCurrencyMagic.Client_MarkupCompile.cache' is denied.
```

### Cause

The WPF build pipeline creates temporary project files and generated markup compilation files under the client project directory and under `obj`.

In this workspace, a failed or interrupted WPF build left generated files behind. A later build then failed when the WPF markup compiler tried to reuse, overwrite, or delete those files.

This is a generated build-output issue, not a source-code issue.

### Solution

Clean the generated WPF client build outputs and restore before building again.

```powershell
Remove-Item -LiteralPath .\src\myCurrencyMagic.Client\bin -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath .\src\myCurrencyMagic.Client\obj -Recurse -Force -ErrorAction SilentlyContinue
dotnet restore .\myCurrencyMagic.sln
dotnet build .\myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false
```

The verified successful build command was:

```powershell
dotnet build .\myCurrencyMagic.sln --no-restore -m:1 -nr:false /p:UseSharedCompilation=false
```

### Notes

- The issue affected only generated WPF build artifacts.
- Shared, Server, UnitTests, and IntegrationTests built successfully while the WPF markup compilation step failed.
- If Visual Studio encounters the same issue, close Visual Studio, remove the WPF client's `bin` and `obj` directories, reopen the solution, restore, and rebuild.
