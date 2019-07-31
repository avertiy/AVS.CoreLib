using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.ConsoleTools.Utils;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ConsoleTools.Commands
{
    class HelpCommand : ConsoleCommand
    {
        private readonly ICommandStore _commandStore;
        public override string Name => "help";
        public override string Description => "prints help";
        public override string[] Shortcuts => new[] {"/?", "/help"};
        public override ConsoleKey HotKey => ConsoleKey.F1;

        public HelpCommand(ICommandStore commandStore)
        {
            _commandStore = commandStore;
        }

        public override void Execute(string command, IDictionary<string, string> args)
        {
            using (var locker = ConsoleLocker.Create())
            {
                ConsoleOut.Apply(ColorScheme.BlackWhite);
                Console.WriteLine();
                foreach (var cmd in _commandStore.GetAllCommands())
                {
                    ConsoleOut.PrintF($"\r\nCommand: {cmd.Name,-10:DarkYellow} - {cmd.Description}");
                    if (cmd.HotKey != 0)
                    {
                        ConsoleOut.PrintF($"HotKey:  {cmd.HotKey.ToString():Blue}");
                    }

                    ConsoleOut.PrintF($" usage:  {cmd.GetUsage():Yellow}");

                    var options = cmd.GetOptionsDescription();
                    if (!string.IsNullOrEmpty(options))
                    {
                        Console.WriteLine($" options:");
                        Console.WriteLine(options);
                    }
                }
                Console.WriteLine($"Press any key to continue..");
                ConsoleOut.SetDefaultColors();
                Console.ReadKey(true);
            }
        }

        public string GetUsage()
        {
            return "/help or /?  - prints help";
        }

        public string GetOptionsDescription()
        {
            return null;
        }

        
    }
}