using System.Reflection;

namespace VReflector;

public static class IsConstructor
{
    public static ParameterInfo[]? GetAllParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters();
    }
    public static ParameterInfo[]? GetRequiredParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters().Where(x => x.IsOptional == false).ToArray();
    }
    public static string GetConstructorAccessModifier(this ConstructorInfo constructor)
    {
        return constructor switch
        {
            _ when constructor.IsPrivate => "private",
            _ when constructor.IsPublic => "public",
            _ when constructor.IsFamilyOrAssembly => "protected internal",
            _ when constructor.IsFamily => "protected",
            _ when constructor.IsAssembly => "internal",
            _ => string.Empty
        };
    }
    public static string GetConstructorModifiers(this ConstructorInfo constructor)
    {
        return constructor switch
        {
            _ when constructor.IsStatic => "static",
            _ => string.Empty
        };
    }
}
