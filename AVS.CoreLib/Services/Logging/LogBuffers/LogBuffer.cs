using System;
using System.Collections.Generic;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Services.Logging.Loggers;

namespace AVS.CoreLib.Services.Logging.LogBuffers
{
    public class LogBuffer : LogBufferBase
    {
        private readonly List<Log> _buffer = new List<Log>();
        private BaseLogger _baseLogger;
        public override void SetLogger(ILogger logger)
        {
            _baseLogger = logger as BaseLogger;
            if(_baseLogger == null)
                throw new ArgumentException("LogBuffer requires the logger to be of a BaseLogger type");
            base.SetLogger(logger);
        }

        protected override void AddToBuffer(LogLevel level, string message, string details)
        {
            var log = new Log()
            {
                LogLevel = level,
                ShortMessage = message,
                FullMessage = details,
                CreatedOnUtc = DateTime.UtcNow
            };
            _buffer.Add(log);
        }

        public override void Flush(LogLevel level, string message)
        {
            EnsureLoggerNotNull();
            if(!string.IsNullOrEmpty(message))
                AddToBuffer(level, message, null);
            _baseLogger.WriteMany(_buffer);
            Clear();
        }

        public int Count => _buffer.Count;

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}