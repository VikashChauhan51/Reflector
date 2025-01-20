using System.Reflection;

namespace VReflector;

public static class IsMethod
{
    public static string GetMethodAccessModifier(this MethodInfo method)
    {
        return method switch
        {
            _ when method.IsPrivate => "private",
            _ when method.IsPublic => "public",
            _ when method.IsFamilyOrAssembly => "protected internal",
            _ when method.IsFamily => "protected",
            _ when method.IsAssembly => "internal",
            _ => string.Empty
        };
    }
    public static string GetMethodModifiers(this MethodInfo method)
    {
        return method switch
        {
            _ when method.IsStatic => "static",
            _ when method.IsFinal => "sealed override",
            _ when method.IsAbstract => "abstract",
            _ when method.IsVirtual => "virtual",
            _ => string.Empty
        };
    }
}
