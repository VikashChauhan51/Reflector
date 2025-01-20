using Reflector;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace VReflector;

public static class IsType
{
    private const string ImplicitCastMethodName = "op_Implicit";
    private const string ExplicitCastMethodName = "op_Explicit";
    private const BindingFlags PublicInstanceMembersFlag =
        BindingFlags.Public | BindingFlags.Instance;

    private const BindingFlags AllStaticAndInstanceMembersFlag =
        PublicInstanceMembersFlag | BindingFlags.NonPublic | BindingFlags.Static;

    public static Array? CreateEmptyArray([DisallowNull] this Type? type)
    {
        if (type == null)
            return null;

        return Array.CreateInstance(type, 0);
    }
    public static IEnumerable<Type> GetImplementedInterfaces([DisallowNull] this Type type)
    {
        if (type == null)
            yield break;

        foreach (var @interface in type.GetInterfaces())
            yield return @interface;
    }
    public static IEnumerable<Type> GetBaseTypes([DisallowNull] this Type type)
    {
        if (type == null)
            yield break;

        var currentBaseType = type.BaseType;
        while (currentBaseType != null)
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }
    public static IEnumerable<Type> GetParentTypes([DisallowNull] this Type type)
    {
        if (type == null)
            yield break;

        foreach (var @interface in type.GetInterfaces())
            yield return @interface;

        var currentBaseType = type.BaseType;
        while (currentBaseType != null)
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }
    public static IEnumerable<Type> GetParentTypesExceptDefault([DisallowNull] this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        return GetParentTypes(type).Where(p => p != typeof(object) || p != typeof(ValueType));
    }
    public static IEnumerable<Type> GetDirectImplementedInterfaces([DisallowNull] this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        Type[] allInterfaces = type.GetInterfaces();
        Type[] inheritedInterfaces = type.BaseType?.GetInterfaces() ?? Array.Empty<Type>();
        return allInterfaces.Except(inheritedInterfaces).ToList();

    }
    public static ConstructorInfo[]? GetAllConstructors([DisallowNull] this Type type)
    {
        return type?.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.ToArray();
    }
    public static object? InvokeStaticMethod([DisallowNull] this Type type, string name, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            args);
    }
    public static object? GetStaticProperty([DisallowNull] this Type type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            Array.Empty<object>());
    }
    public static object? SetStaticProperty([DisallowNull] this Type type, string name, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { value });
    }
    public static object? InvokeInstancMethod([DisallowNull] this Type type, string name, object target, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            args);
    }
    public static object? GetInstanceProperty([DisallowNull] this Type type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            Array.Empty<object>());
    }
    public static object? SetInstanceProperty([DisallowNull] this Type type, string name, object target, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target, new object[] { value });
    }
    public static object? GetInstanceField([DisallowNull] this Type type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            Array.Empty<object>());
    }
    public static object? GetStaticField([DisallowNull] this Type type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            Array.Empty<object>());
    }
    public static object? SetInstanceField([DisallowNull] this Type type, string name, object target, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance,
            null,
            target, new object[] { value });
    }
    public static object? SetStaticField([DisallowNull] this Type type, string name, object value)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.SetField | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { value });
    }
    public static object? SetCustomStructInstanceProperty([DisallowNull] this Type type, string name, object target, object value)
    {
        if (IsType.IsUserdefinedStruct(type))
            return null;
        var propInfo = type.GetProperty(name, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
        if (propInfo == null) return null;

        propInfo.SetValue(target, value, null);
        return target;
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
    public static MethodInfo? GetExplicitConversionOperator(this Type type, Type sourceType, Type targetType)
    {
        return type
            .GetConversionOperators(sourceType, targetType, name => name is "op_Explicit")
            .SingleOrDefault();
    }
    public static MethodInfo? GetImplicitConversionOperator(this Type type, Type sourceType, Type targetType)
    {
        return type
            .GetConversionOperators(sourceType, targetType, name => name is "op_Implicit")
            .SingleOrDefault();
    }
    public static PropertyInfo GetIndexerByParameterTypes(this Type type, IEnumerable<Type> parameterTypes)
    {
        return type.GetProperties(AllStaticAndInstanceMembersFlag)
            .SingleOrDefault(p =>
                p.IsIndexer() && p.GetIndexParameters().Select(i => i.ParameterType).SequenceEqual(parameterTypes));
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
    public static ConstructorInfo[]? GetPublicAndProtectedInstanceConstructors(this Type type)
    {
        return type?.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Where(c => c.IsPublic || c.IsFamily).ToArray();
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

    public static AccessModifier GetTypeAccessModifier(this Type type)
    {

        return type switch
        {
            _ when type.IsPublic => AccessModifier.Public,
            _ when type.IsNestedPublic => AccessModifier.Public,
            _ when type.IsNestedPrivate => AccessModifier.Private,
            _ when type.IsNestedFamily => AccessModifier.Protected,
            _ when type.IsNestedAssembly => AccessModifier.Internal,
            _ when type.IsNestedFamANDAssem => AccessModifier.PrivateProtected,
            _ when type.IsNestedFamORAssem => AccessModifier.ProtectedInternal,
            _ when type.IsNotPublic => AccessModifier.Internal,
            _ => AccessModifier.Internal
        };
    }
    public static string GetAccessModifier(this Type type)
    {
        if (type == null ||
            type.IsArray ||
            type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(object) ||
            type == typeof(ValueType) ||
            IsType.Primitive(type))
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
            Primitive(type))
        {
            return string.Empty;
        }

        return type switch
        {
            _ when type.IsArray && type.IsInterface => "interface ",
            _ when type.IsAbstract && IsType.RecordClass(type) => "abstract record ",
            _ when type.IsSealed && IsType.RecordClass(type) => "sealed record ",
            _ when type.IsAbstract && type.IsSealed => "static class ",
            _ when type.IsAbstract && !type.IsSealed && !IsType.RecordClass(type) => "abstract class ",
            _ when type.IsSealed && type.IsClass && !IsType.RecordClass(type) => "sealed class ",
            _ when type.IsEnum => "enum ",
            _ when IsType.IsReadonlyStruct(type) && IsType.RecordStruct(type) => "readonly record struct ",
            _ when IsType.Primitive(type) && !IsType.RecordStruct(type) => "readonly struct ",
            _ when IsType.RecordStruct(type) => "record struct ",
            _ when IsType.IsStruct(type) => "struct ",
            _ when IsType.RecordClass(type) => "record ",
            _ when type.IsClass => "class ",
            _ => string.Empty
        };
    }
    public static bool CanCast<T>([DisallowNull] this Type baseType)
    {
        if (baseType is null)
        {
            return false;
        }
        return CanCastImplicit<T>(baseType) || CanCastExplicit<T>(baseType);
    }
    public static string? GetName([DisallowNull] this Type type) => type?.Name;
    public static object? GetDefaultValue([DisallowNull] this Type? type)
    {
        return type?.IsValueType == true ? Activator.CreateInstance(type) : null;
    }
    public static bool Primitive([DisallowNull] this Type type)
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
                    ,typeof(Int16)
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
                    ,typeof(Half)
               }.Contains(type)
            || Convert.GetTypeCode(type) != TypeCode.Object);
    }
    public static bool Record([DisallowNull] this Type type)
    {
        return type != null && (RecordClass(type) || RecordStruct(type));

    }
    public static bool RecordClass([DisallowNull] this Type type)
    {
        if (type == null || !type.IsClass)
            return false;

        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is { } &&
            type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
                .GetMethod?.IsDecoratedWith<CompilerGeneratedAttribute>() == true;
    }
    public static bool RecordStruct([DisallowNull] this Type type)
    {
        if (type == null || !type.IsValueType || type.IsPrimitive)
            return false;

        return type.BaseType == typeof(ValueType) &&
            type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
                new Type[] { typeof(StringBuilder) }, null) is { } &&
            type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null,
                    new Type[] { type, type }, null)?
                .IsDecoratedWith<CompilerGeneratedAttribute>() == true;
    }
    public static bool KeyValuePair([DisallowNull] this Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }
    public static bool IsEnum([DisallowNull] this Type type) => type?.GetTypeInfo()?.BaseType == typeof(Enum);
    public static bool IsNullable(this Type? type) => type is null || type.IsClass || System.Nullable.GetUnderlyingType(type) != null;
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
    public static bool IsStruct([DisallowNull] this Type type)
    {
        return (type != null && type.IsValueType && !type.IsPrimitive && !type.IsEnum);
    }
    public static bool IsUserdefinedStruct([DisallowNull] this Type type)
    {
        var typeInfo = type?.GetTypeInfo();
        if (typeInfo == null)
            return false;

        var isStruct = !Primitive(type!) && typeInfo.IsValueType && !typeInfo.IsEnum && type != typeof(Guid);
        if (!isStruct) return false;
        var ctor = typeInfo.GetConstructor(new[] { typeof(string) });
        return ctor != null;
    }
    public static bool IsStatic([DisallowNull] this Type type)
    {
        return type != null && type.IsAbstract && type.IsSealed;
    }
    public static bool IsAnonymous([DisallowNull] this Type type)
    {
        return type != null &&
               type.GetCustomAttributes(false).Any(a => a is CompilerGeneratedAttribute) &&
               type.Name.StartsWith("<>") && type.IsClass;
    }
    private static bool IsTuple([DisallowNull] this Type type)
    {
        if (type == null || !type.IsGenericType)
        {
            return false;
        }

        Type openType = type.GetGenericTypeDefinition();

        return openType == typeof(ValueTuple<>)
            || openType == typeof(ValueTuple<,>)
            || openType == typeof(ValueTuple<,,>)
            || openType == typeof(ValueTuple<,,,>)
            || openType == typeof(ValueTuple<,,,,>)
            || openType == typeof(ValueTuple<,,,,,>)
            || openType == typeof(ValueTuple<,,,,,,>)
            || (openType == typeof(ValueTuple<,,,,,,,>) && IsTuple(type.GetGenericArguments()[7]))
            || openType == typeof(Tuple<>)
            || openType == typeof(Tuple<,>)
            || openType == typeof(Tuple<,,>)
            || openType == typeof(Tuple<,,,>)
            || openType == typeof(Tuple<,,,,>)
            || openType == typeof(Tuple<,,,,,>)
            || openType == typeof(Tuple<,,,,,,>)
            || (openType == typeof(Tuple<,,,,,,,>) && IsTuple(type.GetGenericArguments()[7]));
    }
    public static bool IsReadonlyStruct([DisallowNull] this Type type)
    {
        return type != null && IsStruct(type) && type.GetCustomAttributes(typeof(IsReadOnlyAttribute), false).Any();
    }
    public static bool IsReadonlyRecordStruct([DisallowNull] this Type type)
    {
        return type != null && RecordStruct(type) && type.GetCustomAttributes(typeof(IsReadOnlyAttribute), false).Any();
    }
    public static bool IsDerivedFrom([DisallowNull] this Type childType, [DisallowNull] Type parentType)
    {
        return childType != null && parentType != null && childType.IsSubclassOf(parentType);
    }
    public static bool ClosedTypeOf([DisallowNull] this Type type, [DisallowNull] Type genericTypeDefinition)
    {
        return type != null && genericTypeDefinition != null && type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
    }
    public static bool IsEnumerable([DisallowNull] this Type type)
    {
        return type.IsArray || (type != typeof(string) && AssignableTo(type, typeof(System.Collections.IEnumerable)));
    }
    public static bool AssignableTo([DisallowNull] this Type type, [DisallowNull] Type toType)
    {
        if (type == null || toType == null) return false;

        return toType.IsGenericTypeDefinition
            ? toType.IsAssignableTo(type)
            : type.IsAssignableFrom(toType);
    }
    public static bool CanBeNull(this Type type)
    {
        if (type == null) return true;

        if (!type.IsGenericParameter)
        {
            return type.IsClass || type.IsInterface || IsNullable(type);
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

        return type.GetConstraintTypes().Any((Type t) => CanBeNull(t));
    }
    public static bool IsUserdefinedClass([DisallowNull] this Type type)
    {
        if (type is null ||
           type.IsInterface ||
           type.IsValueType ||
           typeof(Delegate).IsAssignableFrom(type.BaseType) ||
           Primitive(type))
        {
            return false;
        }
        return type.IsClass || IsType.RecordClass(type);
    }
    public static bool HasAttribute<TAttribute>([DisallowNull] this Type type, bool inherit = false) where TAttribute : Attribute
    {
        return type != null && Attribute(type, typeof(TAttribute), inherit: inherit);
    }
    public static bool Attribute([DisallowNull] this Type type, [DisallowNull] Type attributeType, bool inherit)
    {
        return type != null && System.Attribute.IsDefined(type, attributeType, inherit: inherit);
    }
    public static bool IsDeepMutable([DisallowNull] this Type? type)
    {
        if (type == null)
            return false;

        if (IsMutable(type?.GetTypeInfo()))
            return true;

        var inheritedTypes = type.GetParentTypes().Where(x => x != typeof(object)).Select(i => i.GetTypeInfo());

        foreach (TypeInfo inheritedType in inheritedTypes)
        {
            if (IsMutable(inheritedType))
            {
                return true;
            }
        }

        return false;
    }
    public static bool IsMutable([DisallowNull] this Type? type)
    {
        if (type == null)
            return false;

        return IsMutable(type.GetTypeInfo());
    }
    public static bool IsMutable([DisallowNull] this TypeInfo? type)
    {
        if (type == null)
            return false;

        if (type == typeof(object))
            return true;

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Any(p => p.SetIsAllowed(checkNonPublicSetter: false)) ||
                type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(f => !f.IsInitOnly).Any();
    }
    public static bool IsMemory([DisallowNull] this Type type)
    {
        if (type == null) return false;
        if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Memory`1")
        {
            return true;
        }
        return false;
    }
    public static bool IsMemory([DisallowNull] this Type type, out Type? elementType)
    {
        if (IsMemory(type))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }
    public static bool IsBuffer([DisallowNull] this Type type)
    {
        return type != null && (type.IsArray || IsMemory(type));
    }
    public static bool IsReadOnlyMemory([DisallowNull] this Type type)
    {
        if (type == null) return false;
        if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.ReadOnlyMemory`1")
        {
            return true;
        }
        return false;
    }
    public static bool IsReadOnlyMemory(this Type type, out Type? elementType)
    {
        if (IsReadOnlyMemory(type))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }
    public static bool IsAssignableToOpenGeneric([DisallowNull] this Type type, [DisallowNull] Type elementType)
    {
        if (elementType.IsInterface)
        {
            return ImplementationOfOpenGeneric(type, elementType);
        }

        return type == elementType || DerivedFromOpenGeneric(type, elementType);
    }
    public static bool ImplementationOfOpenGeneric([DisallowNull] this Type type, [DisallowNull] Type elementType)
    {
        if (type.IsInterface && type.IsGenericType &&
            type.GetGenericTypeDefinition() == elementType)
        {
            return true;
        }
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == elementType);
    }
    public static bool DerivedFromOpenGeneric([DisallowNull] this Type? type, [DisallowNull] Type? elementType)
    {
        if (type == null || elementType == null)
        {
            return false;
        }

        if (type == elementType)
        {
            return false;
        }

        for (Type? baseType = type; baseType is not null; baseType = baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == elementType)
            {
                return true;
            }
        }

        return false;
    }
    public static bool IsSameOrInherits(this Type actualType, Type expectedType)
    {
        return actualType == expectedType ||
            expectedType.IsAssignableFrom(actualType);
    }

    #region Private members
    private static bool CanCastImplicit<T>(Type baseType)
    {
        return CanCast<T>(baseType, ImplicitCastMethodName);
    }
    private static bool CanCastExplicit<T>(Type baseType)
    {
        return CanCast<T>(baseType, ExplicitCastMethodName);
    }
    private static bool CanCast<T>(Type baseType, string castMethodName)
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
    private static IEnumerable<MethodInfo> GetConversionOperators(this Type type, Type sourceType, Type targetType,
    Func<string, bool> predicate)
    {
        return type
            .GetMethods()
            .Where(m =>
                m.IsPublic
                && m.IsStatic
                && m.IsSpecialName
                && m.ReturnType == targetType
                && predicate(m.Name)
                && m.GetParameters() is { Length: 1 } parameters
                && parameters[0].ParameterType == sourceType);
    }
    #endregion
}
