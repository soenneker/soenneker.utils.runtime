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

## Platform checks

```csharp
if (RuntimeUtil.IsWindows())
{
    // Windows-specific behavior
}

bool browser = RuntimeUtil.IsBrowser();
```

`IsWindows`, `IsMacOs`, `IsLinux`, `IsAndroid`, `IsBrowser`, and `IsIos` directly wrap the
corresponding `OperatingSystem` checks. They describe the current runtime platform; they do not
infer a deployment environment or distribution.

## Hosting environment hints

```csharp
bool actions = RuntimeUtil.IsGitHubAction;
bool functions = RuntimeUtil.IsAzureFunction;
bool appService = RuntimeUtil.IsAzureAppService;
```

- `IsGitHubAction` requires `GITHUB_ACTIONS=true`; a generic `CI=true` marker is not enough.
- `IsAzureFunction` checks for a non-empty `FUNCTIONS_WORKER_RUNTIME` value.
- `IsAzureAppService` checks for a non-empty `WEBSITE_SITE_NAME` or `WEBSITE_INSTANCE_ID` value.

Each property caches its first result for the process lifetime. Set environment variables before
the first access; later environment changes are not observed.

## Container detection

```csharp
bool container = await RuntimeUtil.IsContainer(cancellationToken);
```

Detection first checks `DOTNET_RUNNING_IN_CONTAINER`/`DOTNET_RUNNING_IN_CONTAINERS`. On Linux it
then checks `/.dockerenv` and scans `/proc/1/cgroup` for Docker, Kubernetes, or containerd markers.
On Windows it checks the common container identity and `ContainerType` registry value. Other
platforms return `false`.

The first successful detection result is cached. The cancellation token applies while the initial
Linux cgroup file is read; later calls normally return the cached value. Detection is heuristic and
can have false negatives on container runtimes that expose none of these markers. Do not use it as
a security boundary or authorization decision. Filesystem access errors from the Linux cgroup
probe can propagate.
