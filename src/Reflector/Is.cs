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
    public static bool SequenceEqual<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected);
    }
    public static bool SequenceEqual<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T>? comparer)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected, comparer);
    }
    public static bool SequenceEqualIgnoreOrder<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
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
    public static bool SequenceEqualIgnoreOrder<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T>? comparer)
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
    public static bool AreSpansEqual(this ReadOnlySpan<char> span1, ReadOnlySpan<char> span2)
    {
        return span1.SequenceEqual(span2);
    }
}
