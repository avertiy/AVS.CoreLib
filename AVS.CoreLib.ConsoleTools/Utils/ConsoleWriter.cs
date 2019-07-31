using System;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.ConsoleTools.Bootstraping;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    public class ConsoleWriter: IBootstrapLogger
    {
        public ConsoleWriter()
        {
            FormatProvider = ConsoleFormatProvider.Instance;
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
        
        public void ClearLine()
        {
            ConsoleExt.ClearLine();
        }

        public void WriteLine(string message)
        {
            ConsoleOut.Print(message);
        }

        public void WriteLine(FormattableString formattable)
        {
            ConsoleOut.PrintF(formattable);
        }

        public void WriteError(Exception ex, bool printStackTrace)
        {
            ConsoleOut.PrintF($"\r\n{ex.GetType().Name:red}: {ex.Message:darkred}");
            if (printStackTrace)
            {
                ConsoleOut.PrintF($"\r\n{ex.StackTrace:darkgray}\r\n");
            }
        }
    }

    public static class ConsoleExtensions
    {
        public static void SetDefaultColor(this ConsoleWriter writer)
        {
            Console.ForegroundColor = writer.DefaultColor;
        }

        public static void SetGrayColor(this ConsoleWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void SetRedColor(this ConsoleWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        //public static void ClearLine(this ConsoleWriter writer)
        //{
        //    ConsoleExt.ClearLine();
        //}

        public static void SetColor(this ConsoleWriter writer, ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
    
    public class ConsoleFormatProvider : IFormatProvider
    {
        private ConsoleFormatProvider()
        {
        }

        public static readonly ConsoleFormatProvider Instance = new ConsoleFormatProvider();

        private readonly ICustomFormatter _formatter = new ConsoleFormatter();
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return _formatter;
            return null;
        }
    }

    public class ColorScheme
    {
        public ColorScheme()
        {
        }

        public ColorScheme(ConsoleColor backgorund, ConsoleColor foreground)
        {
            Backgorund = backgorund;
            Foreground = foreground;
        }

        public ConsoleColor Backgorund { get; set; }
        public ConsoleColor Foreground { get; set; }

        public ConsoleColor GetColor(string s)
        {
            return s == "-b" ? Backgorund : Foreground;
        }

        public static bool TryParse(string str, out ColorScheme scheme)
        {
            scheme = new ColorScheme();
            ConsoleColor color;
            var parts = str.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                //just color by default foreground
                scheme.Foreground = TryParseConsoleColor(parts[0], out color) ? color : Console.ForegroundColor;
                scheme.Backgorund = Console.BackgroundColor;
                return true;
            }
            if (parts.Length == 2)
            {
                if (parts[0] == "-f")
                {
                    scheme.Foreground = TryParseConsoleColor(parts[1], out color) ? color : Console.ForegroundColor;
                    scheme.Backgorund = Console.BackgroundColor;
                    return true;
                }
                if (parts[0] == "-b")
                {
                    scheme.Foreground = Console.ForegroundColor;
                    scheme.Backgorund = TryParseConsoleColor(parts[1], out color) ? color : Console.BackgroundColor;
                    return true;
                }
            }

            if (parts.Length == 4)
            {
                var fColorStr = parts[0] == "-f" ? parts[1] : parts[3];
                var bColorStr = parts[2] == "-b" ? parts[3] : parts[1];
                scheme.Foreground = TryParseConsoleColor(fColorStr, out color) ? color : Console.ForegroundColor;
                scheme.Backgorund = TryParseConsoleColor(bColorStr, out color) ? color : Console.BackgroundColor;
                return true;
            }

            return false;
        }

        private static bool TryParseConsoleColor(string value, out ConsoleColor color)
        {
            if (Enum.TryParse(value, out color))
            {
                return true;
            }
            color = ConsoleColor.Black;
            return false;
        }

        public override string ToString()
        {
            return $"-f {Foreground} -b {Backgorund}";
        }

        public static ColorScheme BlackWhite=> new ColorScheme(ConsoleColor.Black, ConsoleColor.White);
    }

    public class ConsoleFormatter : CustomFormatter
    {
        private readonly string[] _colors;
        public ConsoleFormatter()
        {
            _colors = Enum.GetNames(typeof(ConsoleColor)).Select(v => v.ToLower()).ToArray();
        }

        protected override string CustomFormat(string format, object arg)
        {
            //$"la-la {"message":-f red -b gray} abcd {123:-b blue}";
            //transform formattable string into string:

            //$$-f color -b color{{colored-text}}
            //"la-la $$-f red -b gray[[message]] abcd $$-b blu[[123]]";
            //regex pattern: \$\$(?<scheme>-.*?){{(?<text>.*?)}}

            var str = arg.ToString();
            if (ColorScheme.TryParse(format, out ColorScheme scheme))
            {
                str = "$$" + scheme + "{{" + arg + "}}";
            }
            return str;
        }

        protected override bool Match(string format)
        {
            var test = format.ToLower();
            if (format.Contains(" "))
            {
                var parts = format.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 || parts.Length ==4)
                    test = parts[1].ToLower();
            }
            return _colors.Contains(test);
        }
    }

    public abstract class CustomFormatter : ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string result = null;
            if (string.IsNullOrEmpty(format))
            {
                result = arg.ToString();
            }
            else if (Match(format))
            {
                result = CustomFormat(format, arg);
            }
            else
            {
                result = DefaultFormat(format, arg);
            }
            return result;

        }

        protected abstract string CustomFormat(string format, object arg);

        protected abstract bool Match(string format);

        protected virtual string DefaultFormat(string format, object arg)
        {
            return string.Format("{0:" + format + "}", arg);
        }

       
    }
}
