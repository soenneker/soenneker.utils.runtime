using System;

namespace Soenneker.Utils.Runtime;

/// <summary>
/// A collection of helpful runtime-based operations
/// </summary>
public class RuntimeUtil
{
    public static bool IsWindows()
    {
        return OperatingSystem.IsWindows();
    }

    public static bool IsMacOs()
    {
        return OperatingSystem.IsMacOS();
    }

    public static bool IsLinux()
    {
        return OperatingSystem.IsLinux();
    }

    public static bool IsAndroid()
    {
        return OperatingSystem.IsAndroid();
    }

    public static bool IsBrowser()
    {
        return OperatingSystem.IsBrowser();
    }

    public static bool IsIos()
    {
        return OperatingSystem.IsIOS();
    }
}