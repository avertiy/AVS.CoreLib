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

        public TaskLogWriter(ILogBuffer logBuffer, ILogger logger, IAppConfig config) : base(logger, config)
        {
            _logBuffer = logBuffer;
            _logBuffer.SetLogger(logger);
            _logBuffer.UseBuffer = false;
            
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

    /*
    public class TaskLogWriter : LogWriter
    {
        private DateTime _start;
        private DateTime _end;
        private string _taskName;
        private string _result;
        private bool _isSuccess;
        private Exception _error;
        protected IBufferedLogger BufferedLogger;

        public TaskLogWriter(IAppConfig config, IBufferedLogger  logger) 
            : base(logger, config)
        {
            BufferedLogger = logger;
        }

        

        public virtual void StartTask(string name)
        {
            Clear();
            _taskName = name;
            _start = DateTime.Now;
            Write($"----====  Task {name}  ====----");
        }

        public virtual void EndTask()
        {
            _end = DateTime.Now;
            var ts = _end - _start;

            if (SystemLogging)
            {
                WriteSystemDetails($"Start: {_start:g}; End: {_end:g}; Execution time: {ts.TotalMilliseconds} ms");
            }
            else if (DetailedLogging)
            {
                WriteDetails($"Execution time: {ts.TotalMilliseconds} ms");
            }
        }

        public virtual void Result(bool isSuccess)
        {
            _isSuccess = isSuccess;
        }
        
        public virtual void Result(string message, bool isSuccess)
        {
            _result = message;
            _isSuccess = isSuccess;
        }

        public override void WriteError(string message, Exception ex)
        {
            _error = ex;
            base.WriteError(message, ex);
        }
        
        public void Clear()
        {
            BufferedLogger.Clear();
            _result = null;
            _start = DateTime.MinValue;
            _end = DateTime.MinValue;
            _taskName = null;
            _error = null;
        }

        public override void Flush()
        {
            string msg = _result != null ? $"Task {_taskName} SUCCESS - {_result}" : $"Task {_taskName} SUCCESS";
            LogLevel logLevel = LogLevel.INFO_SUCCESS;

            if (_error != null)
            {
                Write(_error.ToString());
                msg = _result != null ? $"Task {_taskName} FAIL - {_result}" : $"Task {_taskName} FAIL";
                logLevel = LogLevel.ERROR;
            }else if (_isSuccess == false)
            {
                logLevel = LogLevel.INFO_FAIL;
                msg = $"Task {_taskName} - {_result}";
            }

            BufferedLogger.Flush(logLevel, msg);
            this.Clear();
        }
    }*/
}