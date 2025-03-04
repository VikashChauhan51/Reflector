﻿using System.Text.RegularExpressions;

namespace VReflector;

public static class IsString
{
    public static bool HasMixedOrNoCase(this string? value)
    {

        if (value == null) return false;
        var hasUpperCase = false;
        var hasLowerCase = false;

        foreach (var ch in value)
        {
            hasUpperCase |= char.IsUpper(ch);
            hasLowerCase |= char.IsLower(ch);

            if (hasUpperCase && hasLowerCase)
            {
                return true;
            }
        }

        return !hasUpperCase && !hasLowerCase;
    }
    public static bool IsUpperCase(this string? value)
    {
        if (value == null) return false;
        foreach (var ch in value)
        {
            if (!char.IsUpper(ch))
            {
                return false;
            }
        }

        return true;
    }
    public static bool IsLowerCase(this string? value)
    {
        if (value == null) return false;
        foreach (var ch in value)
        {
            if (!char.IsLower(ch))
            {
                return false;
            }
        }

        return true;
    }
    public static bool HasValue(this string? value)
    {
        if (value is null || value.Trim().Length == 0)
        {
            return false;
        }
        return true;
    }
    public static bool IsPalindrome(this string? value)
    {
        if (value == null) return false;
        int len = value.Length;
        for (int i = 0; i < len / 2; i++)
        {
            if (value[i] != value[len - i - 1])
            {
                return false;
            }
        }
        return true;
    }
    public static bool IsAnagram(this string value, string other)
    {
        if (value == null || other == null) return false;
        if (value.Length != other.Length) return false;

        Dictionary<char, long> charCounts = new();

        foreach (char c in value)
        {
            if (charCounts.ContainsKey(c))
            {
                charCounts[c]++;
            }
            else
            {
                charCounts[c] = 1;
            }
        }

        foreach (char c in other)
        {
            if (!charCounts.ContainsKey(c) || charCounts[c]-- < 0)
            {
                return false;
            }
        }

        return true;
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
    public static int IndexOfFirstMismatch(this string value, string expected, IEqualityComparer<string> comparer)
    {
        for (int index = 0; index < value.Length; index++)
        {
            if (index >= expected.Length || !comparer.Equals(value[index..(index + 1)], expected[index..(index + 1)]))
            {
                return index;
            }
        }

        return -1;
    }
    public static string? Capitalize(this string? @this)
    {
        if (@this ==null || @this.Length == 0)
        {
            return @this;
        }

        char[] charArray = @this.ToCharArray();
        charArray[0] = char.ToUpperInvariant(charArray[0]);
        return new string(charArray);
    }
    public static int CountSubstring(this string str, string substring, IEqualityComparer<string> comparer)
    {
        string actual = str ?? string.Empty;
        string search = substring ?? string.Empty;

        int count = 0;
        int maxIndex = actual.Length - search.Length;
        for (int index = 0; index <= maxIndex; index++)
        {
            if (comparer.Equals(actual[index..(index + search.Length)], search))
            {
                count++;
            }
        }

        return count;
    }
}
