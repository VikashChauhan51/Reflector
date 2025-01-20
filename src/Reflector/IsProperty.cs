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

    public static bool SetIsAllowed(this PropertyInfo pInfo, bool checkNonPublicSetter = false, bool checkInitSetter = false)
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

    public static string GetPropertyAccessModifier(this PropertyInfo property)
    {
        var method = property?.GetMethod ?? property?.SetMethod;
        return method?.GetMethodAccessModifier() ?? string.Empty;
    }
    public static string GetPropertyModifiers(this PropertyInfo property)
    {
        var method = property?.GetMethod ?? property?.SetMethod;
        return method?.GetMethodModifiers() ?? string.Empty;
    }
}
