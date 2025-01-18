using System.Globalization;
using System.Reflection;

namespace VReflector;

public static class IsAssembly
{
    public static string? GetAssemblyName(Assembly assembly)
    {
        return assembly?.GetName().Name;
    }
    public static string? GetAssemblyVersion(Assembly assembly)
    {
        return Convert.ToString(assembly?.GetName().Version, CultureInfo.InvariantCulture);
    }
}
