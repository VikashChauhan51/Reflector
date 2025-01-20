using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace VReflector;

public static class IsAssembly
{
    public static Assembly GetExecutingOrEntryAssembly()
    {
        return Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
    }
    public static string? GetAssemblyName()
    {
        var assembly = GetExecutingOrEntryAssembly();
        return assembly?.GetName().Name;
    }
    public static string? GetAssemblyName([DisallowNull] this Assembly assembly)
    {
        return assembly?.GetName().Name;
    }
    public static string? GetAssemblyVersion()
    {
        var assembly = GetExecutingOrEntryAssembly();
        return Convert.ToString(assembly?.GetName().Version, CultureInfo.InvariantCulture);
    }
    public static string? GetAssemblyVersion([DisallowNull] this Assembly assembly)
    {
        return Convert.ToString(assembly?.GetName().Version, CultureInfo.InvariantCulture);
    }
    public static string? GetAssemblyFullName([DisallowNull] this Assembly assembly)
    {
        return assembly?.FullName;
    }
    public static string? GetAssemblyLocation([DisallowNull] this Assembly assembly)
    {
        return assembly?.Location;
    }
    public static string? GetAssemblyCulture([DisallowNull] this Assembly assembly)
    {
        return assembly?.GetName().CultureInfo?.Name;
    }
    public static string? GetAssemblyPublicKeyToken([DisallowNull] this Assembly assembly)
    {
        var publicKeyToken = assembly?.GetName().GetPublicKeyToken();
        return publicKeyToken != null ? BitConverter.ToString(publicKeyToken).Replace("-", string.Empty) : null;
    }
    public static IEnumerable<Attribute> GetAssemblyCustomAttributes([DisallowNull] this Assembly assembly)
    {
        return assembly?.GetCustomAttributes() ?? Enumerable.Empty<Attribute>();
    }
    public static bool IsAssemblyFullyTrusted([DisallowNull] this Assembly assembly)
    {
        return assembly?.IsFullyTrusted ?? false;
    }
    public static Type[] GetAssemblyTypes([DisallowNull] this Assembly assembly)
    {
        return assembly?.GetTypes() ?? Array.Empty<Type>();
    }
    public static MethodInfo? GetAssemblyEntryPoint([DisallowNull] this Assembly assembly)
    {
        return assembly?.EntryPoint;
    }
    public static Dictionary<string, string?> GetAssemblyMetadata([DisallowNull] this Assembly assembly)
    {
        if (assembly == null)
            return new Dictionary<string, string?>();

        var metadata = new Dictionary<string, string?>()
         {
             { "Name", assembly.GetName().Name },
             { "Version", Convert.ToString(assembly.GetName().Version, CultureInfo.InvariantCulture) },
             { "Culture", assembly.GetName().CultureInfo?.Name },
             { "PublicKeyToken", GetAssemblyPublicKeyToken(assembly) },
             { "Location", assembly.Location }
         };

        return metadata;
    }
    public static string[] GetAssemblyManifestResourceNames([DisallowNull] this Assembly assembly)
    {
        return assembly?.GetManifestResourceNames() ?? Array.Empty<string>();
    }

}
