using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace Reflector;

public static class ReflectionExtensions
{

    public static IEnumerable<Type> GetParentTypes(this Type type)
    {
        // is there any base type?
        if (type == null)
            yield break;

        // return all implemented or inherited interfaces
        foreach (var @interface in type.GetInterfaces())
            yield return @interface;

        // return all inherited types
        var currentBaseType = type.BaseType;
        while (currentBaseType != null)
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
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

    public static bool IsCustomStruct(this Type type)
    {
        var typeInfo = type?.GetTypeInfo();
        if (typeInfo == null)
            return false;

        var isStruct = !IsPrimitive(type!) && typeInfo.IsValueType && !typeInfo.IsEnum && type != typeof(Guid);
        if (!isStruct) return false;
        var ctor = typeInfo.GetConstructor(new[] { typeof(string) });
        return ctor != null;
    }
    public static bool IsPrimitive(this Type type)
    {
        var typeInfo = type?.GetTypeInfo();
        if (typeInfo == null)
            return false;

        return (typeInfo.IsPrimitive
            || new[] {
                     typeof(string)
                    ,typeof(decimal)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    ,typeof(TimeSpan)
                    ,typeof(Char)
                    ,typeof(String)
                    ,typeof(Int32)
                    ,typeof(Int64)
                    ,typeof(Byte)
                    ,typeof(Decimal)
                    ,typeof(Double)
                    ,typeof(Boolean)
                    ,typeof(SByte)
                    ,typeof(Single)
                    ,typeof(UInt16)
                    ,typeof(UInt32)
                    ,typeof(UInt64)
                    ,typeof(UIntPtr)
               }.Contains(type)
            || Convert.GetTypeCode(type) != TypeCode.Object);
    }

    public static object? InvokeStaticMethod(this Type type, string name, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            args);
    }

