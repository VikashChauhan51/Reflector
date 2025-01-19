using System.Text;

namespace VReflector;

public static class IsMemory
{
    public static long GetMemoryUsage(bool forceFullCollection = false)
    {
        if (forceFullCollection)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        return GC.GetTotalMemory(forceFullCollection);
    }

    public static long GetHeapSize()
    {
        var gcInfo = GC.GetGCMemoryInfo();
        return gcInfo.HeapSizeBytes;
    }

    public static double GetGCFragmentation()
    {
        var gcInfo = GC.GetGCMemoryInfo();
        return (double)gcInfo.FragmentedBytes / gcInfo.HeapSizeBytes;
    }

    public static long GetAllocatedMemoryForCurrentThread()
    {
        return GC.GetAllocatedBytesForCurrentThread();
    }

    public static long GetTotalAllocatedMemory()
    {
        return GC.GetTotalAllocatedBytes();
    }

    public static string GetMemoryUsageDetails()
    {
        var gcInfo = GC.GetGCMemoryInfo();
        var sb = new StringBuilder();

        sb.AppendLine($"Heap Size: {gcInfo.HeapSizeBytes} bytes");
        sb.AppendLine($"Fragmented Bytes: {gcInfo.FragmentedBytes} bytes");
        sb.AppendLine($"Memory Usage: {GC.GetTotalMemory(false)} bytes");

        return sb.ToString();
    }
}
