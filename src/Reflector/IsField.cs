
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
    public static AccessModifier GetFieldAccessModifier(this FieldInfo field)
    {
        return field switch
        {
            _ when field.IsPrivate => AccessModifier.Private,
            _ when field.IsPublic => AccessModifier.Public,
            _ when field.IsFamilyOrAssembly => AccessModifier.ProtectedInternal,
            _ when field.IsFamily => AccessModifier.Protected,
            _ when field.IsAssembly => AccessModifier.Internal,
            _ => AccessModifier.Internal
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
