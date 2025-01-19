using Reflector;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace VReflector;

public static class IsType
{
    public static string? GetName(Type type) => type?.Name;
    public static object? GetDefaultValue(Type? type)
    {
        return type?.IsValueType == true ? Activator.CreateInstance(type) : null;
    }
    public static bool Primitive(Type type)
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
    public static bool Record(Type type)
    {
        return type != null && (RecordClass(type) || RecordStruct(type));

    }
    public static bool RecordClass(Type type)
    {
        if (type == null || !type.IsClass)
            return false;

        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is { } &&
            type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
                .GetMethod?.IsDecoratedWith<CompilerGeneratedAttribute>() == true;
    }
    public static bool RecordStruct(Type type)
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
    public static bool KeyValuePair(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }
    public static bool Enum(Type type) => type?.GetTypeInfo()?.BaseType == typeof(Enum);
    public static bool Nullable(Type? type) => type is null || type.IsClass || System.Nullable.GetUnderlyingType(type) != null;
    public static bool Struct(Type type)
    {
        return (type != null && type.IsValueType && !type.IsPrimitive && !type.IsEnum);
    }
    public static bool CustomStruct(Type type)
    {
        var typeInfo = type?.GetTypeInfo();
        if (typeInfo == null)
            return false;

        var isStruct = !Primitive(type!) && typeInfo.IsValueType && !typeInfo.IsEnum && type != typeof(Guid);
        if (!isStruct) return false;
        var ctor = typeInfo.GetConstructor(new[] { typeof(string) });
        return ctor != null;
    }
    public static bool Static(Type type)
    {
        return type != null && type.IsAbstract && type.IsSealed;
    }
    public static bool Anonymous(Type type)
    {
        return type != null &&
               type.GetCustomAttributes(false).Any(a => a is CompilerGeneratedAttribute) &&
               type.Name.StartsWith("<>") && type.IsClass;
    }
    private static bool IsTuple(Type type)
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
    public static bool ReadonlyStruct(Type type)
    {
        return type != null && Struct(type) && type.GetCustomAttributes(typeof(IsReadOnlyAttribute), false).Any();
    }
    public static bool ReadonlyRecordStruct(Type type)
    {
        return type != null && RecordStruct(type) && type.GetCustomAttributes(typeof(IsReadOnlyAttribute), false).Any();
    }
    public static bool DerivedFrom(Type childType, Type parentType)
    {
        return childType != null && parentType != null && childType.IsSubclassOf(parentType);
    }
    public static bool ClosedTypeOf(Type type, Type genericTypeDefinition)
    {
        return type != null && genericTypeDefinition != null && type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
    }
    public static bool Enumerable(Type type)
    {
        return type.IsArray || (type != typeof(string) && AssignableTo(type, typeof(System.Collections.IEnumerable)));
    }
    public static bool AssignableTo(Type fromType, Type toType)
    {
        if (fromType == null || toType == null) return false;

        return toType.IsGenericTypeDefinition
            ? toType.IsAssignableTo(fromType)
            : fromType.IsAssignableFrom(toType);
    }
    public static bool CanBeNull(Type type)
    {
        if (type == null) return true;

        if (!type.IsGenericParameter)
        {
            return type.IsClass || type.IsInterface || Nullable(type);
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
    public static bool Public(Type type)
    {
        return type != null && type.IsPublic;
    }
    public static bool Class(Type type)
    {
        return type != null && type.IsClass;
    }
    public static bool Interface(Type type)
    {
        return type != null && type.IsInterface;
    }
    public static bool Abstract(Type type)
    {
        return type != null && type.IsAbstract;
    }
    public static bool Sealed(Type type)
    {
        return type != null && type.IsSealed;
    }
    public static bool ValueType(Type type)
    {
        return type != null && type.IsValueType;
    }
    public static bool GenericType(Type type)
    {
        return type != null && type.IsGenericType;
    }
    public static bool Attribute(Type type, Type attributeType, bool inherit)
    {
        return type != null && System.Attribute.IsDefined(type, attributeType, inherit: inherit);
    }
    public static bool DeepMutable(Type? type)
    {
        if (type == null)
            return false;

        if (Mutable(type?.GetTypeInfo()))
            return true;

        var inheritedTypes = type.GetParentTypes().Where(x => x != typeof(object)).Select(i => i.GetTypeInfo());

        foreach (TypeInfo inheritedType in inheritedTypes)
        {
            if (Mutable(inheritedType))
            {
                return true;
            }
        }

        return false;
    }
    public static bool Mutable(Type? type)
    {
        if (type == null)
            return false;

        return Mutable(type.GetTypeInfo());
    }
    public static bool Mutable(TypeInfo? type)
    {
        if (type == null)
            return false;

        if (type == typeof(object))
            return true;

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Any(p => p.SetIsAllowed(checkNonPublicSetter: false)) ||
                type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(f => !f.IsInitOnly).Any();
    }
    public static bool Memory(Type type)
    {
        if (type == null) return false; 
        if (GenericType(type) && type.GetGenericTypeDefinition().FullName == "System.Memory`1")
        {
            return true;
        }
        return false;
    }
    public static bool Memory(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        if (Memory(type))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }
    public static bool IsBuffer(Type type)
    {
        return IsArray(type) || Memory(type);
    }
    public static bool IsArray(Type type)
    {
        return type?.IsArray ?? false;
    }
    public static bool ReadOnlyMemory(Type type)
    {
        if (GenericType(type) && type.GetGenericTypeDefinition().FullName == "System.ReadOnlyMemory`1")
        {
            return true;
        }
        return false;
    }
    public static bool ReadOnlyMemory(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        if (ReadOnlyMemory(type))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }
    public static bool AssignableToOpenGeneric(Type type, Type elementType)
    {
        if (elementType.IsInterface)
        {
            return ImplementationOfOpenGeneric(type, elementType);
        }

        return type == elementType || DerivedFromOpenGeneric(type, elementType);
    }
    public static bool ImplementationOfOpenGeneric(Type type, Type elementType)
    {
        if (type.IsInterface && type.IsGenericType &&
            type.GetGenericTypeDefinition() == elementType)
        {
            return true;
        }
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == elementType);
    }
    public static bool DerivedFromOpenGeneric(Type? type, Type? elementType)
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
}
