using System;
using System.Globalization;
using AVS.CoreLib.ConsoleTools.Bootstraping;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    public class ConsoleWriter: IBootstrapLogger
    {
        public ConsoleWriter()
        {
            FormatProvider = CultureInfo.InvariantCulture;
            _defaultColor = Console.ForegroundColor;
        }

        public ConsoleWriter(IFormatProvider formatProvider)
        {
            FormatProvider = formatProvider;
            _defaultColor = Console.ForegroundColor;
        }

        public IFormatProvider FormatProvider { get; set; }
        private ConsoleColor _defaultColor;

        public ConsoleColor DefaultColor
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                Console.ForegroundColor = value;
            }
        }

        public void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public void SetDefaultColor()
        {
            Console.ForegroundColor = DefaultColor;
        }

        public void SetGrayColor()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void SetRedColor()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        public void ClearLine()
        {
            ConsoleExt.ClearLine();
        }

        public void WriteLine(FormattableString formattable)
        {
            Console.WriteLine(formattable.ToString(FormatProvider));
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(string.Format(FormatProvider, format, args));
        }

        public void WriteException(Exception ex, bool stackTrace)
        {
            ConsoleExt.WriteException(ex,stackTrace);
        }

        public void WriteLine(FormattableString formattable, ConsoleColor color)
        {
            var bakup = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(formattable.ToString(FormatProvider));
            Console.ForegroundColor = bakup;
        }
    }
}
