using System.Reflection;


namespace VReflector;

public static class IsMember
{
    public static bool IsDecoratedWith<TAttribute>(this MemberInfo type)
     where TAttribute : Attribute
    {
        return Attribute.IsDefined(type, typeof(TAttribute), inherit: false);
    }

    public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
    {
        return (T)Attribute.GetCustomAttribute(member, typeof(T));
    }
}
