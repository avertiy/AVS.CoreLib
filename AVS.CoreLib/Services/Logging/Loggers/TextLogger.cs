using System;
using System.Text;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    public class TextLogger : BaseLogger
    {
        private readonly StringBuilder _sb;
        public bool AddPrefix = true;
        public int Count { get; protected set; }
        public TextLogger()
        {
            _sb = new StringBuilder();
        }

        public TextLogger(StringBuilder sb)
        {
            _sb = sb;
        }

        public void Clear()
        {
            _sb.Clear();
        }

        public StringBuilder GetStringBuilder()
        {
            return _sb;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
        
        protected override void WriteLog(LogLevel level, string message, string details)
        {
            _sb.AppendLine(AddPrefix ? $"{level} {DateTime.UtcNow:G}: {message}" : message);

            if (!string.IsNullOrEmpty(details))
                _sb.AppendLine($"{details}");

            Count++;
        }
    }
}