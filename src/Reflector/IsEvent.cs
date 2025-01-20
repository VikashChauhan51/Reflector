
using System.Reflection;

namespace VReflector;

public static class IsEvent
{
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
}
