using System.Diagnostics;
using System.Text;

namespace VReflector;

public static class IsStackTrace
{
    public static string GetStackTrace()
    {
        return new StackTrace().ToString();
    }
    public static string GetStackTrace(object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            return stackTrace.ToString();
        }
        return string.Empty;
    }
    public static StackFrame[] GetStackFrames()
    {
        return new StackTrace(true).GetFrames();
    }

    public static bool IsStackTraceType(object obj)
    {
        return obj is StackTrace;
    }

    public static string GetCallerMethod()
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1); // Get the caller's stack frame
        return frame?.GetMethod()?.Name ?? string.Empty;
    }

    public static string GetCallerMethod(object obj)
    {
        if (obj is StackTrace stackTrace)
        {
            var frame = stackTrace.GetFrame(1); // Caller frame
            return frame?.GetMethod()?.Name ?? string.Empty;
        }
        return string.Empty;
    }
    public static string GetMethodFromStackTrace(int frameIndex)
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(frameIndex);
        return frame?.GetMethod()?.Name ?? string.Empty;
    }
    public static string GetStackTraceDetails()
    {
        var stackTrace = new StackTrace(true);  
        var sb = new StringBuilder();

        foreach (var frame in stackTrace.GetFrames())
        {
            sb.AppendLine($"Method: {frame.GetMethod()?.Name}, " +
                           $"File: {frame.GetFileName()}, " +
                           $"Line: {frame.GetFileLineNumber()}");
        }

        return sb.ToString();
    }
    public static int GetCurrentLineNumber()
    {
        var stackTrace = new StackTrace(true);
        var stackFrame = stackTrace.GetFrame(0); // Get the current frame
        return stackFrame?.GetFileLineNumber() ?? -1; // Return the line number, or -1 if not available
    }

    public static string GetCurrentMethodName()
    {
        var stackTrace = new StackTrace(true);
        var stackFrame = stackTrace.GetFrame(1); // Get the calling method (1 level up)
        return stackFrame?.GetMethod()?.Name ?? "Unknown Method";
    }

    public static string? GetCurrentLineCode()
    {
        var stackTrace = new StackTrace(true); 
        var stackFrame = stackTrace.GetFrame(0); // Get the current frame

        int lineNumber = stackFrame?.GetFileLineNumber() ?? -1;
        if (lineNumber == -1)
        {
            return null; 
        }

        string? filePath = stackFrame?.GetFileName();
        if (filePath == null || !File.Exists(filePath))
        {
            return null; 
        }

        try
        {
            var lines = File.ReadLines(filePath).ToList();
            if (lineNumber <= lines.Count)
            {
                return lines[lineNumber - 1]; // Line number is 1-based, so we subtract 1 for 0-based indexing
            }
        }
        catch
        {
            // Ignored
            return null;
        }

        return null;
    }
}
