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

}
