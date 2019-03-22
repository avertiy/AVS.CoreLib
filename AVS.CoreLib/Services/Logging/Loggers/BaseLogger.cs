using System;
using System.Collections.Generic;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Services.Logging.Filters;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    public abstract class BaseLogger : ILogger
    {
        public IFilter Filter { get; set; }
        public LogLevel Level { get; set; }

        protected BaseLogger()
        {
            Filter = new DuplicateFilter();
        }

        protected BaseLogger(IFilter duplicateChecker)
        {
            Filter = duplicateChecker;
        }
        
        public void System(string message, string details="")
        {
            Write(LogLevel.SYSTEM, message, details);
        }

        public void Debug(string message, string details = "")
        {
            Write(LogLevel.DEBUG, message, details);
        }

        public void Info(string message, string details = "")
        {
            Write(LogLevel.INFO, message, details);
        }

        public void Fail(string message, string details = "")
        {
            Write(LogLevel.INFO_FAIL, message, details);
        }

        public void Success(string message, string details = "")
        {
            Write(LogLevel.INFO_SUCCESS, message, details);
        }

        public void Warning(string message, string details = "")
        {
            Write(LogLevel.WARNING, message, details);
        }

        public void Error(string message, Exception exception)
        {
            if (exception is System.Threading.ThreadAbortException)
                return;
            Write(LogLevel.ERROR, message, exception.ToString());
        }

        public void Error(string message, string details = "")
        {
            Write(LogLevel.ERROR, message,details);
        }

        public void Fatal(string message, Exception exception)
        {
            Write(LogLevel.FATAL, message, exception.ToString());
        }

        public virtual void Write(LogLevel level, string message, string details)
        {
            if(Level > level)
                return;
            if(Filter != null && !Filter.ShouldWrite(level, message, details))
                return;
            WriteLog(level, message, details);
        }

        protected abstract void WriteLog(LogLevel level, string message, string details);

        protected internal virtual void WriteMany(IEnumerable<Log> logs)
        {
            foreach (var log in logs)
            {
                WriteLog(log.LogLevel, log.ShortMessage, log.FullMessage);
            }
        }
    }
}