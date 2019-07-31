using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.ConsoleTools.Utils;

namespace AVS.CoreLib.ConsoleTools.Commands
{
    class QuitCommand : ConsoleCommand
    {
        public override string Name => "quit";
        public override string Description => "quits the app";
        public override string[] Shortcuts => new[] { "/quit", "/q", "quit" };
        
        public override void Execute(string command, IDictionary<string, string> args)
        {
            ConsoleInput.ShouldQuit = true;
            return;
        }
    }

    class ClearScreenCommand : ConsoleCommand
    {
        public override string Name => "clear screen";
        public override string Description => "clears screen";
        public override string[] Shortcuts => new[] { "/clear", "clear" };
        public override ConsoleKey HotKey => ConsoleKey.F2;

        public override void Execute(string command, IDictionary<string, string> args)
        {
            Console.Clear();
            return;
        }
    }
}