    public static object? GetStaticProperty(this Type type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            Array.Empty<object>());
    }

    public static object? SetStaticProperty(this Type type, string name, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { value });
    }

    public static object? InvokeInstancMethod(this Type type, string name, object target, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            args);
    }
    public static object? GetInstanceProperty(this Type type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            Array.Empty<object>());
    }
    public static object? SetInstanceProperty(this Type type, string name, object target, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target, new object[] { value });
    }

    public static object? GetInstanceField(this Type type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            Array.Empty<object>());
    }
    public static object? GetStaticField(this Type type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            Array.Empty<object>());
    }
    public static object? SetInstanceField(this Type type, string name, object target, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance,
            null,
            target, new object[] { value });
    }
    public static object? SetStaticField(this Type type, string name, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetField | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { value });
    }

    public static object? SetCustomStructInstanceProperty(this Type type, string name, object target, object value)
    {
        if (IsCustomStruct(type))
            return null;
        var propInfo = type.GetProperty(name, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
        if (propInfo == null) return null;

        propInfo.SetValue(target, value, null);
        return target;
    }

    public static Assembly GetExecutingOrEntryAssembly()
    {
        return Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
    }

    public static IEnumerable<string> GetNamesOfEnum(this Type type)
    {
        if (type == null)
            return Enumerable.Empty<string>();

        if (type.IsEnum)
            return Enum.GetNames(type);

        Type? u = Nullable.GetUnderlyingType(type);
        return (u != null && u.IsEnum) ? Enum.GetNames(u) : Enumerable.Empty<string>();

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

    public static bool IsEnum(this Type type) => type?.GetTypeInfo()?.BaseType == typeof(Enum);

    public static object? GetEnumPropertyValue(this PropertyInfo targetType, string value, bool ignoreCase = false) =>
    IsEnum(targetType.PropertyType) ? Enum.Parse(targetType.PropertyType, value, ignoreCase) : null;

    public static bool IsNullable(this PropertyInfo property) =>
    IsNullable(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    public static bool IsNullable(this FieldInfo field) =>
    IsNullable(field.FieldType, field.DeclaringType, field.CustomAttributes);

    public static bool IsNullable(this ParameterInfo parameter) =>
    IsNullable(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    public static bool IsNullable(this Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
    {
        if (memberType == null)
            return true;

        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullable = customAttributes?
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

    public static bool IsNullable(Type? type) => type is null || type.IsClass || Nullable.GetUnderlyingType(type) != null;
    public static Type? GetType(object obj) => obj?.GetType();

    public static ConstructorInfo[]? GetAllConstructors(this Type type)
    {
        return type?.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.ToArray();
    }

    public static ConstructorInfo[]? GetConstructors(this Type type)
    {
        return type?.GetConstructors()?.ToArray();
    }

    public static ParameterInfo[]? GetAllParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters();
    }
    public static ParameterInfo[]? GetRequiredParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters().Where(x => x.IsOptional == false).ToArray();
    }

    public static Array? CreateEmptyArray(this Type? type)
    {
        if (type == null)
            return null;

        return Array.CreateInstance(type, 0);
    }

    public static object? GetDefaultValue(this Type? type)
    {
        return type?.IsValueType == true ? Activator.CreateInstance(type) : null;
    }

    public static IEnumerable<Type> SafeGetInterfaces(this Type? type)
    {
        return type == null ? Enumerable.Empty<Type>() : type.GetTypeInfo().GetInterfaces();
    }

    public static object? CreateDefaultForImmutable(this Type? type)
    {
        if (type?.GetTypeInfo().IsGenericType == true && type?.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetTypeInfo().GetGenericArguments()[0].CreateEmptyArray();
        }
        return type?.GetDefaultValue();
    }

    public static bool IsDeepMutable(this Type? type)
    {
        if (type == null)
            return false;

        if (IsMutable(type?.GetTypeInfo()))
            return true;

        var inheritedTypes = type!.GetParentTypes().Where(x => x != typeof(object)).Select(i => i.GetTypeInfo());

        foreach (TypeInfo inheritedType in inheritedTypes)
        {
            if (IsMutable(inheritedType))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsMutable(this Type? type)
    {
        if (type == null)
            return false;

        return IsMutable(type.GetTypeInfo());
    }

    public static bool IsMutable(this TypeInfo? type)
    {
        if (type == null)
            return false;

        if (type == typeof(object))
            return true;

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Any(p => SetIsAllowed(p, checkNonPublicSetter: true)) ||
                type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(f => !f.IsInitOnly).Any();
    }

    public static FieldInfo[]? GetAllFields(this Type type)
    {
        return type?.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToArray();
    }

    public static PropertyInfo[]? GetAllProperties(this Type type)
    {
        return type?.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToArray();
    }

    public static MethodInfo[]? GetAllMethods(this Type type)
    {
        return type?.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToArray();
    }


    public static EventInfo[]? GetAllEvents(this Type type)
    {
        return type?.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToArray();
    }


    public static FieldInfo[]? GetPublicAndProtectedInstanceAndStaticFields(this Type type)
    {

        return type?.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.IsPublic || f.IsFamily).ToArray();
    }

    public static PropertyInfo[]? GetPublicAndProtectedInstanceAndStaticProperties(this Type type)
    {
        return type?.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(p => (p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsFamily))).ToArray();
    }

    public static MethodInfo[]? GetPublicAndProtectedInstanceAndStaticMethods(this Type type)
    {
        return type?.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName).ToArray();
    }


    public static EventInfo[]? GetPublicAndProtectedInstanceAndStaticEvents(this Type type)
    {
        return type?.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(e => e.AddMethod.IsPublic || e.AddMethod.IsFamily).ToArray();
    }

    public static FieldInfo[]? GetPublicAndProtectedInstanceFields(this Type type)
    {

        return type?.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.IsPublic || f.IsFamily).ToArray();
    }

    public static PropertyInfo[]? GetPublicAndProtectedInstanceProperties(this Type type)
    {
        return type?.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => (p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsFamily))).ToArray();
    }

    public static MethodInfo[]? GetPublicAndProtectedInstanceMethods(this Type type)
    {
        return type?.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName).ToArray();
    }


    public static EventInfo[]? GetPublicAndProtectedInstanceEvents(this Type type)
    {
        return type?.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(e => e.AddMethod.IsPublic || e.AddMethod.IsFamily).ToArray();
    }

    public static FieldInfo[]? GetPublicAndProtectedStaticFields(this Type type)
    {

        return type?.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.IsPublic || f.IsFamily).ToArray();
    }

    public static PropertyInfo[]? GetPublicAndProtectedStaticProperties(this Type type)
    {
        return type?.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(p => (p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsFamily))).ToArray();
    }

    public static MethodInfo[]? GetPublicAndProtectedStaticMethods(this Type type)
    {
        return type?.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName).ToArray();
    }

    public static EventInfo[]? GetPublicAndProtectedStaticEvents(this Type type)
    {
        return type?.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(e => e.AddMethod.IsPublic || e.AddMethod.IsFamily).ToArray();
    }

    public static FieldInfo[]? GetInternalOrProtectedInstanceAndStaticFields(this Type type)
    {

        return type?.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.IsFamily || f.IsFamilyOrAssembly || f.IsFamilyAndAssembly).ToArray();
    }

    public static PropertyInfo[]? GetInternalOrPProtectedInstanceAndStaticProperties(this Type type)
    {
        return type?.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(p => (p.GetMethod != null && (p.GetMethod.IsFamily || p.GetMethod.IsFamilyAndAssembly || p.GetMethod.IsFamilyOrAssembly))).ToArray();
    }

    public static MethodInfo[]? GetInternalOrPProtectedInstanceAndStaticMethods(this Type type)
    {
        return type?.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(m => (m.IsFamily || m.IsFamilyAndAssembly || m.IsFamilyOrAssembly) && !m.IsSpecialName).ToArray();
    }

    public static EventInfo[]? GetInternalOrPProtectedInstanceAndStaticEvents(this Type type)
    {
        return type?.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(e => e.AddMethod.IsFamily || e.AddMethod.IsFamilyAndAssembly || e.AddMethod.IsFamilyAndAssembly).ToArray();
    }

    public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
    {
        return (T)Attribute.GetCustomAttribute(member, typeof(T));
    }

    public static IEnumerable<MethodInfo>? GetMethodsWithName(this Type type, string methodName)
    {
        return type?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.Name == methodName);
    }

    public static IEnumerable<PropertyInfo>? GetPropertiesWithName(this Type type, string propertyName)
    {
        return type?.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(p => p.Name == propertyName);
    }

    public static IEnumerable<FieldInfo>? GetFieldsWithName(this Type type, string fieldName)
    {
        return type?.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(f => f.Name == fieldName);
    }

    public static IEnumerable<MemberInfo>? GetMembersWithName(this Type type, string name)
    {
        return type?.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.Name == name);
    }

    public static IEnumerable<MemberInfo>? GetEventsWithName(this Type type, string name)
    {
        return type?.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(e => e.Name == name);
    }


    public static IEnumerable<MethodInfo>? GetConstructorsWithParameters(this Type type, int parameterCount)
    {
        return type?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetParameters().Length == parameterCount);
    }

    public static IEnumerable<PropertyInfo>? GetPropertiesWithType(this Type type, Type propertyType)
    {
        return type?.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(p => p.PropertyType == propertyType);
    }

    public static IEnumerable<FieldInfo>? GetFieldsWithType(this Type type, Type fieldType)
    {
        return type?.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(f => f.FieldType == fieldType);
    }

    public static IEnumerable<MethodInfo>? GetInstanceConstructorsWithParameters(this Type type, int parameterCount)
    {
        return type?.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetParameters().Length == parameterCount);
    }

    public static IEnumerable<PropertyInfo>? GetGetInstancPropertiesWithType(this Type type, Type propertyType)
    {
        return type?.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == propertyType);
    }

    public static IEnumerable<FieldInfo>? GetGetInstancFieldsWithType(this Type type, Type fieldType)
    {
        return type?.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.FieldType == fieldType);
    }

}


