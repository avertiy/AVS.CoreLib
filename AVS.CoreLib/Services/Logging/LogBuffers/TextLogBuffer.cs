using System;
using System.Text;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.LogBuffers
{
    public class TextLogBuffer : LogBufferBase
    {
        private readonly StringBuilder _sb = new StringBuilder();
        public bool AddPrefix = true;

        protected override void AddToBuffer(LogLevel level, string message, string details)
        {
            _sb.AppendLine(AddPrefix ? $"{level} {DateTime.UtcNow:G}: {message}" : message);

            if (!string.IsNullOrEmpty(details))
                _sb.AppendLine($"{details}");

            Count++;
        }

        public override void Flush(LogLevel level, string message)
        {
            EnsureLoggerNotNull();
            //AddToBuffer(level, message, null);
            _sb.AppendLine();
            var details = _sb.ToString();
            Logger.Write(level, message, details);
            Clear();
        }

        public void Clear()
        {
            _sb.Clear();
            Count = 0;
        }

        public int Count { get; protected set; }

        public StringBuilder GetStringBuilder()
        {
            return _sb;
        }
    }
}