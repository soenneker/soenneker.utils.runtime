[![](https://img.shields.io/nuget/v/soenneker.utils.runtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.runtime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.runtime/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.runtime/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.runtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.runtime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.runtime/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.runtime/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Runtime
A collection of helpful runtime-based operations.

## Installation

```bash
dotnet add package Soenneker.Utils.Runtime
```

## Quick start

```csharp
using Soenneker.Utils.Runtime;
```

Call the static `RuntimeUtil` methods directly; no dependency-injection registration is required.

## Common operations

- `IsWindows()` - Determines whether the current operating system is Windows.
- `IsMacOs()` - Returns whether the current process is running on macOS.
- `IsLinux()` - Returns whether the current process is running on Linux.
- `IsAndroid()` - Returns whether the current process is running on Android.
- `IsBrowser()` - Returns whether the current runtime is WebAssembly in a browser.
- `IsIos()` - Returns whether the current process is running on iOS.
- `IsContainer()` - Determines whether the current process is running inside a container (for example Docker or Kubernetes).
