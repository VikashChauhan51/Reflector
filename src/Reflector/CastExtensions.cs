using System.Globalization;
using System.Reflection;

namespace Reflector;


public static class CastExtensions
{
    private const string ImplicitCastMethodName = "op_Implicit";
    private const string ExplicitCastMethodName = "op_Explicit";

    public static T? SafeCast<T>(this object obj) where T : class
    {
        return obj as T;
    }

    public static T CastOrDefault<T>(this object obj, T defaultValue = default)
    {
        try
        {
            return (T)obj;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static bool TryCast<T>(this object obj, out T? result) where T : class
    {
        if (obj == null)
        {
            result = default;
            return false;
        }

        if (obj is T castedObj)
        {
            result = castedObj;
            return true;
        }

        if (obj.CanExplicitCast<T>())
        {
            result = obj.ExplicitCast<T>();
            return true;
        }

        result = default;
        return false;
    }


    public static Func<T, T, bool> GetComparer<T>()
    {
        if (typeof(T).IsValueType)
        {
            // Avoid causing any boxing for value types
            return (actual, expected) => EqualityComparer<T>.Default.Equals(actual, expected);
        }

        if (typeof(T) != typeof(object))
        {
            // CompareNumerics is only relevant for numerics boxed in an object.
            return (actual, expected) => actual is null
                ? expected is null
                : expected is not null && EqualityComparer<T>.Default.Equals(actual, expected);
        }

        return (actual, expected) => actual is null
            ? expected is null
            : expected is not null
            && (EqualityComparer<T>.Default.Equals(actual, expected) || CompareNumerics(actual, expected));
    }

    public static bool IsCastableTo<T>(this object obj)
    {
        return obj is T || obj.GetType().CanCast<T>();
    }

    public static bool CheckCastCompatibility(this Type sourceType, Type targetType)
    {
        return sourceType.IsAssignableFrom(targetType) || targetType.IsAssignableFrom(sourceType);
    }

    public static bool CanConvert(object source, object target, Type sourceType, Type targetType)
    {
        try
        {
            var converted = source.ConvertTo(targetType);

            return source.Equals(converted.ConvertTo(sourceType))
                && converted.Equals(target);
        }
        catch
        {
            // Ignored
            return false;
        }
    }
    public static bool CanCast<T>(this Type baseType)
    {
        return baseType.CanImplicitCast<T>() || baseType.CanExplicitCast<T>();
    }

    public static bool CanCast<T>(this object obj)
    {
        var objType = obj.GetType();
        return objType.CanCast<T>();
    }

    public static T Cast<T>(this object obj)
    {
        try
        {
            return (T)obj;
        }
        catch (InvalidCastException)
        {
            if (obj.CanImplicitCast<T>())
                return obj.ImplicitCast<T>();
            if (obj.CanExplicitCast<T>())
                return obj.ExplicitCast<T>();
            else
                throw;
        }
    }

    private static bool CanImplicitCast<T>(this Type baseType)
    {
        return baseType.CanCast<T>(ImplicitCastMethodName);
    }

    private static bool CanImplicitCast<T>(this object obj)
    {
        var baseType = obj.GetType();
        return baseType.CanImplicitCast<T>();
    }

    private static bool CanExplicitCast<T>(this Type baseType)
    {
        return baseType.CanCast<T>(ExplicitCastMethodName);
    }

    private static bool CanExplicitCast<T>(this object obj)
    {
        var baseType = obj.GetType();
        return baseType.CanExplicitCast<T>();
    }

    private static bool CanCast<T>(this Type baseType, string castMethodName)
    {
        var targetType = typeof(T);
        return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(mi => mi.Name == castMethodName && mi.ReturnType == targetType)
            .Any(mi =>
            {
                ParameterInfo? pi = mi.GetParameters().FirstOrDefault();
                return pi != null && pi.ParameterType == baseType;
            });
    }

    private static T ImplicitCast<T>(this object obj)
    {
        return obj.Cast<T>(ImplicitCastMethodName);
    }

    private static T ExplicitCast<T>(this object obj)
    {
        return obj.Cast<T>(ExplicitCastMethodName);
    }

    private static T Cast<T>(this object obj, string castMethodName)
    {
        var objType = obj.GetType();
        MethodInfo conversionMethod = objType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(mi => mi.Name == castMethodName && mi.ReturnType == typeof(T))
            .SingleOrDefault(mi =>
            {
                ParameterInfo? pi = mi.GetParameters().FirstOrDefault();
                return pi != null && pi.ParameterType == objType;
            });
        if (conversionMethod != null)
            return (T)conversionMethod.Invoke(null, new[] { obj })!;
        else
            throw new InvalidCastException($"No method to cast {objType.FullName} to {typeof(T).FullName}");
    }

    private static bool CompareNumerics(object actual, object expected)
    {
        Type expectedType = expected.GetType();
        Type actualType = actual.GetType();

        return actualType != expectedType
            && actual.IsNumericType()
            && expected.IsNumericType()
            && CanConvert(actual, expected, actualType, expectedType)
            && CanConvert(expected, actual, expectedType, actualType);
    }


    private static object ConvertTo(this object source, Type targetType)
    {
        return Convert.ChangeType(source, targetType, CultureInfo.InvariantCulture);
    }
}

