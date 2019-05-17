using System;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    public static class ConsoleExt
    {
        public static void SetRedColor()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        public static void SetDarkGrayColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public static void SetGrayColor()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void SetDefaultColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        public static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void WriteException(Exception ex, bool stackTrace)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Environment.NewLine + "  "+ ex.GetType().Name +": ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(ex.Message + Environment.NewLine);
            if (stackTrace)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(ex.StackTrace);
            }

            Console.ForegroundColor = color;
        }
    }
}