using System.Diagnostics.CodeAnalysis;
using System.Numerics;


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
    public static object Difference(object a, object b)
    {
        if (a is double || b is double)
        {
            return Convert.ToDouble(a) - Convert.ToDouble(b);
        }

        if (a is float || b is float)
        {
            return Convert.ToSingle(a) - Convert.ToSingle(b);
        }

        if (a is Half aHalf && b is Half bHalf)
        {
            return aHalf - bHalf;
        }

        if (a is decimal || b is decimal)
        {
            return Convert.ToDecimal(a) - Convert.ToDecimal(b);
        }

        if (a is ulong || b is ulong)
        {
            ulong ua = Convert.ToUInt64(a);
            ulong ub = Convert.ToUInt64(b);
            if (ua < ub) throw new OverflowException("Subtraction resulted in a negative value for unsigned types.");
            return ua - ub;
        }

        if (a is long || b is long)
        {
            return Convert.ToInt64(a) - Convert.ToInt64(b);
        }

        if (a is uint || b is uint)
        {
            uint ua = Convert.ToUInt32(a);
            uint ub = Convert.ToUInt32(b);
            if (ua < ub) throw new OverflowException("Subtraction resulted in a negative value for unsigned types.");
            return ua - ub;
        }

        if (a is ushort || b is ushort)
        {
            return (ushort)(Convert.ToUInt16(a) - Convert.ToUInt16(b));
        }

        if (a is short || b is short)
        {
            return (short)(Convert.ToInt16(a) - Convert.ToInt16(b));
        }

        if (a is byte || b is byte)
        {
            return (byte)(Convert.ToByte(a) - Convert.ToByte(b));
        }

        if (a is sbyte || b is sbyte)
        {
            return (sbyte)(Convert.ToSByte(a) - Convert.ToSByte(b));
        }

        return Convert.ToInt32(a) - Convert.ToInt32(b);
    }
    public static T Difference<T>(T a, T b) where T : INumber<T>
    {
        return a - b;
    }
    public static bool AreNotEqual<T>(T a, T b) where T : INumber<T>
    {
        return !a.Equals(b);
    }
    public static bool IsGreaterThan<T>(T a, T b) where T : INumber<T>
    {
        return a > b;
    }
    public static bool IsLessThan<T>(T a, T b) where T : INumber<T>
    {
        return a < b;
    }
    public static bool IsGreaterThanOrEqual<T>(T a, T b) where T : INumber<T>
    {
        return a >= b;
    }
    public static bool IsLessThanOrEqual<T>(T a, T b) where T : INumber<T>
    {
        return a <= b;
    }
    public static bool AreEqual<T>(T a, T b) where T : INumber<T>
    {
        return a.Equals(b);
    }
    public static bool AreEqual<T>(T expected, T actual, T tolerance) where T : INumber<T>
    {
        T difference = T.Abs(expected - actual);
        return difference <= tolerance;
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
    public static bool IsNaN<T>(T value) where T : INumber<T>
    {
        if (T.IsNaN(value))
        {
            return true;
        }
        return false;
    }
    public static bool IsInfinity<T>(T value) where T : INumber<T>
    {
        if (T.IsInfinity(value))
        {
            return true;
        }
        return false;
    }
    public static bool IsZero<T>(T value) where T : INumber<T>
    {
        return value == T.Zero;
    }
    public static T Negate<T>(T value) where T : INumber<T>
    {
        return -value;
    }
    public static T GCD<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.Zero)
        {
            T temp = b;
            b = a % b;
            a = temp;
        }
        return T.Abs(a);
    }
    public static T LCM<T>(T a, T b) where T : INumber<T>
    {
        if (a == T.Zero || b == T.Zero) return T.Zero;
        return T.Abs(a * b) / GCD(a, b);
    }
    public static T Factorial<T>(T value) where T : INumber<T>
    {
        if (value < T.Zero) throw new ArgumentException("Value must be non-negative.");
        T result = T.One;
        for (T i = T.One; i <= value; i++)
        {
            result *= i;
        }
        return result;
    }
    public static T Clamp<T>(T value, T min, T max) where T : INumber<T>
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    public static bool IsInRange<T>(T value, T min, T max, bool inclusive = true) where T : INumber<T>
    {
        if (inclusive)
        {
            return value >= min && value <= max;
        }
        else
        {
            return value > min && value < max;
        }
    }
    public static T Power<T>(T baseValue, T exponent) where T : INumber<T>
    {
        if (exponent.Equals(0))
            return (T)Convert.ChangeType(1, typeof(T)); // Anything to the power of 0 is 1

        T result = (T)Convert.ChangeType(1, typeof(T));
        T currentBase = baseValue;
        T currentExponent = exponent;

        while (currentExponent > (T)Convert.ChangeType(0, typeof(T)))
        {
            if ((currentExponent % (T)Convert.ChangeType(2, typeof(T))) != (T)Convert.ChangeType(0, typeof(T)))
            {
                result *= currentBase;
            }
            currentBase *= currentBase;
            currentExponent /= (T)Convert.ChangeType(2, typeof(T)); // Integer division
        }

        return result;
    }
    public static T ModularExponentiation<T>(T baseValue, T exponent, T modulus) where T : INumber<T>
    {
        T result = (T)Convert.ChangeType(1, typeof(T));
        baseValue = baseValue % modulus;

        while (exponent > (T)Convert.ChangeType(0, typeof(T)))
        {
            if (exponent % (T)Convert.ChangeType(2, typeof(T)) != (T)Convert.ChangeType(0, typeof(T)))
            {
                result = (result * baseValue) % modulus;
            }
            baseValue = (baseValue * baseValue) % modulus;
            exponent = exponent / (T)Convert.ChangeType(2, typeof(T)); // Integer division
        }

        return result;
    }
    public static T ArithmeticSum<T>(T firstTerm, T commonDifference, T numTerms) where T : INumber<T>
    {
        return numTerms * (firstTerm + (firstTerm + (numTerms - (T)Convert.ChangeType(1, typeof(T))) * commonDifference)) / (T)Convert.ChangeType(2, typeof(T));
    }
    public static T GeometricSum<T>(T firstTerm, T commonRatio, T numTerms) where T : INumber<T>
    {
        if (commonRatio == (T)Convert.ChangeType(1, typeof(T)))
            return firstTerm * numTerms;

        return firstTerm * ((T)Convert.ChangeType(1, typeof(T)) - Power<T>(commonRatio, numTerms)) / (T)Convert.ChangeType(1, typeof(T)) - commonRatio;
    }
    public static T BinomialCoefficient<T>(T n, T k) where T : INumber<T>
    {
        if (k > n)
            return (T)Convert.ChangeType(0, typeof(T));

        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }


}
