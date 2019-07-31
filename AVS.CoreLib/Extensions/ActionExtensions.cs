using System;
using System.Diagnostics;
using AVS.CoreLib.Services.Logging.LogWriters;

namespace AVS.CoreLib.Extensions
{
    public static class ActionExtensions
    {
        public static TimeSpan InvokeWithTimer(this Action action)
        {
            var watch = Stopwatch.StartNew();
            action.Invoke();
            watch.Stop();
            return watch.Elapsed;
        }

        public static TimeSpan Invoke(this Action action, Action<Exception> errorHandler)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                errorHandler(ex);
            }

            watch.Stop();
            return watch.Elapsed;
        }

        /// <summary>
        /// usage: action.Invoke(log).OnSucess("sucess").OnFail("fail");
        /// </summary>
        public static InvokeResult Invoke(this Action action, TaskLogWriter log)
        {
            var watch = Stopwatch.StartNew();
            Exception error = null;
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            watch.Stop();
            return new InvokeResult(watch.Elapsed, log, error);
        }
    }

    //var ts = log.Run(()=>{}).OnSucess("").OnFail("");
    public readonly struct InvokeResult
    {
        public TimeSpan Timespan { get; }
        public LogWriter Log { get; }
        public Exception Error { get; }

        public InvokeResult(TimeSpan timespan, LogWriter log, Exception error)
        {
            Timespan = timespan;
            Log = log;
            Error = error;
        }

        public InvokeResult OnSucess(string message)
        {
            Log.Write(message);
            return this;
        }

        public InvokeResult OnFail(string message)
        {
            if(Error != null)
                Log.WriteError(message, Error);
            return this;
        }
        public static implicit operator TimeSpan(InvokeResult obj)
        {
            return obj.Timespan;
        }

        public override string ToString()
        {
            return $"{Timespan.TotalMilliseconds} ms";
        }
    }
}