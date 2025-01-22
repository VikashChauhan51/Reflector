using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VReflector;

public static class IsMethod
{
    public static AccessModifier GetMethodAccessModifier(this MethodBase method)
    {
        return method switch
        {
            _ when method.IsPrivate => AccessModifier.Private,
            _ when method.IsPublic => AccessModifier.Public,
            _ when method.IsFamilyOrAssembly => AccessModifier.ProtectedInternal,
            _ when method.IsFamily => AccessModifier.Protected,
            _ when method.IsAssembly => AccessModifier.Internal,
            _ => AccessModifier.Internal
        };
    }
    public static AccessModifier GetMethodAccessModifier(this MethodInfo method)
    {
        return method switch
        {
            _ when method.IsPrivate => AccessModifier.Private,
            _ when method.IsPublic => AccessModifier.Public,
            _ when method.IsFamilyOrAssembly => AccessModifier.ProtectedInternal,
            _ when method.IsFamily => AccessModifier.Protected,
            _ when method.IsAssembly => AccessModifier.Internal,
            _ => AccessModifier.Internal
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
    public static bool IsNonVirtual(this MethodInfo method)
    {
        return !method.IsVirtual || method.IsFinal;
    }
    public static bool IsVirtualOrAbstract(this MethodInfo method)
    {
        return method.IsVirtual && !method.IsFinal || method.IsAbstract;
    }
    public static bool IsAsync(this MethodInfo methodInfo)
    {
        return methodInfo.IsDecoratedWith<AsyncStateMachineAttribute>();
    }
}
