using System;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    interface IBootstrapLogger
    {
        void ClearLine();
        void WriteLine(FormattableString formattable);
        void WriteLine(string format, params object[] args);
    }

    public class BootstrapLogger : IBootstrapLogger
    {
        public void ClearLine()
        {
        }

        public void WriteLine(FormattableString formattable)
        {
        }

        public void WriteLine(string format, params object[] args)
        {
        }
    }
}