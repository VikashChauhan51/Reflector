using System.Diagnostics.CodeAnalysis;

namespace VReflector;

public static class IsSequence
{
    public static bool SequenceEqual<T>([DisallowNull] this IEnumerable<T> actual, [NotNullWhen(true)] IEnumerable<T> expected)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected);
    }
    public static bool SequenceEqual<T>([DisallowNull] this IEnumerable<T> actual, [NotNullWhen(true)] IEnumerable<T> expected, IEqualityComparer<T>? comparer)
    {
        if (actual == null || expected == null)
            return actual == expected;

        return actual.SequenceEqual(expected, comparer);
    }
    public static bool SequenceEqualIgnoreOrder<T>([DisallowNull] this IEnumerable<T> actual, [NotNullWhen(true)] IEnumerable<T> expected)
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
    public static bool SequenceEqualIgnoreOrder<T>([DisallowNull] this IEnumerable<T> actual, [NotNullWhen(true)] IEnumerable<T> expected, IEqualityComparer<T>? comparer)
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
    public static bool AreSpansEqual([DisallowNull] this ReadOnlySpan<char> span1, [NotNullWhen(true)] ReadOnlySpan<char> span2)
    {
        return span1.SequenceEqual(span2);
    }
    public static bool HasUniqueItems<T>([DisallowNull] this IEnumerable<T> sequence)
    {
        var set = new HashSet<T>();
        foreach (var item in sequence)
        {
            if (!set.Add(item))
                return false;
        }
        return true;
    }
    public static bool HasUniqueItems<T>([DisallowNull] this IEnumerable<T> sequence, IEqualityComparer<T> comparer)
    {
        var set = new HashSet<T>(comparer);
        foreach (var item in sequence)
        {
            if (!set.Add(item))
                return false;
        }
        return true;
    }
    public static (long DuplicateCount, long Count) GetDuplicateItems<T>([DisallowNull] this IEnumerable<T> sequence, IEqualityComparer<T> comparer = null)
    {
        long nonUniqueCount = 0;
        long allCount = 0;
        if (sequence != null)
        {
            HashSet<T> set = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();

            foreach (var item in sequence)
            {
                allCount++;
                if (!set.Add(item))
                {
                    nonUniqueCount++;
                }
            }
        }
        return (nonUniqueCount, allCount);
    }
    public static bool IsSortedAscending<T>([DisallowNull] this IEnumerable<T> collection, IComparer<T> comparer)
    {
        if (collection == null || !collection.Any())
        {
            return true;
        }

        T previous = collection.First();

        foreach (T item in collection.Skip(1))
        {
            if (comparer.Compare(previous, item) >= 0)
            {
                return false;
            }
            previous = item;
        }

        return true;
    }
    public static bool IsSortedAscending<T>([DisallowNull] this IEnumerable<T> collection)
    {
        var comparer = Comparer<T>.Default;
        var enumerator = collection.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true;
        }

        T previous = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (comparer.Compare(previous, enumerator.Current) >= 0)
            {
                return false;
            }
            previous = enumerator.Current;
        }

        return true;
    }
    public static bool IsSortedDescending<T>([DisallowNull] this IEnumerable<T> collection, IComparer<T> comparer)
    {
        if (collection == null || !collection.Any())
        {
            return true;
        }

        T previous = collection.First();

        foreach (T item in collection.Skip(1))
        {
            if (comparer.Compare(previous, item) <= 0)
            {
                return false;
            }
            previous = item;
        }

        return true;
    }
    public static bool IsSortedDescending<T>([DisallowNull] this IEnumerable<T> collection)
    {
        var comparer = Comparer<T>.Default;
        var enumerator = collection.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true;
        }

        T previous = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (comparer.Compare(previous, enumerator.Current) <= 0)
            {
                return false;
            }
            previous = enumerator.Current;
        }

        return true;
    }
    public static bool IsSortedAscendingAndUnique<T>([DisallowNull] this IEnumerable<T> collection, IComparer<T> comparer)
    {
        if (collection == null || !collection.Any())
        {
            return true;
        }

        T previous = collection.First();

        foreach (T item in collection.Skip(1))
        {
            if (comparer.Compare(previous, item) > 0)
            {
                return false;
            }
            previous = item;
        }

        return true;
    }
    public static bool IsSortedAscendingAndUnique<T>([DisallowNull] this IEnumerable<T> collection)
    {
        var comparer = Comparer<T>.Default;
        var enumerator = collection.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true;
        }

        T previous = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (comparer.Compare(previous, enumerator.Current) > 0)
            {
                return false;
            }
            previous = enumerator.Current;
        }

        return true;
    }
    public static bool IsSortedDescendingAndUnique<T>([DisallowNull] this IEnumerable<T> collection, IComparer<T> comparer)
    {
        if (collection == null || !collection.Any())
        {
            return true;
        }

        T previous = collection.First();

        foreach (T item in collection.Skip(1))
        {
            if (comparer.Compare(previous, item) < 0)
            {
                return false;
            }
            previous = item;
        }

        return true;
    }
    public static bool IsSortedDescendingAndUnique<T>([DisallowNull] this IEnumerable<T> collection)
    {
        var comparer = Comparer<T>.Default;
        var enumerator = collection.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true;
        }

        T previous = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (comparer.Compare(previous, enumerator.Current) < 0)
            {
                return false;
            }
            previous = enumerator.Current;
        }

        return true;
    }
    public static bool IsSubset<T>([DisallowNull] this IEnumerable<T> subset, IEnumerable<T> superset)
    {
        if (subset == null || superset == null)
        {
            return false;
        }
        var supersetHashSet = new HashSet<T>(superset);
        return subset.All(item => supersetHashSet.Contains(item));
    }
    public static IDictionary<TKey, TValue> MergeWith<TKey, TValue>([DisallowNull] this IDictionary<TKey, TValue> first, [DisallowNull] IDictionary<TKey, TValue> second)
    {
        if (first == null || second == null)
        {
            throw new ArgumentNullException(first == null ? nameof(first) : nameof(second));
        }

        var result = new Dictionary<TKey, TValue>(first);

        foreach (var kvp in second)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }
    public static bool ContainsAllKeys<TKey, TValue>([DisallowNull] this IDictionary<TKey, TValue> dictionary, [DisallowNull] IEnumerable<TKey> keys)
    {
        if (dictionary == null || keys == null)
        {
            throw new ArgumentNullException(dictionary == null ? nameof(dictionary) : nameof(keys));
        }

        return keys.All(key => dictionary.ContainsKey(key));
    }


}
