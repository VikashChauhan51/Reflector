using System.Globalization;
using System.Reflection;
using VReflector;

namespace Reflector;


public static class Cast
{
    private const string ImplicitCastMethodName = "op_Implicit";
    private const string ExplicitCastMethodName = "op_Explicit";

    public static T? To<T>(object obj) where T : class
    {
        return obj as T;
    }
    public static T ToOrDefault<T>(object obj, T defaultValue = default)
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
    public static bool TryTo<T>(object obj, out T? result) where T : class
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

        if (CanExplicit<T>(obj))
        {
            result = Explicit<T>(obj);
            return true;
        }

        result = default;
        return false;
    }
    public static bool IsCastableTo<T>(object obj)
    {
        return obj is T || Can<T>(obj.GetType());
    }
    public static bool CheckCompatibility(Type sourceType, Type targetType)
    {
        return IsType.AssignableTo(targetType, sourceType) || IsType.AssignableTo(sourceType, targetType);
    }
    public static bool CanConvert(object source, object target, Type sourceType, Type targetType)
    {
        try
        {
            var converted = ConvertTo(source, targetType);

            return source.Equals(ConvertTo(converted, sourceType))
                && converted.Equals(target);
        }
        catch
        {
            // Ignored
            return false;
        }
    }
    public static bool Can<T>(Type baseType)
    {
        if (baseType is null)
        {
            return false;
        }
        return CanExplicit<T>(baseType) || CanExplicit<T>(baseType);
    }
    public static bool Can<T>(object obj)
    {
        return Can<T>(obj.GetType());
    }
    public static T It<T>(object obj)
    {
        try
        {
            return (T)obj;
        }
        catch (InvalidCastException)
        {
            if (CanImplicit<T>(obj))
                return Implicit<T>(obj);
            if (CanExplicit<T>(obj))
                return Explicit<T>(obj);
            else
                throw;
        }
    }
    private static bool CanImplicit<T>(Type baseType)
    {
        return Can<T>(baseType, ImplicitCastMethodName);
    }
    private static bool CanImplicit<T>(object obj)
    {
        var baseType = obj.GetType();
        return CanImplicit<T>(baseType);
    }
    private static bool CanExplicit<T>(Type baseType)
    {
        return Can<T>(baseType, ExplicitCastMethodName);
    }
    private static bool CanExplicit<T>(object obj)
    {
        var baseType = obj.GetType();
        return CanExplicit<T>(baseType);
    }
    private static bool Can<T>(Type baseType, string castMethodName)
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
    private static T Implicit<T>(object obj)
    {
        return To<T>(obj, ImplicitCastMethodName);
    }
    private static T Explicit<T>(object obj)
    {
        return To<T>(obj, ExplicitCastMethodName);
    }
    private static T To<T>(object obj, string castMethodName)
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
    private static object ConvertTo(object source, Type targetType)
    {
        return Convert.ChangeType(source, targetType, CultureInfo.InvariantCulture);
    }
}

