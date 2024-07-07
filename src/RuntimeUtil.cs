using System;
using System.Diagnostics.Contracts;

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
}