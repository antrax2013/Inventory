using System.Diagnostics;

namespace Inventory;

public static class BenchmarkHelper
{
    public static async Task Run(string name, int iterations, Func<Task> action)
    {
        var timings = new long[iterations];
        var swGlobal = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, iterations).Select(async i =>
        {
            var t0 = Stopwatch.GetTimestamp();
            await action();
            var t1 = Stopwatch.GetTimestamp();
            timings[i] = t1 - t0;
        });

        await Task.WhenAll(tasks);

        swGlobal.Stop();

        PrintStats(name, swGlobal.ElapsedMilliseconds, timings);
    }

    private static void PrintStats(string name, long totalMs, long[] timings)
    {
        var freq = Stopwatch.Frequency;

        double ToUs(long ticks) => ticks * 1_000_000.0 / freq;

        var avg = timings.Average(ToUs);
        var max = timings.Max(ToUs);
        var min = timings.Min(ToUs);

        Console.WriteLine($"\n=== {name} ===");
        Console.WriteLine($"Total time       : {totalMs} ms");
        Console.WriteLine($"Ops/sec          : {timings.Length / (totalMs / 1000.0):N0}");
        Console.WriteLine($"Avg latency      : {avg:N2} µs");
        Console.WriteLine($"Min latency      : {min:N2} µs");
        Console.WriteLine($"Max latency      : {max:N2} µs");

        //PrintHistogram(timings.Select(ToUs).ToArray());
    }

    private static void PrintHistogram(double[] values)
    {
        var buckets = new[]
        {
        (0, 1),
        (1, 5),
        (5, 10),
        (10, 50),
        (50, 100),
        (100, 500),
        (500, 1000),
        (1000, 5000)
    };

        Console.WriteLine("\nHistogram (latency µs):");

        foreach (var (min, max) in buckets)
        {
            var count = values.Count(v => v >= min && v < max);
            Console.WriteLine($"{min,4}–{max,4} µs : {new string('█', count / (values.Length / 50 + 1))}");
        }
    }
}