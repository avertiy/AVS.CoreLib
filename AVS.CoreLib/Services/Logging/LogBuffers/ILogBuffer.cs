using System;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.LogBuffers
{
    public interface ILogBuffer
    {
        bool UseBuffer { get; set; }
        void SetLogger(ILogger logger);
        void Write(LogLevel level, string message, string details);
        void Flush(LogLevel level, string message);
    }

    public abstract class LogBufferBase : ILogBuffer
    {
        protected ILogger Logger { get; set; }
        public bool UseBuffer { get; set; }
        
        public virtual void SetLogger(ILogger logger)
        {
            Logger = logger;
        }

        protected void EnsureLoggerNotNull()
        {
            if (Logger == null)
                throw new Exception("Logger is not initialized");
        }

        public void Write(LogLevel level, string message, string details)
        {
            EnsureLoggerNotNull();
            if (UseBuffer)
            {
                if (Logger.Level > level)
                    return;
                if (!Logger.Filter.ShouldWrite(level, message, details))
                    return;
                AddToBuffer(level, message, details);
            }
            else
            {
                Logger.Write(level,message,details);
            }
        }

        protected abstract void AddToBuffer(LogLevel level, string message, string details);
        public abstract void Flush(LogLevel level, string message);
    }
}