using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace VReflector;

public static class Is
{
    public static Type GetType<T>() => typeof(T);
    public static string GetName<T>() => nameof(T);
    public static bool Equal(this DateTime actual, DateTime expected, TimeSpan tolerance) =>
        (actual - expected).Duration() < tolerance;
    public static bool Equal(this DateTimeOffset actual, DateTimeOffset expected, TimeSpan tolerance) =>
        (actual - expected).Duration() < tolerance;
    public static bool Equal(this TimeOnly actual, TimeOnly expected, TimeSpan tolerance) => (actual - expected).Duration() < tolerance;
    public static bool Equal(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance) => (actual - expected).Duration() < tolerance;
    
}
