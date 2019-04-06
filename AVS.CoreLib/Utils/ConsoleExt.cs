using System;

namespace AVS.CoreLib.Utils
{
    public static class ConsoleExt
    {
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
            Console.WriteLine(ex.Message);
            if (stackTrace)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(ex.StackTrace);
            }

            Console.ForegroundColor = color;
        }
    }
}