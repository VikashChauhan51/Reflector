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
    public static string? GetAssemblyFullName(Assembly assembly)
    {
        return assembly?.FullName;
    }
    public static string? GetAssemblyLocation(Assembly assembly)
    {
        return assembly?.Location;
    }
    public static string? GetAssemblyCulture(Assembly assembly)
    {
        return assembly?.GetName().CultureInfo?.Name;
    }
    public static string? GetAssemblyPublicKeyToken(Assembly assembly)
    {
        var publicKeyToken = assembly?.GetName().GetPublicKeyToken();
        return publicKeyToken != null ? BitConverter.ToString(publicKeyToken).Replace("-", string.Empty) : null;
    }
    public static IEnumerable<Attribute> GetAssemblyCustomAttributes(Assembly assembly)
    {
        return assembly?.GetCustomAttributes() ?? Enumerable.Empty<Attribute>();
    }
    public static bool IsAssemblyFullyTrusted(Assembly assembly)
    {
        return assembly?.IsFullyTrusted ?? false;
    }
    public static Type[] GetAssemblyTypes(Assembly assembly)
    {
        return assembly?.GetTypes() ?? Array.Empty<Type>();
    }
    public static MethodInfo? GetAssemblyEntryPoint(Assembly assembly)
    {
        return assembly?.EntryPoint;
    }
    public static Dictionary<string, string?> GetAssemblyMetadata(Assembly assembly)
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
    public static string[] GetAssemblyManifestResourceNames(Assembly assembly)
    {
        return assembly?.GetManifestResourceNames() ?? Array.Empty<string>();
    }

}
