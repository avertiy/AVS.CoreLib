using System;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Services.Logging.Filters;

namespace AVS.CoreLib.Services.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// by setting higher loglevel the lower levels will be ignored 
        /// </summary>
        LogLevel Level { get; set; }
        IFilter Filter { get; set; }
        void System(string message, string details="");
        void Debug(string message, string details = "");
        void Info(string message, string details = "");
        void Fail(string message, string details = "");
        void Success(string message, string details = "");
        void Warning(string message, string details = "");
        void Error(string message, string details = "");
        void Error(string message, Exception exception);
        void Fatal(string message, Exception exception);
        void Write(LogLevel level, string message, string details="");
    }
}
