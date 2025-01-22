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
    public static bool IsCloseTo(this TimeOnly subject, TimeOnly other, TimeSpan precision)
    {
        long startTicks = other.Add(-precision).Ticks;
        long endTicks = other.Add(precision).Ticks;
        long ticks = subject.Ticks;

        return startTicks <= endTicks
            ? startTicks <= ticks && endTicks >= ticks
            : startTicks <= ticks || endTicks >= ticks;
    }
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
    {
        return dateTime.ToDateTimeOffset(TimeSpan.Zero);
    }
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), offset);
    }
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return dateTime.ToDateTimeOffset(TimeSpan.Zero).ToUnixTimeMilliseconds();
    }
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return dateTime.ToDateTimeOffset(TimeSpan.Zero).ToUnixTimeSeconds();
    }
}
