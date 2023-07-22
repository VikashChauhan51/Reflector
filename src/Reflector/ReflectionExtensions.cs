using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Reflector;

public static class ReflectionExtensions
{

    #region Constants
    const string Reference_Type = "class";
    const string ValueType_Type = "struct";
    const string Default_Constructor = "new()";
    #endregion Constants

    #region Public

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
                    ,typeof(Guid)
                    ,typeof(DateOnly)
                    ,typeof(TimeOnly)
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

    public static bool IsNullable(this Type? type) => type is null || type.IsClass || Nullable.GetUnderlyingType(type) != null;
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

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Any(p => SetIsAllowed(p, checkNonPublicSetter: false)) ||
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

    public static bool IsRecordClass(this Type type)
    {
        return type != null && type.IsClass &&
            type.GetMethod("ToString").GetCustomAttributes(false).Any(a => a.GetType() == typeof(CompilerGeneratedAttribute)) &&
            type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Any(property => property.Name.Contains("EqualityContract"));
    }

    public static bool IsRecordStruct(this Type type)
    {
        return type != null && type.IsStruct() && type.GetMethod("ToString").GetCustomAttributes(false).Any(a => a.GetType() == typeof(CompilerGeneratedAttribute));
    }

    public static bool IsRecord(this Type type)
    {
        return type != null && (type.IsRecordClass() || type.IsRecordStruct());
    }
    public static bool IsStruct(this Type type)
    {
        return type != null && type.IsValueType && !type.IsPrimitive && !type.IsEnum;
    }
    public static IEnumerable<Type> GetParentTypesExceptDefault(this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        return type.GetParentTypes().Where(p => p != typeof(object) || p != typeof(ValueType));
    }
    public static IEnumerable<Type> GetDirectImplementedInterfaces(this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        Type[] allInterfaces = type.GetInterfaces();
        Type[] inheritedInterfaces = type.BaseType?.GetInterfaces() ?? Array.Empty<Type>();
        return allInterfaces.Except(inheritedInterfaces).ToList();

    }
    public static bool IsStatic(this Type type)
    {
        return type != null && type.IsAbstract && type.IsSealed;
    }
    public static bool IsAnonymous(this Type type)
    {
        return type != null &&
            type.GetCustomAttributes(false).Any(a => a.GetType() == typeof(CompilerGeneratedAttribute)) &&
            type.Name.StartsWith("<>");
    }
    public static ConstructorInfo[]? GetPublicAndProtectedInstanceConstructors(this Type type)
    {
        return type?.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Where(c => c.IsPublic || c.IsFamily).ToArray();
    }

    public static bool IsReadonlyStruct(this Type type)
    {
        return type != null && type.IsStruct() && type.GetCustomAttributes().Any(a => a.GetType() == typeof(IsReadOnlyAttribute));
    }

    public static GenericParameterAttributes GetConstraints(this Type type)
    {
        if (type == null || !type.IsGenericParameter)
        {
            return GenericParameterAttributes.None;
        }

        return type.GenericParameterAttributes;
    }

    public static Type[] GetConstraintTypes(this Type type)
    {
        if (type == null || !type.IsGenericParameter)
        {
            return Array.Empty<Type>();
        }

        return type.GetGenericParameterConstraints();
    }
    public static bool IsDerivedFrom(this Type childType, Type parentType)
    {
        return childType != null && parentType != null && childType.IsSubclassOf(parentType);
    }

    public static bool IsClosedTypeOf(this Type type, Type genericTypeDefinition)
    {
        return type != null && genericTypeDefinition != null && type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
    }

    public static bool IsEnumerable(this Type type)
    {
        return type.IsArray || (type != typeof(string) && IsAssignableTo(type, typeof(System.Collections.IEnumerable)));
    }

    public static bool IsAssignableTo(this Type assignableType, Type type)
    {
        return type?.IsAssignableFrom(assignableType) ?? false;
    }
    public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
    {
        return type != null && Attribute.IsDefined(type, typeof(TAttribute), inherit: false);
    }

