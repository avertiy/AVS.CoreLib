using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Filters
{
    public class DuplicateFilter : IFilter
    {
        private Log _prevLog;
        public virtual bool ShouldWrite(LogLevel level, string message, string details = "")
        {
            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(details))
                return false;

            var log = new Log() { LogLevel = level, FullMessage = details, ShortMessage = message };

            if (Equal(log, _prevLog))
                return false;

            _prevLog = log;
            return true;
        }

        protected bool Equal(Log log1, Log log2)
        {
            return log1 != null && log2 != null &&
                   (log1.LogLevel == log2.LogLevel && log1.ShortMessage == log2.ShortMessage &&
                    log1.FullMessage == log2.FullMessage);
        }
    }
}