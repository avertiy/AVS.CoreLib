using System;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Logging.LogBuffers;

namespace AVS.CoreLib.Services.Logging.LogWriters
{
    public class TaskLogWriter : LogWriter
    {
        private readonly ILogBuffer _logBuffer;
        private string _message;
        private LogLevel _level;
        private DateTime _start;
        private bool _taskExited = false;

        public TaskLogWriter(ILogBuffer logBuffer, ILogger logger, IAppConfig config, IFormatProvider formatProvider) : base(logger, config)
        {
            _logBuffer = logBuffer;
            _logBuffer.SetLogger(logger);
            _logBuffer.UseBuffer = false;
            FormatProvider = formatProvider;
        }

        protected override void Write(LogLevel level, string message, string details = "")
        {
            _logBuffer.Write(level, message, details);
        }

        
        public void StartTask(string name)
        {
            _start = DateTime.Now;
            _logBuffer.UseBuffer = true;
            WriteSystemDetails($"----====  Task {name} start {_start:G} ====----");
            
            _level = LogLevel.ERROR;
            _message = $"----====  Task {name}   FAILED  ====----";
        }

        public void ExitTask(string message, bool fail = true)
        {
            WriteFail(message);
            _taskExited = true;
            _level = fail ? LogLevel.INFO_FAIL : LogLevel.INFO;
        }

        public void EndTask(string name)
        {
            WriteSystemDetails($"----====  end {DateTime.Now:G} ====----");
            var ts = DateTime.Now - _start;
            _message = $"----====  Task {name}    OK  [{ts.TotalMilliseconds:0.00} ms]  ====----";
            if(!_taskExited)
                _level = LogLevel.INFO_SUCCESS;
        }

        public void Flush()
        {
            _logBuffer.Flush(_level, _message);
            _logBuffer.UseBuffer = false;
        }
    }
}