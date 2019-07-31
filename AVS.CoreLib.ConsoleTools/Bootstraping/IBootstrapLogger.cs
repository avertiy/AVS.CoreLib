using System;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    /// <summary>
    /// Logger prints (console or text file or whatever) log messages when program/service is starting
    /// </summary>
    public interface IBootstrapLogger
    {
        void ClearLine();
        void WriteLine(string message);
        void WriteLine(FormattableString formattable);
        void WriteError(Exception ex, bool stackTrace);
    }

    public class BootstrapLogger : IBootstrapLogger
    {
        public void ClearLine()
        {
        }

        public void WriteLine(string message)
        {
        }

        public void WriteLine(FormattableString formattable)
        {
        }

        public void WriteLine(string format, params object[] args)
        {
        }

        public void WriteError(Exception ex, bool stackTrace)
        {
        }
    }
}