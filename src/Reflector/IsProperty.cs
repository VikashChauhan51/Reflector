using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace VReflector;

public static class IsProperty
{
    public static object? GetEnumPropertyValue(this PropertyInfo targetType, string value, bool ignoreCase = false) =>
   IsType.IsEnum(targetType.PropertyType) ? Enum.Parse(targetType.PropertyType, value, ignoreCase) : null;
    public static bool IsNullable(this PropertyInfo property) =>
    property.PropertyType.IsNullable(property.DeclaringType, property.CustomAttributes);
    public static bool IsIndexer(this PropertyInfo member)
    {
        return member.GetIndexParameters().Length != 0;
    }

    public static bool SetIsAllowed([DisallowNull]this PropertyInfo pInfo, bool checkNonPublicSetter = false, bool checkInitSetter = false)
    {
        var setMethod = pInfo.GetSetMethod(nonPublic: checkNonPublicSetter);
        if (setMethod == null)
            return false;

        // Get the modifiers applied to the return parameter.
        var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();
        // Init-only properties are marked with the IsExternalInit type.
        var IsExternalInit = setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        if (IsExternalInit && !checkInitSetter)
            return false;

        if (IsExternalInit && checkInitSetter)
            return true;

        return pInfo.CanWrite &&
               ((!checkNonPublicSetter && setMethod.IsPublic) ||
                (checkNonPublicSetter && (setMethod.IsPrivate ||
                setMethod.IsFamily || setMethod.IsPublic ||
                setMethod.IsAbstract)));

    }

    public static AccessModifier? GetPropertyAccessModifier([DisallowNull] this PropertyInfo property)
    {
        var method = property?.GetMethod ?? property?.SetMethod;
        return method?.GetMethodAccessModifier();
    }
    public static bool IsVirtual([DisallowNull] this PropertyInfo property)
    {
        MethodInfo? methodInfo = property?.GetGetMethod(nonPublic: true) ?? property?.GetSetMethod(nonPublic: true);
        return !methodInfo?.IsNonVirtual() ?? false;
    }

    public static bool IsStatic([DisallowNull] this PropertyInfo property)
    {
        MethodInfo? methodInfo = property?.GetGetMethod(nonPublic: true) ?? property?.GetSetMethod(nonPublic: true);
        return methodInfo?.IsStatic ?? false;
    }

    public static bool IsAbstract([DisallowNull] this PropertyInfo property)
    {
        MethodInfo? methodInfo = property?.GetGetMethod(nonPublic: true) ?? property?.GetSetMethod(nonPublic: true);
        return methodInfo?.IsAbstract ?? false;
    }
    public static string GetPropertyModifiers([DisallowNull] this PropertyInfo property)
    {
        var method = property?.GetMethod ?? property?.SetMethod;
        return method?.GetMethodModifiers() ?? string.Empty;
    }
}
