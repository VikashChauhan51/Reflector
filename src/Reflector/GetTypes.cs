using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace VReflector;

public static class GetTypes
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
    public static string? GetAssemblyVersion()
    {
        var assembly = GetExecutingOrEntryAssembly();
        return Convert.ToString(assembly?.GetName().Version, CultureInfo.InvariantCulture);
    }
    public static bool TryGetEnumerable(object obj, [NotNullWhen(true)] out IEnumerable? enumerable)
    {
        enumerable = obj as IEnumerable;

        if (enumerable == null && obj != null)
        {
            var objectType = obj.GetType();
            if (IsType.Memory(objectType, out var genericParameterType))
            {
                var readOnlyMemory = ToReadOnlyMemory(obj, objectType, genericParameterType);

                if (readOnlyMemory != null)
                {
                    enumerable = ToEnumerable(readOnlyMemory, genericParameterType);
                }
            }
            else if (IsType.ReadOnlyMemory(objectType, out genericParameterType))
            {
                enumerable = ToEnumerable(obj, genericParameterType);
            }
        }

        return enumerable != null;
    }
    public static IEnumerable ToEnumerable(object readOnlyMemory, Type elementType)
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
                && IsType.ReadOnlyMemory(method.ReturnType, out var returnElementType)
                && returnElementType == genericParameterType)
            ?.Invoke(null, new[] { obj })!;
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
            && (EqualityComparer<T>.Default.Equals(actual, expected) || IsNumerics.Compare(actual, expected) == 0);
    }

}
