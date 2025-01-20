using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;


namespace VReflector;

public static class IsNumerics
{
    public static bool IsInRange<T>([DisallowNull] this T value, T min, T max, bool inclusive = true) where T : INumber<T>
    {
        if (inclusive)
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
        else
        {
            return value.CompareTo(min) > 0 && value.CompareTo(max) < 0;
        }
    }
    public static T Difference<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a - b;
    }
    public static bool AreNotEqual<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) != 0;
    }
    public static bool GreaterThan<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) > 0;
    }
    public static bool LessThan<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) < 0;
    }
    public static bool GreaterThanOrEqual<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) >= 0;
    }
    public static bool LessThanOrEqual<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) <= 0;
    }
    public static bool IsNaN<T>([DisallowNull] this T value) where T : INumber<T>
    {
        if (T.IsNaN(value))
        {
            return true;
        }
        return false;
    }
    public static bool IsInfinity<T>([DisallowNull] this T value) where T : INumber<T>
    {
        if (T.IsInfinity(value))
        {
            return true;
        }
        return false;
    }
    public static bool IsPositive<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value?.CompareTo(T.Zero) > 0;
    }
    public static bool IsNegative<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value?.CompareTo(T.Zero) < 0;
    }
    public static bool IsEvenInteger<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value.IsEvenInteger();
    }
    public static bool IsOddInteger<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value.IsOddInteger();
    }
    public static bool IsZero<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value?.CompareTo(T.Zero) == 0;
    }
    public static bool IsPositiveOrZero<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value?.CompareTo(T.Zero) >= 0;
    }
    public static bool IsNegativeOrZero<T>([DisallowNull] this T value) where T : INumber<T>
    {
        return value?.CompareTo(T.Zero) <= 0;
    }
    public static T GCD<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        while (b != T.Zero)
        {
            T temp = b;
            b = a % b;
            a = temp;
        }
        return T.Abs(a);
    }
    public static T LCM<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        if (a == T.Zero || b == T.Zero) return T.Zero;
        return T.Abs(a * b) / GCD(a, b);
    }
    public static T Factorial<T>([DisallowNull] this T value) where T : INumber<T>
    {
        if (value < T.Zero) throw new ArgumentException("Value must be non-negative.");
        T result = T.One;
        for (T i = T.One; i <= value; i++)
        {
            result *= i;
        }
        return result;
    }
    public static T Clamp<T>([DisallowNull] this T value, T min, T max) where T : INumber<T>
    {
        if (min > max)
        {
            throw new NotSupportedException("min is greater than max value");
        }
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    public static T Max<T>([DisallowNull] this T x, T y) where T : INumber<T>
    {
        if (x != y)
        {
            if (!T.IsNaN(x))
            {
                return y < x ? x : y;
            }

            return x;
        }
        return T.IsNegative(y) ? x : y;
    }
    public static T Min<T>([DisallowNull] this T x, T y) where T : INumber<T>
    {
        if ((x != y) && !T.IsNaN(x))
        {
            return x < y ? x : y;
        }
        return T.IsNegative(x) ? x : y;
    }
    public static int Sign<T>([DisallowNull] this T value) where T : INumber<T>
    {
        if (value != T.Zero)
        {
            return T.IsNegative(value) ? -1 : +1;
        }
        return 0;
    }
    public static T CopySign<T>([DisallowNull] this T value, T sign) where T : INumber<T>
    {
        T result = value;
        if (T.IsNegative(value) != T.IsNegative(sign))
        {
            result = checked(-result);
        }
        return result;
    }
    public static T Power<T>([DisallowNull] this T baseValue, T exponent) where T : INumber<T>
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
    public static T ModularExponentiation<T>([DisallowNull] this T baseValue, T exponent, T modulus) where T : INumber<T>
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
    public static T ArithmeticSum<T>([DisallowNull] this T firstTerm, T commonDifference, T numTerms) where T : INumber<T>
    {
        return numTerms * (firstTerm + (firstTerm + (numTerms - (T)Convert.ChangeType(1, typeof(T))) * commonDifference)) / (T)Convert.ChangeType(2, typeof(T));
    }
    public static T GeometricSum<T>([DisallowNull] this T firstTerm, T commonRatio, T numTerms) where T : INumber<T>
    {
        if (commonRatio == (T)Convert.ChangeType(1, typeof(T)))
            return firstTerm * numTerms;

        return firstTerm * ((T)Convert.ChangeType(1, typeof(T)) - Power<T>(commonRatio, numTerms)) / (T)Convert.ChangeType(1, typeof(T)) - commonRatio;
    }
    public static T BinomialCoefficient<T>([DisallowNull] this T n, T k) where T : INumber<T>
    {
        if (k > n)
            return (T)Convert.ChangeType(0, typeof(T));

        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }
    public static bool AreEqual<T>([DisallowNull] this T a, T b) where T : INumber<T>
    {
        return a?.CompareTo(b) == 0;
    }
    public static bool AreEqual<T>([DisallowNull] this T expected, T actual, T tolerance) where T : INumber<T>
    {
        T difference = T.Abs(expected - actual);
        return difference <= tolerance;
    }
    public static bool AreEqual(this double expected, double actual, double tolerance)
    {
        if (double.IsNaN(expected) && double.IsNaN(actual))
            return true;

        if (double.IsInfinity(expected) || double.IsNaN(expected) || double.IsNaN(actual))
        {
            return expected.Equals(actual);
        }

        return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance);
    }
    public static bool AreEqual(this float expected, float actual, float tolerance)
    {
        if (float.IsNaN(expected) && float.IsNaN(actual))
            return true;

        if (float.IsInfinity(expected) || float.IsNaN(expected) || float.IsNaN(actual))
        {
            return expected.Equals(actual);
        }
        return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance);
    }
    public static bool AreEqual(this Half expected, Half actual)
    {
        if (Half.IsNaN(expected) && Half.IsNaN(actual))
            return true;

        if (Half.IsInfinity(expected) || Half.IsNaN(expected) || Half.IsNaN(actual))
        {
            return expected.Equals(actual);
        }

        return expected.Equals(actual);
    }
    public static bool AreEqual(this decimal expected, decimal actual, decimal tolerance)
    {
        var decimalTolerance = Convert.ToDecimal(tolerance);
        if (decimalTolerance > 0m)
            return Math.Abs(expected - actual) <= decimalTolerance;

        return expected.Equals(actual);
    }
    public static bool AreEqual(this ulong expected, ulong actual, ulong tolerance)
    {
        var ulongTolerance = Convert.ToUInt64(tolerance);
        if (ulongTolerance > 0ul)
        {
            var diff = expected >= actual ? expected - actual : actual - expected;
            return diff <= ulongTolerance;
        }

        return expected.Equals(actual);
    }
    public static bool AreEqual(this long expected, long actual, long tolerance)
    {
        var longTolerance = Convert.ToInt64(tolerance);
        if (longTolerance > 0L)
            return Math.Abs(expected - actual) <= longTolerance;

        return expected.Equals(actual);
    }
    public static bool AreEqual(this uint expected, uint actual, uint tolerance)
    {
        var uintTolerance = Convert.ToUInt32(tolerance);
        if (uintTolerance > 0)
        {
            var diff = expected >= actual ? expected - actual : actual - expected;
            return diff <= uintTolerance;
        }

        return expected.Equals(actual);
    }
    public static bool AreEqual(this int expected, int actual, int tolerance)
    {
        var intTolerance = Convert.ToInt32(tolerance);
        if (intTolerance > 0)
            return Math.Abs(expected - actual) <= intTolerance;

        return expected.Equals(actual);
    }

}