    public static bool CanBeNull(this Type type)
    {
        if (type == null) return true;

        if (!type.IsGenericParameter)
        {
            return type.IsClass || type.IsInterface || type.IsNullable();
        }

        GenericParameterAttributes constraints = type.GetConstraints();
        if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
        {
            return true;
        }

        if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
        {
            return false;
        }

        return type.GetConstraintTypes().Any((Type t) => t.CanBeNull());
    }

    public static bool IsSafePublic(this Type type)
    {
        return type != null && type.IsPublic;
    }

    public static bool IsSafeClass(this Type type)
    {
        return type != null && type.IsClass;
    }

    public static bool IsSafeInterface(this Type type)
    {
        return type != null && type.IsInterface;
    }

    public static bool IsSafeAbstract(this Type type)
    {
        return type != null && type.IsAbstract;
    }

    public static bool IsSafeSealed(this Type type)
    {
        return type != null && type.IsSealed;
    }

    public static bool IsSafeEnum(this Type type)
    {
        return type != null && type.IsEnum;
    }

    public static bool IsSafeValueType(this Type type)
    {
        return type != null && type.IsValueType;
    }

    public static bool IsSafeGenericType(this Type type)
    {
        return type != null && type.IsGenericType;
    }

    public static bool IsReadonly(this FieldInfo field)
    {
        return field != null && field.IsInitOnly;
    }
    public static bool IsConstant(this FieldInfo field)
    {
        return field != null && field.IsLiteral;
    }

    public static string GetAccessModifier(this Type type)
    {
        if (type == null ||
            type.IsArray ||
            type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(object) ||
            type == typeof(ValueType) ||
            type.IsPrimitive())
        {
            return string.Empty;
        }
        return type switch
        {
            _ when type.IsPublic => "public",
            _ when type.IsNestedPublic => "public",
            _ when type.IsNestedPrivate => "private",
            _ when type.IsNestedFamily => "protected",
            _ when type.IsNestedAssembly => "internal",
            _ when type.IsNestedFamANDAssem => "private protected",
            _ when type.IsNestedFamORAssem => "protected internal",
            _ when type.IsNotPublic => "internal",
            _ => string.Empty
        };
    }


    public static string GetTypeModifiers(this Type type)
    {
        if (type == null ||
            type.IsArray ||
            type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(object) ||
            type == typeof(ValueType) ||
            type.IsPrimitive())
        {
            return string.Empty;
        }

        return type switch
        {
            _ when type.IsArray && type.IsInterface => "interface ",
            _ when type.IsAbstract && type.IsRecordClass() => "abstract record ",
            _ when type.IsSealed && type.IsRecordClass() => "sealed record ",
            _ when type.IsAbstract && type.IsSealed => "static class ",
            _ when type.IsAbstract && !type.IsSealed && !type.IsRecordClass() => "abstract class ",
            _ when type.IsSealed && type.IsClass && !type.IsRecordClass() => "sealed class ",
            _ when type.IsEnum => "enum ",
            _ when type.IsReadonlyStruct() && type.IsRecordStruct() => "readonly record struct ",
            _ when type.IsReadonlyStruct() && !type.IsRecordStruct() => "readonly struct ",
            _ when type.IsRecordStruct() => "record struct ",
            _ when type.IsStruct() => "struct ",
            _ when type.IsRecordClass() => "record ",
            _ when type.IsClass => "class ",
            _ => string.Empty
        };
    }

