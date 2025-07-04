using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;

#if WINDOWS
using Microsoft.Win32;
#endif

namespace Soenneker.Utils.Runtime;

/// <summary>
/// A collection of helpful runtime-based operations
/// </summary>
public static class RuntimeUtil
{
    /// <summary>
    /// Determines whether the current operating system is Windows.
    /// </summary>
    /// <returns>true if the current operating system is Windows; otherwise, false.</returns>
    [Pure]
    public static bool IsWindows()
    {
        return OperatingSystem.IsWindows();
    }

    /// <summary>
    /// Determines whether the current operating system is macOS.
    /// </summary>
    /// <returns>true if the current operating system is macOS; otherwise, false.</returns>
    [Pure]
    public static bool IsMacOs()
    {
        return OperatingSystem.IsMacOS();
    }

    /// <summary>
    /// Determines whether the current operating system is Linux.
    /// </summary>
    /// <returns>true if the current operating system is Linux; otherwise, false.</returns>
    [Pure]
    public static bool IsLinux()
    {
        return OperatingSystem.IsLinux();
    }

    /// <summary>
    /// Determines whether the current operating system is Android.
    /// </summary>
    /// <returns>true if the current operating system is Android; otherwise, false.</returns>
    [Pure]
    public static bool IsAndroid()
    {
        return OperatingSystem.IsAndroid();
    }

    /// <summary>
    /// Determines whether the current operating system is a browser.
    /// </summary>
    /// <returns>true if the current operating system is a browser; otherwise, false.</returns>
    [Pure]
    public static bool IsBrowser()
    {
        return OperatingSystem.IsBrowser();
    }

    /// <summary>
    /// Determines whether the current operating system is iOS.
    /// </summary>
    /// <returns>true if the current operating system is iOS; otherwise, false.</returns>
    [Pure]
    public static bool IsIos()
    {
        return OperatingSystem.IsIOS();
    }

    private static readonly Lazy<bool> _isGitHubAction = new(IsGitHubAction, true);

    /// <summary>
    /// Determines whether the current environment is a GitHub Action.
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static bool IsGitHubAction() => _isGitHubAction.Value;

    private static bool DetectIsGitHubAction()
    {
        string? actionStr = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") ?? Environment.GetEnvironmentVariable("CI");

        if (actionStr != null && actionStr.EqualsIgnoreCase("true"))
            return true;

        return false;
    }

    private static readonly Lazy<bool> _isAzureFunction = new(() => Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME").HasContent(), true);

    /// <summary>
    ///  Determines whether the current environment is an Azure Function.
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static bool IsAzureFunction => _isAzureFunction.Value;

    private static readonly Lazy<bool> _isAzureAppService =
        new(
            () => (Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ?? Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")).HasContent(), true);

    /// <summary>
    /// Code, Custom container, windows or linux
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static bool IsAzureAppService => _isAzureAppService.Value;

    private static readonly AsyncSingleton.AsyncSingleton<bool?> _isContainer = new(async (token, _) => await DetectIsContainer(token));

    /// <summary>
    /// Determines if the current OS is running inside a container (like Docker).
    /// </summary>
    /// <remarks>
    /// Not super dependable when it comes to running in various Windows container environments.
    /// </remarks>
    [Pure]
    public static async ValueTask<bool> IsContainer(CancellationToken cancellationToken = default)
    {
        return (await _isContainer.Get(cancellationToken)).GetValueOrDefault();
    }

    private static async ValueTask<bool> DetectIsContainer(CancellationToken cancellationToken = default)
    {
        string? inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") ??
                              Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINERS");

        if (inContainer != null && inContainer.EqualsIgnoreCase("true"))
            return true;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (File.Exists("/.dockerenv"))
                return true;

            const string cgroupPath = "/proc/1/cgroup";

            if (File.Exists(cgroupPath))
            {
                string[] lines = await File.ReadAllLinesAsync(cgroupPath, cancellationToken).NoSync();

                foreach (string line in lines)
                {
                    if (line.Contains("docker", StringComparison.OrdinalIgnoreCase) || line.Contains("kubepods", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("containerd", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

#if WINDOWS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Most Windows containers run as ContainerAdministrator under "User Manager"
            if (Environment.UserName == "ContainerAdministrator" && Environment.UserDomainName == "User Manager")
            {
                return true;
            }

            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control");
                object? val = key?.GetValue("ContainerType");

                if (val is 2)
                    return true;
            }
            catch
            {
                // Ignore registry read failures
            }

            return false;
        }
#endif

        // Unknown platform
        return false;
    }
}