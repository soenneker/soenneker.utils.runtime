using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;


#if WINDOWS
using Microsoft.Win32;
#endif

namespace Soenneker.Utils.Runtime;

/// <summary>
/// A collection of helpful runtime-based environment and platform detection utilities.
/// </summary>
public static class RuntimeUtil
{
    /// <summary>
    /// Determines whether the current operating system is Windows.
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows() => OperatingSystem.IsWindows();

    /// <summary>
    /// Determines whether the current operating system is macOS.
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMacOs() => OperatingSystem.IsMacOS();

    /// <summary>
    /// Determines whether the current operating system is Linux.
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLinux() => OperatingSystem.IsLinux();

    /// <summary>
    /// Determines whether the current operating system is Android.
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAndroid() => OperatingSystem.IsAndroid();

    /// <summary>
    /// Determines whether the current operating system is a browser (WebAssembly).
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBrowser() => OperatingSystem.IsBrowser();

    /// <summary>
    /// Determines whether the current operating system is iOS.
    /// </summary>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIos() => OperatingSystem.IsIOS();

    /// <summary>
    /// Lazily determines whether the current process is running inside a GitHub Actions
    /// or generic CI environment.
    /// </summary>
    private static readonly Lazy<bool> _isGitHubAction = new(() =>
    {
        string? actionStr = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") ??
                            Environment.GetEnvironmentVariable("CI");

        return actionStr != null && actionStr.EqualsIgnoreCase("true");
    }, isThreadSafe: true);

    /// <summary>
    /// Gets a value indicating whether the current process is running inside
    /// GitHub Actions or a compatible CI environment.
    /// </summary>
    public static bool IsGitHubAction => _isGitHubAction.Value;

    /// <summary>
    /// Lazily determines whether the current process is running inside an Azure Function.
    /// </summary>
    private static readonly Lazy<bool> _isAzureFunction =
        new(() => Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME").HasContent(), true);

    /// <summary>
    /// Gets a value indicating whether the current process is running inside an Azure Function.
    /// </summary>
    public static bool IsAzureFunction => _isAzureFunction.Value;

    /// <summary>
    /// Lazily determines whether the current process is running inside an Azure App Service
    /// (Code, Custom Container, Windows, or Linux).
    /// </summary>
    private static readonly Lazy<bool> _isAzureAppService =
        new(() => (Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ??
                   Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")).HasContent(), true);

    /// <summary>
    /// Gets a value indicating whether the current process is running inside an Azure App Service.
    /// </summary>
    public static bool IsAzureAppService => _isAzureAppService.Value;

    /// <summary>
    /// Async singleton that determines whether the current process is running inside a container.
    /// The result is cached after the first evaluation.
    /// </summary>
    private static readonly AsyncSingleton.AsyncSingleton<bool?> _isContainer =
        new(async (token, _) => await DetectIsContainer(token).NoSync());

    /// <summary>
    /// Determines whether the current process is running inside a container
    /// (for example Docker or Kubernetes).
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token used while performing environment and filesystem checks.
    /// </param>
    /// <returns>
    /// A value indicating whether the current process is running inside a container.
    /// </returns>
    [Pure]
    public static async ValueTask<bool> IsContainer(CancellationToken cancellationToken = default)
        => (await _isContainer.Get(cancellationToken).NoSync()).GetValueOrDefault();

    /// <summary>
    /// Performs platform-specific heuristics to determine whether the current process
    /// is running inside a container.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token used while performing environment and filesystem checks.
    /// </param>
    /// <returns>
    /// A value indicating whether the current process is running inside a container.
    /// </returns>
    private static async ValueTask<bool> DetectIsContainer(CancellationToken cancellationToken = default)
    {
        // Fast-path environment variable hints
        string? inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") ??
                              Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINERS");

        if (inContainer != null && inContainer.EqualsIgnoreCase("true"))
            return true;

        if (OperatingSystem.IsLinux())
        {
            // Docker-specific marker file
            if (File.Exists("/.dockerenv"))
                return true;

            const string cgroupPath = "/proc/1/cgroup";

            if (!File.Exists(cgroupPath))
                return false;

            // PERF: stream line-by-line to avoid allocating the entire file contents
            await using var fs = new FileStream(
                cgroupPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan);

            using var reader = new StreamReader(fs);

            while (true)
            {
                string? line = await reader.ReadLineAsync(cancellationToken).NoSync();
                if (line is null)
                    break;

                if (line.Contains("docker", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("kubepods", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("containerd", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

#if WINDOWS
        if (OperatingSystem.IsWindows())
        {
            // Most Windows containers run as ContainerAdministrator under "User Manager"
            if (Environment.UserName == "ContainerAdministrator" &&
                Environment.UserDomainName == "User Manager")
            {
                return true;
            }

            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control");
                object? val = key?.GetValue("ContainerType");

                if (val is int i && i == 2)
                    return true;
            }
            catch
            {
                // Ignore registry read failures
            }

            return false;
        }
#endif

        // Unknown or unsupported platform
        return false;
    }
}
