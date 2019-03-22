using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    /// <summary>
    /// Represents a composite logger to write log for example into Console and into File
    /// </summary>
    public class CompositeLogger : BaseLogger
    {
        protected List<BaseLogger> Loggers = new List<BaseLogger>();
        
        public void AddLogger(BaseLogger logger)
        {
            logger.Level = this.Level;
            Loggers.Add(logger);
        }

        public ILogger GetLogger(Type type)
        {
            return Loggers.FirstOrDefault(l => l.GetType().Name == type.Name);
        }

        protected override void WriteLog(LogLevel level, string message, string details)
        {
            foreach (var logger in Loggers)
            {
                logger.Write(level, message, details);
            }
        }
    }
}