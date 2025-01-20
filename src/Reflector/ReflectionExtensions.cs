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
