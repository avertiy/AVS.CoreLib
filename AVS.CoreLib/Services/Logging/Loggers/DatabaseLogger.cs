using System;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    /// <summary>
    /// Database logger writes logs into Log table
    /// </summary>
    public class DatabaseLogger : BaseLogger
    {
        protected readonly ILogEntityService LogEntityService;

        public DatabaseLogger(ILogEntityService logEntityService) 
        {
            LogEntityService = logEntityService;
        }

        protected override void WriteLog(LogLevel level, string message, string details)
        {
            if (IsDuplicate(level, message, details)) 
                return;

            var log = new Log()
            {
                LogLevel = level,
                ShortMessage = message,
                FullMessage = details,
                CreatedOnUtc = DateTime.UtcNow
            };
            LogEntityService.Insert(log);
        }

        protected bool IsDuplicate(LogLevel level, string message, string details)
        {
            var logs = LogEntityService.GetLastLogs(10);
            foreach (var log in logs)
            {
                if (log.LogLevel == level && log.ShortMessage == message && log.FullMessage == details)
                    return true;
            }
            return false;
        }
    }
}