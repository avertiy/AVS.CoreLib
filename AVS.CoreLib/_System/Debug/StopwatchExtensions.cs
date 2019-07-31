using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib._System.Debug
{
    public static class StopwatchExtensions
    {
        public static void Execute(this Stopwatch watch, Action action)
        {
            watch.Start();
            action();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"Execution time: {watch.ElapsedMilliseconds} ms",
                $"PERFORMANCE [thread: {Thread.CurrentThread.ManagedThreadId}]");
        }

        public static T Execute<T>(this Stopwatch watch, Func<T> action)
        {
            watch.Start();
            var result = action();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"Execution time: {watch.ElapsedMilliseconds} ms",
                $"PERFORMANCE [thread: {Thread.CurrentThread.ManagedThreadId}]");
            return result;
        }

        public static async Task<T> Execute<T>(this Stopwatch watch, Func<Task<T>> action)
        {
            watch.Start();
            var result = await action();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"Execution time: {watch.ElapsedMilliseconds} ms",
                $"PERFORMANCE [thread: {Thread.CurrentThread.ManagedThreadId}]");
            return result;
        }
    }
}