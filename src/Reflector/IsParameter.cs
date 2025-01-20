using System.Reflection;

namespace VReflector;

public static class IsParameter
{
    public static bool IsNullable(this ParameterInfo parameter) =>
   parameter.ParameterType.IsNullable(parameter.Member, parameter.CustomAttributes);
}
