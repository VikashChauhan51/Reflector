using System.Collections.ObjectModel;
using System.Reflection;
using VReflector;

namespace Reflector;

public static class ReflectionExtensions
{

    #region Constants
    const string Reference_Type = "class";
    const string ValueType_Type = "struct";
    const string Default_Constructor = "new()";
    #endregion Constants

    #region Public
    public static Array? CreateEmptyArray(this Type? type)
    {
        if (type == null)
            return null;

        return Array.CreateInstance(type, 0);
    }
    public static IEnumerable<Type> GetImplementedInterfaces(this Type type)
    {
        if (type == null)
            yield break;

        foreach (var @interface in type.GetInterfaces())
            yield return @interface;
    }
    public static IEnumerable<Type> GetBaseTypes(this Type type)
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
    public static IEnumerable<Type> GetParentTypes(this Type type)
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
    public static IEnumerable<Type> GetParentTypesExceptDefault(this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        return GetParentTypes(type).Where(p => p != typeof(object) || p != typeof(ValueType));
    }
    public static IEnumerable<Type> GetDirectImplementedInterfaces(this Type type)
    {
        if (type == null)
            return Enumerable.Empty<Type>();

        Type[] allInterfaces = type.GetInterfaces();
        Type[] inheritedInterfaces = type.BaseType?.GetInterfaces() ?? Array.Empty<Type>();
        return allInterfaces.Except(inheritedInterfaces).ToList();

    }
    public static ConstructorInfo[]? GetAllConstructors(this Type type)
    {
        return type?.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.ToArray();
    }
    public static ConstructorInfo[]? GetConstructors(this Type type)
    {
        return type?.GetConstructors()?.ToArray();
    }
    public static object? CreateDefaultForImmutable(this Type? type)
    {
        if (type?.GetTypeInfo().IsGenericType == true && type?.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return CreateEmptyArray(type.GetTypeInfo().GetGenericArguments()[0]);
        }
        return IsType.GetDefaultValue(type);
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
        if (IsType.CustomStruct(type))
            return null;
        var propInfo = type.GetProperty(name, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
        if (propInfo == null) return null;

        propInfo.SetValue(target, value, null);
        return target;
    }
    public static object? GetEnumPropertyValue(this PropertyInfo targetType, string value, bool ignoreCase = false) =>
    IsType.Enum(targetType.PropertyType) ? Enum.Parse(targetType.PropertyType, value, ignoreCase) : null;
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
    public static bool IsReadonly(this FieldInfo field)
    {
        return field != null && field.IsInitOnly;
    }
    public static bool IsConstant(this FieldInfo field)
    {
        return field != null && field.IsLiteral;
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
    public static ParameterInfo[]? GetAllParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters();
    }
    public static ParameterInfo[]? GetRequiredParameters(this ConstructorInfo constructor)
    {
        return constructor?.GetParameters().Where(x => x.IsOptional == false).ToArray();
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
            IsType.Primitive(type))
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
            _ when IsType.ReadonlyStruct(type) && IsType.RecordStruct(type) => "readonly record struct ",
            _ when IsType.Primitive(type) && !IsType.RecordStruct(type) => "readonly struct ",
            _ when IsType.RecordStruct(type) => "record struct ",
            _ when IsType.Struct(type) => "struct ",
            _ when IsType.RecordClass(type) => "record ",
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
    public static string GetName<T>() => GetName(nameof(T));
    public static string GetName(this ConstructorInfo constructor) => GetName(constructor?.Name);
    public static string GetName(this MethodInfo method) => GetName(method?.Name);
    public static string GetName(this PropertyInfo property) => GetName(property?.Name);
    public static string GetName(this FieldInfo field) => GetName(field?.Name);
    public static string GetName(this EventInfo @event) => GetName(@event?.EventHandlerType?.Name);
    public static string AsString(this Type classOrEnumOrStructOrInterface, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
    {

        //check type is not supported
        if (classOrEnumOrStructOrInterface is null ||
            classOrEnumOrStructOrInterface == typeof(ValueType) ||
            classOrEnumOrStructOrInterface == typeof(object) ||
            IsType.Primitive(classOrEnumOrStructOrInterface) ||
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
    public static string AsString(this EventInfo @event, string genericSepratorStartTag = "<", string genericSepratorEndTag = ">")
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
            classOrEnumOrStructOrInterface == typeof(ValueType) ||
            classOrEnumOrStructOrInterface == typeof(object) ||
            IsType.Primitive(classOrEnumOrStructOrInterface) ||
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
