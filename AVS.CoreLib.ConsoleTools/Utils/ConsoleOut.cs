using System;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    public static class ConsoleOut
    {
        public static ColorScheme Default { get; set; }

        static ConsoleOut()
        {
            Default = new ColorScheme
            {
                Foreground = Console.ForegroundColor,
                Backgorund = Console.BackgroundColor
            };
        }

        public static void SetDefaultColors()
        {
            Apply(Default);
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void PrintF(FormattableString str, bool endLine = true)
        {
            PrintF((ConsoleFormattedString) str, endLine);
        }

        public static void PrintF(ConsoleFormattedString str, bool endLine = true)
        {
            foreach ((string, ColorScheme, string) tuple in str)
            {
                if (!string.IsNullOrEmpty(tuple.Item1))
                {
                    Apply(Default);
                    Console.Write(tuple.Item1);
                }
                if(tuple.Item2 != null)
                    Apply(tuple.Item2);

                if (!string.IsNullOrEmpty(tuple.Item3))
                    Console.Write(tuple.Item3);
            }
            if(endLine)
                Console.WriteLine();
            Apply(Default);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void Print(string str, bool endLine = true)
        {
            if(endLine)
                Console.WriteLine(str);
            else
                Console.Write(str);
        }

        public static void Print(string str, ConsoleColor color, bool endLine = true)
        {
            Console.ForegroundColor = color;
            if (endLine)
                Console.WriteLine(str);
            else
                Console.Write(str);
            SetDefaultColors();
        }

        public static void Apply(ColorScheme scheme)
        {
            Console.ForegroundColor = scheme.Foreground;
            Console.BackgroundColor = scheme.Backgorund;
        }
    }
}