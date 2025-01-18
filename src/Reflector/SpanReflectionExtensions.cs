namespace VReflector;

public static class SpanReflectionExtensions
{

    /// <summary>
    /// Converts a ReadOnlySpan<char> to a string using reflection.
    /// Useful when dealing with Spans in scenarios where a string is needed.
    /// </summary>
    /// <param name="span">The ReadOnlySpan to convert.</param>
    /// <returns>A string representation of the span.</returns>
    public static string ToStringViaReflector(ReadOnlySpan<char> span)
    {
        // Use the ReadOnlySpan<char> constructor of string if available
        return new string(span);
    }

    /// <summary>
    /// Compares two spans character by character for equality.
    /// </summary>
    /// <param name="span1">The first span.</param>
    /// <param name="span2">The second span.</param>
    /// <returns>True if the spans are equal; otherwise, false.</returns>
    public static bool AreSpansEqual(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2)
    {
        return span1.SequenceEqual(span2);
    }

    /// <summary>
    /// Extracts a substring from a ReadOnlySpan<char>.
    /// </summary>
    /// <param name="span">The span to extract from.</param>
    /// <param name="start">The start index.</param>
    /// <param name="length">The length of the substring.</param>
    /// <returns>A string representation of the extracted substring.</returns>
    public static string Substring(ReadOnlySpan<char> span, int start, int length)
    {
        return new string(span.Slice(start, length));
    }

    /// <summary>
    /// Reflectively analyzes the contents of a span for debugging or logging.
    /// </summary>
    /// <param name="span">The span to analyze.</param>
    /// <returns>A string representing the span's contents.</returns>
    public static string Analyze(ReadOnlySpan<char> span)
    {
        return $"Span Length: {span.Length}, Contents: '{new string(span)}'";
    }

    /// <summary>
    /// Converts any Span<T> to a human-readable format for debugging.
    /// </summary>
    /// <typeparam name="T">The type of the span elements.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>A string representation of the span's contents.</returns>
    public static string ToDebugString<T>(ReadOnlySpan<T> span)
    {
        return $"[ {string.Join(", ", span.ToArray())} ]";
    }

}
