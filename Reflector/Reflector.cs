using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

//namespace
namespace Reflector;

public static class Reflector
{

    public static IEnumerable<Type> GetParentTypes(Type type)
    {
        // is there any base type?
        if (type == null)
        {
            yield break;
        }

        // return all implemented or inherited interfaces
        foreach (var @interface in type.GetInterfaces())
        {
            yield return @interface;
        }

        // return all inherited types
        var currentBaseType = type.BaseType;
        while (currentBaseType != null)
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }

    public static bool IsImmutable(Type type, bool checkNonPublicSetter = false, int depth = 10)
    {
        static bool Immutable(Type type) => type == typeof(string) || type.IsValueType;
        IEnumerable<FieldInfo> GetFields(Type type) => type.GetFields()
            .Where(fInfo => !fInfo.IsStatic)
            .Where(fInfo => fInfo.FieldType != type);
        IEnumerable<PropertyInfo> GetProperties(Type type) => type.GetProperties()
                       .Where(pInfo => pInfo.GetMethod != null && !pInfo.GetMethod.IsStatic)
                       .Where(pInfo => pInfo.PropertyType != type);

        if (Immutable(type))
            return true;

        else
        {
            var fields = GetFields(type);

            if (fields.Any(fInfo => !fInfo.IsInitOnly))
                return false;
            var properties = GetProperties(type);

            if (!properties.Any() && !fields.Any())
                return true;

            return (!fields.Any() || fields
                .All(fInfo => fInfo.IsInitOnly && (depth <= 1 || IsImmutable(fInfo.FieldType, checkNonPublicSetter, depth - 1)))) &&
                (!properties.Any() ||
                     properties
                       .All(pInfo => !SetIsAllowed(pInfo, checkNonPublicSetter: checkNonPublicSetter) && (depth <= 1 || IsImmutable(pInfo.PropertyType, checkNonPublicSetter, depth - 1))));
        }

    }

    public static bool SetIsAllowed(PropertyInfo pInfo, bool checkNonPublicSetter)
    {
        var setMethod = pInfo.GetSetMethod(nonPublic: checkNonPublicSetter);
        if (setMethod == null)
            return false;
        //check property is init only

        // Get the modifiers applied to the return parameter.
        var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

        // Init-only properties are marked with the IsExternalInit type.
        var IsExternalInit = setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        if (IsExternalInit)
            return false;

        return pInfo.CanWrite &&
               ((!checkNonPublicSetter && setMethod.IsPublic) ||
                (checkNonPublicSetter && (setMethod.IsPrivate ||
                setMethod.IsFamily || setMethod.IsPublic || setMethod.IsAbstract)));

    }

    public static bool IsCustomStruct(Type type)
    {
        var isStruct = type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsPrimitive && !type.GetTypeInfo().IsEnum && type != typeof(Guid);
        if (!isStruct) return false;
        var ctor = type.GetTypeInfo().GetConstructor(new[] { typeof(string) });
        return ctor != null;
    }
    public static bool IsPrimitive(Type type)
    {
        return
               (type.GetTypeInfo().IsValueType && type != typeof(Guid))
            || type.GetTypeInfo().IsPrimitive
            || new[] {
                     typeof(string)
                    ,typeof(decimal)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    ,typeof(TimeSpan)
               }.Contains(type)
            || Convert.GetTypeCode(type) != TypeCode.Object;
    }

    public static object? InvokeStaticMethod(Type type, string name, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            args);
    }

    public static object? GetStaticProperty(Type type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { });
    }

    public static object? SetStaticProperty(Type type, string name, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { value });
    }

    public static object? InvokeInstancMethod(Type type, string name, object target, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            args);
    }
    public static object? GetInstanceProperty(Type type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            new object[] { });
    }
    public static object? SetInstanceProperty(Type type, string name, object target, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target, new object[] { value });
    }

    public static Assembly GetExecutingOrEntryAssembly()
    {
        return Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
    }

    public static IEnumerable<string> GetNamesOfEnum(Type t)
    {
        if (t.IsEnum)
            return Enum.GetNames(t);
        Type? u = Nullable.GetUnderlyingType(t);
        if (u != null && u.IsEnum)
            return Enum.GetNames(u);
        return Enumerable.Empty<string>();
    }

    public static string? GetAssemblyName()
    {
        var assembly = GetExecutingOrEntryAssembly();
        return assembly.GetName().Name;
    }

    public static string? GetAssemblyVersion()
    {
        var assembly = GetExecutingOrEntryAssembly();
        return Convert.ToString(assembly?.GetName().Version, CultureInfo.InvariantCulture);
    }

    public static bool IsEnum(Type type)
    {
        return type.GetTypeInfo().BaseType == typeof(Enum);
    }
    public static object? GetEnumPropertyValue(PropertyInfo targetType, string value, bool ignoreCase = false)
    {
        return IsEnum(targetType.PropertyType) ? Enum.Parse(targetType.PropertyType, value, ignoreCase) : null;
    }

    public static bool IsNullable(PropertyInfo property) =>
    IsNullable(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    public static bool IsNullable(FieldInfo field) =>
        IsNullable(field.FieldType, field.DeclaringType, field.CustomAttributes);

    public static bool IsNullable(ParameterInfo parameter) =>
        IsNullable(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    public static bool IsNullable(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
    {
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
        if (nullable != null && nullable.ConstructorArguments.Count == 1)
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return (byte)args[0].Value! == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte)attributeArgument.Value! == 2;
            }
        }

        for (var type = declaringType; type != null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }
        return false;
    }
}
       
    
