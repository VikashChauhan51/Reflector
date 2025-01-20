using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace VReflector;

public static class IsObject
{
    private const string ImplicitCastMethodName = "op_Implicit";
    private const string ExplicitCastMethodName = "op_Explicit";

    public static T? CastTo<T>([DisallowNull] this object obj) where T : class
    {
        return obj as T;
    }
    public static T ToOrDefault<T>([DisallowNull] this object obj, T defaultValue = default)
    {
        return obj is T value ? value : defaultValue;
    }
    public static T CastIt<T>([DisallowNull] object obj)
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
    public static bool TryCastTo<T>([DisallowNull] object obj, out T? result) where T : class
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
    public static bool IsCastableTo<T>([DisallowNull] object obj)
    {
        return obj is T || CanCast<T>(obj.GetType());
    }
    public static Type? GetType(this object? obj) => obj?.GetType();
    public static bool IsSame(this object? actual, object? expected)
    {
        if (actual == null || expected == null)
            return false;

        return ReferenceEquals(actual, expected);
    }
    public static bool IsEqual(this object? actual, object? expected)
    {
        if (actual == null || expected == null)
            return false;

        Type actualType = actual.GetType();
        Type expectedType = expected.GetType();

        if (actualType != expectedType)
            return false;

        if (actualType.IsValueType)
            return actual.Equals(expected);

        if (IsType.Record(actualType))
            return actual.Equals(expected);

        if (actual is IComparable comparable)
        {
            return comparable.CompareTo(expected) == 0;
        }

        return ReferenceEquals(actual, expected);
    }
    public static string GetStackTrace([DisallowNull] this object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            return stackTrace.ToString();
        }
        return string.Empty;
    }
    public static bool IsStackTraceType([DisallowNull] this object obj)
    {
        return obj is StackTrace;
    }
    public static string GetCallerMethod([DisallowNull] this object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            var frame = stackTrace.GetFrame(1); // Caller frame
            return frame?.GetMethod()?.Name ?? string.Empty;
        }
        return string.Empty;
    }
    public static bool Is([NotNullWhen(true)] this object? obj)
    {
        return obj is double or float or byte or sbyte or decimal or int or uint or long or ulong or short or ushort or Half;
    }
    public static bool IsNaN(this object? value)
    {
        if (value is double d)
        {
            return double.IsNaN(d);
        }
        if (value is float f)
        {
            return float.IsNaN(f);
        }
        return false;
    }
    public static bool CanConvert([DisallowNull] this object source, object target, Type sourceType, Type targetType)
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
    public static bool CanCast<T>([DisallowNull] this object obj)
    {
        return CanCast<T>(obj.GetType());
    }
    public static bool TryGetEnumerable([DisallowNull]  this object obj, [NotNullWhen(true)] out IEnumerable? enumerable)
    {
        enumerable = obj as IEnumerable;

        if (enumerable == null && obj != null)
        {
            var objectType = obj.GetType();
            if (IsType.IsMemory(objectType, out var genericParameterType))
            {
                var readOnlyMemory = ToReadOnlyMemory(obj, objectType, genericParameterType);

                if (readOnlyMemory != null)
                {
                    enumerable = ToEnumerable(readOnlyMemory, genericParameterType);
                }
            }
            else if (IsType.IsReadOnlyMemory(objectType, out genericParameterType))
            {
                enumerable = ToEnumerable(obj, genericParameterType);
            }
        }

        return enumerable != null;
    }
    public static IEnumerable ToEnumerable([DisallowNull] this object readOnlyMemory, Type elementType)
    {
        return (IEnumerable)Type.GetType("System.Runtime.InteropServices.MemoryMarshal, System.Memory")
            ?.GetMethod("ToEnumerable", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            ?.MakeGenericMethod(elementType)
            .Invoke(null, new[] { readOnlyMemory })!;
    }
    public static object ToReadOnlyMemory(object obj, Type objectType, Type genericParameterType)
    {
        return objectType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .SingleOrDefault(method =>
                method.Name == "op_Implicit"
                && method.GetParameters()[0].ParameterType == objectType
                && IsType.IsReadOnlyMemory(method.ReturnType, out var returnElementType)
                && returnElementType == genericParameterType)
            ?.Invoke(null, new[] { obj })!;
    }
    private static bool CanImplicit<T>(object obj)
    {
        var baseType = obj.GetType();
        return CanImplicit<T>(baseType);
    }
    private static bool CanExplicit<T>(object obj)
    {
        var baseType = obj.GetType();
        return CanExplicit<T>(baseType);
    }
    private static T Implicit<T>(object obj)
    {
        return CastTo<T>(obj, ImplicitCastMethodName);
    }
    private static T Explicit<T>(object obj)
    {
        return CastTo<T>(obj, ExplicitCastMethodName);
    }
    private static T CastTo<T>(object obj, string castMethodName)
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