    public static string GetMethodAccessModifier(this MethodInfo method)
    {
        return method switch
        {
            _ when method.IsPrivate => "private",
            _ when method.IsPublic => "public",
            _ when method.IsFamilyOrAssembly => "protected internal",
            _ when method.IsFamily => "protected",
            _ when method.IsAssembly => "internal",
            _ => string.Empty
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

    public static string GetFieldAccessModifier(this FieldInfo field)
    {
        return field switch
        {
            _ when field.IsPrivate => "private",
            _ when field.IsPublic => "public",
            _ when field.IsFamilyOrAssembly => "protected internal",
            _ when field.IsFamily => "protected",
            _ when field.IsAssembly => "internal",
            _ => string.Empty
        };
    }

    public static string GetFieldModifiers(this FieldInfo field)
    {
        return field switch
        {
            _ when field.IsInitOnly && field.IsStatic => "static readonly",
            _ when field.IsInitOnly => "readonly",
            _ when field.IsLiteral => "const",
            _ when field.IsStatic => "static",
            _ => string.Empty
        };
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

    public static string GetEventAccessModifier(this EventInfo @event)
    {
        var method = @event?.GetAddMethod() ?? @event?.GetRemoveMethod(true);
        return method?.GetMethodAccessModifier() ?? string.Empty;
    }

    public static string GetEventModifiers(this EventInfo @event)
    {
        var method = @event?.GetAddMethod() ?? @event?.GetRemoveMethod(true);
        return method?.GetMethodModifiers() ?? string.Empty;
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
    public static Type GetType<T>() => typeof(T);
    public static string GetName<T>() => GetName(nameof(T));
    public static string GetName(this Type type) => GetName(type?.Name);
    public static string GetName(this ConstructorInfo constructor) => GetName(constructor?.Name);
    public static string GetName(this MethodInfo method) => GetName(method?.Name);
    public static string GetName(this PropertyInfo property) => GetName(property?.Name);
    public static string GetName(this FieldInfo field) => GetName(field?.Name);
    public static string GetName(this EventInfo @event) => GetName(@event?.EventHandlerType?.Name);
    public static string AsString(this Type classOrEnumOrStructOrInterface, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {

        //check type is not supported
        if (classOrEnumOrStructOrInterface is null ||
            classOrEnumOrStructOrInterface.IsPrimitive() ||
            !(classOrEnumOrStructOrInterface.IsClass ||
            classOrEnumOrStructOrInterface.IsEnum ||
            classOrEnumOrStructOrInterface.IsInterface ||
            classOrEnumOrStructOrInterface.IsValueType))
        {
            return string.Empty;
        }

        var accessSpecifier = classOrEnumOrStructOrInterface.GetAccessModifier();
        var typeModifier = classOrEnumOrStructOrInterface.GetTypeModifiers();
        var typename = GetTypeName(classOrEnumOrStructOrInterface, genericSepratorStartTag, genericSepratorEndTag);
        var inhereted = new List<string>();
        if (classOrEnumOrStructOrInterface.BaseType != typeof(object) &&
            classOrEnumOrStructOrInterface.BaseType != typeof(ValueType) &&
            classOrEnumOrStructOrInterface.BaseType != typeof(Enum))
        {
            inhereted.Add(GetTypeName(classOrEnumOrStructOrInterface.BaseType!, genericSepratorStartTag, genericSepratorEndTag));
        }

        foreach (var inter in classOrEnumOrStructOrInterface.GetDirectImplementedInterfaces())
        {
            inhereted.Add(GetTypeName(inter, genericSepratorStartTag, genericSepratorEndTag));
        }
        var inheritedText = string.Join(",", inhereted);
        var constraints = GetTypeConstraints(classOrEnumOrStructOrInterface);
        var constraintsText = string.Join(" ", constraints);

        return $@"{accessSpecifier} {typeModifier}{typename}{(inheritedText.Length > 0 ? $":{inheritedText}" : "")} {(constraints.Any() ? constraintsText : "")}";

    }
    public static string AsString(this ConstructorInfo constructor, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        if (constructor == null) return string.Empty;

        var constructorAccessSpecifier = constructor.GetConstructorAccessModifier();
        var constructorAccessModifier = constructor.GetConstructorModifiers();
        var name = GetName(constructor.DeclaringType?.Name);
        var parameters = constructor.GetParameters().Select(p => GetParemeterTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
        var parametersText = string.Join(",", parameters);
        return $@"{constructorAccessSpecifier} {constructorAccessModifier} {name}({parametersText})";
    }
    public static string AsString(this MethodInfo method, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        if (method == null) return string.Empty;

        var methodAccessSpecifier = method.GetMethodAccessModifier();
        var methodModifier = method.GetMethodModifiers();
        var methodReturnTypes = GetMemberReturnTypes(method.ReturnType, genericSepratorStartTag, genericSepratorEndTag);
        var name = GetName(method.Name);
        var parameters = method.GetParameters().Select(p => GetParemeterTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
        var parametersText = string.Join(",", parameters);
        var constraints = GetMethodConstraints(method);
        var constraintsText = string.Join(" ", constraints);
        if (method.IsGenericMethod)
        {
            var pars = method.GetGenericArguments().Select(p => GetName(p.Name)).ToList();
            var parmText = string.Join(",", pars);
            return $@"{methodAccessSpecifier} {methodModifier} {methodReturnTypes} {name}{genericSepratorStartTag}{parmText}{genericSepratorEndTag}({parametersText}) {constraintsText}";
        }
        else
        {
            return $@"{methodAccessSpecifier} {methodModifier} {methodReturnTypes} {name}({parametersText})";
        }
    }
    public static string AsString(this PropertyInfo property, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        if (property == null) return string.Empty;

        var propertyAccessSpecifier = property.GetPropertyAccessModifier();
        var propertyModifier = property.GetPropertyModifiers();
        var propertyReturnTypes = GetMemberReturnTypes(property.PropertyType, genericSepratorStartTag, genericSepratorEndTag);
        var name = GetName(property.Name);
        var getText = property.CanRead ? $"get;" : "";
        var setText = string.Empty;
        if (property.SetIsAllowed())
        {
            setText = $@"set;";
        }
        else if (property.SetIsAllowed(checkInitSetter: true))
        {
            setText = $@"init;";
        }
        var propertGeterSetter = $"{{{getText}{setText}}}";
        return $@"{propertyAccessSpecifier} {propertyModifier} {propertyReturnTypes} {name}{propertGeterSetter}";
    }
    public static string AsString(this FieldInfo field, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        if (field == null) return string.Empty;

        var fieldAccessSpecifier = field.GetFieldAccessModifier();
        var fieldModifier = field.GetFieldModifiers();
        var fieldReturnTypes = GetMemberReturnTypes(field.FieldType, genericSepratorStartTag, genericSepratorEndTag);
        var name = GetName(field.Name);
        return $@"{fieldAccessSpecifier} {fieldModifier} {fieldReturnTypes} {name}";
    }
    public static string AsString(EventInfo @event, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        if (@event == null) return string.Empty;

        var eventAccessSpecifier = @event.GetEventAccessModifier();
        var eventModifier = @event.GetEventModifiers();
        var eventReturnTypes = GetMemberReturnTypes(@event.EventHandlerType, genericSepratorStartTag, genericSepratorEndTag);
        var name = GetName(@event.Name);
        return $@"{eventAccessSpecifier} {eventModifier} {eventReturnTypes} {name}";
    }
    public static string NameAsString(this Type classOrEnumOrStructOrInterface, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {
        //check type is not supported
        if (classOrEnumOrStructOrInterface is null ||
            classOrEnumOrStructOrInterface.IsPrimitive() ||
            !(classOrEnumOrStructOrInterface.IsClass ||
            classOrEnumOrStructOrInterface.IsEnum ||
            classOrEnumOrStructOrInterface.IsInterface ||
            classOrEnumOrStructOrInterface.IsValueType))
        {
            return string.Empty;
        }
        return GetTypeName(classOrEnumOrStructOrInterface, genericSepratorStartTag, genericSepratorEndTag);
    }
    #endregion Public

    #region Private Members
    private static string GetTypeName(Type type, string genericSepratorStartTag, string genericSepratorEndTag)
    {
        if (type.IsGenericType)
        {
            var name = GetName(type.Name);
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{name}{genericSepratorStartTag}{parmText}{genericSepratorEndTag}";
        }
        return $"{type.Name}";
    }
    private static string GetGenericTypeName(Type type, string genericSepratorStartTag, string genericSepratorEndTag)
    {
        var name = GetName(type.Name);

        if (type.IsGenericType)
        {
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{name}{genericSepratorStartTag}{parmText}{genericSepratorEndTag}";
        }

        if (type.IsArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"[{name}{genericSepratorStartTag}{returnParmText}{genericSepratorEndTag}]";
            }
            else
            {
                return $"{name}";
            }
        }
        return $"{type.Name}";
    }
    private static string GetParemeterTypeName(ParameterInfo type, string genericSepratorStartTag, string genericSepratorEndTag)
    {
        var pName = GetName(type.Name);
        var name = GetName(type.ParameterType.Name);
        if (type.ParameterType.IsGenericType)
        {
            var parameters = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{name}{genericSepratorStartTag}{parmText}{genericSepratorEndTag} {pName}";
        }

        if (type.ParameterType.IsArray)
        {
            var returnParms = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"{name.Split("[]")[0]}{genericSepratorStartTag}{returnParmText}{genericSepratorEndTag}[] {pName}";
            }
        }
        return $"{name} {pName}";
    }
    private static HashSet<string> GetTypeConstraints(Type type)
    {
        if (type.IsGenericType)
        {
            var parmeters = type.GetGenericArguments();
            GetParmetersConstraints(parmeters);
        }

        return new HashSet<string>();
    }
    private static HashSet<string> GetMethodConstraints(MethodInfo method)
    {
        if (method.IsGenericMethod)
        {
            var parmeters = method.GetGenericArguments();
            return GetParmetersConstraints(parmeters);
        }

        return new HashSet<string>();
    }
    private static HashSet<string> GetParmetersConstraints(Type[] parmeters)
    {
        var parmData = new HashSet<string>();
        foreach (var prm in parmeters)
        {
            if (prm.IsGenericParameter)
            {
                var constrains = prm.GetGenericParameterConstraints().Where(x => x != typeof(ValueType)).Select(x => x.Name).ToArray();
                var constraintsText = string.Join(",", constrains);
                if (constraintsText.Length > 0)
                {
                    //custom type as constraints
                    parmData.Add($"where {prm.Name}:{constraintsText}");
                }
                else
                {
                    var attributes = prm.GenericParameterAttributes;
                    // default classs constraints
                    if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
                    {
                        if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                        {
                            parmData.Add($"where {prm.Name}:{Reference_Type},{Default_Constructor}");
                        }
                        else
                        {
                            parmData.Add($"where {prm.Name}:{Reference_Type}");
                        }
                    }
                    else if ((attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
                    {
                        parmData.Add($"where {prm.Name}:{ValueType_Type}");
                    }
                    else if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                    {
                        parmData.Add($"where {prm.Name}:{Default_Constructor}");
                    }

                }
            }
        }

        return parmData;
    }
    private static string GetMemberReturnTypes(Type type, string genericSepratorStartTag, string genericSepratorEndTag)
    {
        var name = GetName(type.Name);
        if (type.IsGenericType)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var returnParmText = string.Join(",", returnParms);
            return $"{name}{genericSepratorStartTag}{returnParmText}{genericSepratorEndTag}";
        }
        else if (type.IsArray || type.IsSZArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p, genericSepratorStartTag, genericSepratorEndTag)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"{name.Split("[]")[0]}{genericSepratorStartTag}{returnParmText}{genericSepratorEndTag}[]";
            }
            else
            {
                return $"{name.Split("[]")[0]}[]";
            }

        }
        else
        {
            return $"{name}";
        }
    }
    private static string GetName(string? name) => name?.Split('`')[0] ?? string.Empty;

    #endregion Private Members
}
