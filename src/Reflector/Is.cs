using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace VReflector;

public static class Is
{
    public static Type? GetType(object? obj) => obj?.GetType();
    public static Type GetType<T>() => typeof(T);
    public static bool InRange<T>([DisallowNull] T comparable, T? from, T? to)
        where T : IComparable<T> =>
        comparable.CompareTo(from) >= 0 &&
        comparable.CompareTo(to) <= 0;
    public static bool Same(object? actual, object? expected)
    {
        if (actual == null && expected == null)
            return true;
        if (actual == null || expected == null)
            return false;

        return ReferenceEquals(actual, expected);
    }
    public static bool Same<T>(T? actual, T? expected) where T : class
    {
        if (actual == null && expected == null)
            return true;
        if (actual == null || expected == null)
            return false;

        return ReferenceEquals(actual, expected);
    }
    public static bool Equal(object? actual, object? expected)
    {
        if (actual == null && expected == null)
            return true;
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
    public static bool Equal<T>(T? actual, T? expected) where T : IEquatable<T>, IComparable<T>
    {
        if (actual == null && expected == null)
            return true;
        if (actual == null || expected == null)
            return false;

        var type = typeof(T);

        if (type.IsValueType)
            return actual.Equals(expected);

        if (IsType.Record(type))
            return actual.Equals(expected);

        if (actual is IComparable comparable)
        {
            return comparable.CompareTo(expected) == 0;
        }

        return ReferenceEquals(actual, expected);
    }
    public static bool Equal<T>(T? expected, T? actual, IEqualityComparer<T> comparer) =>
        comparer.Equals(actual, expected);
    public static bool Equal(decimal actual, decimal expected, decimal tolerance) =>
        Math.Abs(actual - expected) < tolerance;
    public static bool Equal(double actual, double expected, double tolerance) =>
        Math.Abs(actual - expected) < tolerance;
    public static bool Equal(DateTime actual, DateTime expected, TimeSpan tolerance) =>
        (actual - expected).Duration() < tolerance;
    public static bool Equal(DateTimeOffset actual, DateTimeOffset expected, TimeSpan tolerance) =>
        (actual - expected).Duration() < tolerance;
    public static bool Equal(TimeOnly actual, TimeOnly expected, TimeSpan tolerance) => (actual - expected).Duration() < tolerance;
    public static bool Equal(TimeSpan actual, TimeSpan expected, TimeSpan tolerance) => (actual - expected).Duration() < tolerance;
    public static bool GreaterThanOrEqualTo<T>(T? comparable, T? expected)
        where T : IComparable<T>? =>
        Compare(comparable, expected) >= 0;
    public static bool GreaterThanOrEqualTo<T>(T? actual, T? expected, IComparer<T> comparer) =>
        Compare(actual, expected, comparer) >= 0;
    public static bool LessThanOrEqualTo<T>(T? comparable, T? expected)
        where T : IComparable<T>? =>
        Compare(comparable, expected) <= 0;
    public static bool LessThanOrEqualTo<T>(T? actual, T? expected, IComparer<T> comparer) => Compare(actual, expected, comparer) <= 0;
    public static bool GreaterThan<T>(T? comparable, T? expected)
        where T : IComparable<T>? =>
        Compare(comparable, expected) > 0;
    public static bool GreaterThan<T>(T? actual, T? expected, IComparer<T> comparer) =>
        Compare(actual, expected, comparer) > 0;
    public static bool LessThan<T>(T? comparable, T? expected)
        where T : IComparable<T>? =>
        Compare(comparable, expected) < 0;
    public static bool LessThan<T>(T? actual, T? expected, IComparer<T> comparer) =>
        Compare(actual, expected, comparer) < 0;
    private static decimal Compare<T>(T? actual, T? expected, IComparer<T> comparer) =>
        comparer.Compare(actual, expected);
    private static decimal Compare<T>(T? comparable, T? expected)
        where T : IComparable<T>?
    {
        if (!typeof(T).IsValueType)
        {
            if (comparable == null)
                return expected == null ? 0 : -1;
            if (expected == null)
                return +1;
        }
        return comparable!.CompareTo(expected);
    }
    public static bool StringMatchingRegex(string actual, string regexPattern) =>
        Regex.IsMatch(actual, regexPattern);
    public static bool StringContainingIgnoreCase(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.Contains(expected, StringComparison.OrdinalIgnoreCase);
    }
    public static bool StringContaining(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.Contains(expected);
    }
    public static bool EndsWith(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.EndsWith(expected, StringComparison.Ordinal);
    }
    public static bool EndsWithIgnoreCase(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.EndsWith(expected, StringComparison.OrdinalIgnoreCase);
    }
    public static bool StringStarting(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.StartsWith(expected, StringComparison.Ordinal);
    }
    public static bool StringStartingIgnoreCase(string? actual, string expected)
    {
        if (actual == null)
            return false;

        return actual.StartsWith(expected, StringComparison.OrdinalIgnoreCase);
    }
    public static bool StringEqual(string? actual, string? expected)
    {
        return StringComparer.Ordinal.Equals(actual, expected);
    }
    public static bool StringEqualIgnoreCase(string? actual, string? expected)
    {
        return StringComparer.OrdinalIgnoreCase.Equals(actual, expected);
    }
    public static bool SequenceEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected);
    }
    public static bool SequenceEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T>? comparer)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected, comparer);
    }
    public static bool SequenceEqualIgnoreOrder<T>(IEnumerable<T> actual, IEnumerable<T> expected)
    {
        if (actual == null || expected == null)
            return actual == expected;

        var actualList = actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
            return false;

        actualList.Sort();
        expectedList.Sort();

        return actualList.SequenceEqual(expectedList);
    }
    public static bool SequenceEqualIgnoreOrder<T>(IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T>? comparer)
    {
        if (actual == null || expected == null)
            return actual == expected;

        var actualList = actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
            return false;

        actualList.Sort();
        expectedList.Sort();

        return actualList.SequenceEqual(expectedList, comparer);
    }
    public static bool AreSpansEqual(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2)
    {
        return span1.SequenceEqual(span2);
    }
    public static bool HasAttribute<TAttribute>(Type type, bool inherit = false) where TAttribute : Attribute
    {
        return type != null && Attribute.IsDefined(type, typeof(TAttribute), inherit: inherit);
    }
}
