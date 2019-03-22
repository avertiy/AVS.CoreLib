using System;
using AVS.CoreLib.Services.Logging.LogWriters;

namespace AVS.CoreLib.Services.Logging.Extensions
{
    //log.GetResponse(()=>{... return response},"some message")
    //wrapper.Call(()=>{.. return response},"")

    public static class TaskLogWriterExtensions
    {
        //public static bool Execute(this TaskLogWriter log, Func<IResponse> func, string message)
        //{
        //    var start = DateTime.Now;
        //    var response = func();
        //    if (response.HasError)
        //    {
        //        log.Write(message,LogLevel.INFO_FAIL,);
        //        return false;
        //    }

        //    var ts = DateTime.Now - start;
        //    log.Write(measureTime ? $"{message} {ts.TotalMilliseconds:0.00}ms OK" : $"{message} OK");
        //}


        public static void Run(this TaskLogWriter log, string message, Action<TaskLogWriter> action, bool measureTime = false)
        {
            try
            {
                var start = DateTime.Now;
                action(log);
                var ts = DateTime.Now - start;
                log.Write(measureTime ? $"{message} {ts.TotalMilliseconds:0.00}ms OK" : $"{message} OK");
            }
            catch (Exception ex)
            {
                log.WriteError(message + " FAILED", ex);
            }
        }

        public static void Run<T1>(this TaskLogWriter log, string message, T1 argument, Action<TaskLogWriter,T1> action, bool measureTime = false)
        {
            try
            {
                var start = DateTime.Now;
                action(log, argument);
                var ts = DateTime.Now - start;
                log.Write(measureTime ? $"{message} {ts.TotalMilliseconds:0.00}ms OK" : $"{message} OK");
            }
            catch (Exception ex)
            {
                log.WriteError(message + " FAILED", ex);
            }
        }

        public static void Run(this TaskLogWriter log, string message, Action<TaskLogWriter> action, out TimeSpan ts)
        {
            var start = DateTime.Now;
            try
            {
                action(log);
                log.Write($"{message} OK");
            }
            catch (Exception ex)
            {
                log.WriteError(message + " FAILED", ex);
            }
            ts = DateTime.Now - start;
        }

        public static TResult Run<T1, TResult>(this TaskLogWriter log, string message, T1 argument, Func<TaskLogWriter, T1, TResult> func, bool measureTime = false)
        {
            TResult res = default(TResult);
            try
            {
                var start = DateTime.Now;
                res = func(log, argument);
                var ts = DateTime.Now - start;
                log.Write(measureTime ? $"{message} {ts.TotalMilliseconds:0.00}ms OK" : $"{message} OK");
                return res;
            }
            catch (Exception ex)
            {
                log.WriteError(message + " FAILED", ex);
            }
            return res;
        }

        public static T Run<T>(this TaskLogWriter log, string message, Func<TaskLogWriter, T> func, bool measureTime = false)
        {
            return log.Run(message, (string)null, (l, a) => func(l));
        }

        public static TResult Run<T1,TResult>(this TaskLogWriter log, string message, T1 argument, Func<TaskLogWriter, T1, TResult> func, out TimeSpan ts)
        {
            var start = DateTime.Now;
            TResult res = default(TResult);
            try
            {
                res = func(log, argument);
                log.Write($"{message} OK");
            }
            catch (Exception ex)
            {
                log.WriteError(message + " FAILED", ex);
            }
            ts = DateTime.Now - start;
            return res;
        }
    }
}