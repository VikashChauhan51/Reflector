using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace VReflector;

public static class IsStackFrame
{
    public static string? GetSourceCodeStatementFrom([DisallowNull] this StackFrame frame)
    {
        if (frame == null) return null;

        var fileName = frame.GetFileName();
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        var lineNumber = frame!.GetFileLineNumber();
        if (lineNumber == 0)
        {
            return null;
        }

        try
        {
            var lines = File.ReadAllLines(fileName);
            return lines[lineNumber - 1].Trim();
        }
        catch
        {

            return null;
        }

    }

}
