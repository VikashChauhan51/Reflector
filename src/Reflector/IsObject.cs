using System.Diagnostics;

namespace VReflector;

public static class IsObject
{
    public static Type? GetType(this object? obj) => obj?.GetType();
    public static bool Same(this object? actual, object? expected)
    {
        if (actual == null || expected == null)
            return false;

        return ReferenceEquals(actual, expected);
    }
    public static bool Equal(this object? actual, object? expected)
    {
        if (actual == null || expected == null)
            return false;

        Type actualType = actual.GetType();
        Type expectedType = expected.GetType();

        if (actualType != expectedType)
            return false;

        if (actualType.IsValueType)
            return actual.Equals(expected);

        if (IsType.Record(actualType))
            return actual.Equals(expected);

        if (actual is IComparable comparable)
        {
            return comparable.CompareTo(expected) == 0;
        }

        return ReferenceEquals(actual, expected);
    }
    public static string GetStackTrace(this object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            return stackTrace.ToString();
        }
        return string.Empty;
    }
    public static bool IsStackTraceType(this object obj)
    {
        return obj is StackTrace;
    }
    public static string GetCallerMethod(this object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            var frame = stackTrace.GetFrame(1); // Caller frame
            return frame?.GetMethod()?.Name ?? string.Empty;
        }
        return string.Empty;
    }
}
