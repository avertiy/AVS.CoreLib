using System;
using System.Collections.Generic;
using AVS.CoreLib.ConsoleTools.Commands;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    public static class ConsoleInput
    {
        private static readonly CommandHandler Handler = new CommandHandler();
        public static bool ShouldQuit { get; set; }
        public static event Action<string> Input = delegate { };
        public static event Action<ConsoleKeyInfo> KeyPress = delegate { };

        public static void Register(params IConsoleCommand[] commands)
        {
            foreach (var command in commands)
            {
                Handler.AddCommand(command);
            }
        }

        public static void WaitForInput()
        {
            do
            {
                Console.WriteLine("Press enter to quit.");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (keyInfo.Key == ConsoleKey.Tab)
                {
                    using (var locker = ConsoleLocker.Create())
                    {
                        WaitForCommand();
                        if (ShouldQuit)
                            break;
                    }
                }
            } while (true);
        }

        internal static ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept);
            KeyPress?.Invoke(keyInfo);
            return keyInfo;
        }

        private static void WaitForCommand()
        {
            ConsoleOut.PrintF($"\r\n{"type command or press F1 to get help or press Esc to escape":Green}\r\n$",false);

            var keyInfo = ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                ConsoleExt.ClearLine();
                return;
            }

            if (Handler.MatchShortCut(keyInfo))
            {
                ConsoleExt.ClearLine();
                return;
            }

            Console.Write(keyInfo.KeyChar);
            string input = keyInfo.KeyChar + Console.ReadLine();
            Handler.HandleCommand(input);
            Input?.Invoke(input);
        }
    }

   
}