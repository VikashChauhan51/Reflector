using System.Diagnostics.CodeAnalysis;


namespace VReflector;

public static class IsNumerics
{
    public static bool NumericType([NotNullWhen(true)] object? obj)
    {
        return obj is double or float or byte or sbyte or decimal or int or uint or long or ulong or short or ushort or Half;
    }

    public static int Compare(object expected, object actual)
    {
        if (!NumericType(expected) || !NumericType(actual))
            throw new ArgumentException("Both arguments must be numeric");

        if (expected is double or float || actual is double or float)
            return Convert.ToDouble(expected).CompareTo(Convert.ToDouble(actual));

        if (expected is decimal || actual is decimal)
            return Convert.ToDecimal(expected).CompareTo(Convert.ToDecimal(actual));

        if (expected is ulong || actual is ulong)
            return Convert.ToUInt64(expected).CompareTo(Convert.ToUInt64(actual));

        if (expected is long || actual is long)
            return Convert.ToInt64(expected).CompareTo(Convert.ToInt64(actual));

        if (expected is uint || actual is uint)
            return Convert.ToUInt32(expected).CompareTo(Convert.ToUInt32(actual));

        return Convert.ToInt32(expected).CompareTo(Convert.ToInt32(actual));
    }

    public static bool AreEqual(object expected, object actual, object tolerance)
    {
        if (expected is double || actual is double)
            return AreEqual(Convert.ToDouble(expected), Convert.ToDouble(actual), Convert.ToDouble(tolerance));

        if (expected is float || actual is float)
            return AreEqual(Convert.ToSingle(expected), Convert.ToSingle(actual), Convert.ToSingle(tolerance));

        if (expected is Half h1 && actual is Half h2)
            return AreEqual(h1, h2);

        if (expected is decimal || actual is decimal)
            return AreEqual(Convert.ToDecimal(expected), Convert.ToDecimal(actual), Convert.ToDecimal(tolerance));

        if (expected is ulong || actual is ulong)
            return AreEqual(Convert.ToUInt64(expected), Convert.ToUInt64(actual), Convert.ToUInt64(tolerance));

        if (expected is long || actual is long)
            return AreEqual(Convert.ToInt64(expected), Convert.ToInt64(actual), Convert.ToInt64(tolerance));

        if (expected is uint || actual is uint)
            return AreEqual(Convert.ToUInt32(expected), Convert.ToUInt32(actual), Convert.ToUInt32(tolerance));

        return AreEqual(Convert.ToInt32(expected), Convert.ToInt32(actual), Convert.ToInt32(tolerance));
    }

    public static bool AreEqual(double expected, double actual, double tolerance)
    {
        if (double.IsNaN(expected) && double.IsNaN(actual))
            return true;

        if (double.IsInfinity(expected) || double.IsNaN(expected) || double.IsNaN(actual))
        {
            return expected.Equals(actual);
        }

        return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance);
    }

    public static bool AreEqual(float expected, float actual, float tolerance)
    {
        if (float.IsNaN(expected) && float.IsNaN(actual))
            return true;

        if (float.IsInfinity(expected) || float.IsNaN(expected) || float.IsNaN(actual))
        {
            return expected.Equals(actual);
        }
        return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance);
    }

    public static bool AreEqual(Half expected, Half actual)
    {
        if (Half.IsNaN(expected) && Half.IsNaN(actual))
            return true;

        if (Half.IsInfinity(expected) || Half.IsNaN(expected) || Half.IsNaN(actual))
        {
            return expected.Equals(actual);
        }

        return expected.Equals(actual);
    }

    public static bool AreEqual(decimal expected, decimal actual, decimal tolerance)
    {
        var decimalTolerance = Convert.ToDecimal(tolerance);
        if (decimalTolerance > 0m)
            return Math.Abs(expected - actual) <= decimalTolerance;

        return expected.Equals(actual);
    }

    public static bool AreEqual(ulong expected, ulong actual, ulong tolerance)
    {
        var ulongTolerance = Convert.ToUInt64(tolerance);
        if (ulongTolerance > 0ul)
        {
            var diff = expected >= actual ? expected - actual : actual - expected;
            return diff <= ulongTolerance;
        }

        return expected.Equals(actual);
    }

    public static bool AreEqual(long expected, long actual, long tolerance)
    {
        var longTolerance = Convert.ToInt64(tolerance);
        if (longTolerance > 0L)
            return Math.Abs(expected - actual) <= longTolerance;

        return expected.Equals(actual);
    }

    public static bool AreEqual(uint expected, uint actual, uint tolerance)
    {
        var uintTolerance = Convert.ToUInt32(tolerance);
        if (uintTolerance > 0)
        {
            var diff = expected >= actual ? expected - actual : actual - expected;
            return diff <= uintTolerance;
        }

        return expected.Equals(actual);
    }
    public static bool AreEqual(int expected, int actual, int tolerance)
    {
        var intTolerance = Convert.ToInt32(tolerance);
        if (intTolerance > 0)
            return Math.Abs(expected - actual) <= intTolerance;

        return expected.Equals(actual);
    }

}
