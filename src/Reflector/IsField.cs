
using System.Reflection;

namespace VReflector;

public static class IsField
{
    public static bool IsNullable(this FieldInfo field) =>
 field.FieldType.IsNullable(field.DeclaringType, field.CustomAttributes);
    public static bool IsReadonly(this FieldInfo field)
    {
        return field != null && field.IsInitOnly;
    }
    public static bool IsConstant(this FieldInfo field)
    {
        return field != null && field.IsLiteral;
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
}
