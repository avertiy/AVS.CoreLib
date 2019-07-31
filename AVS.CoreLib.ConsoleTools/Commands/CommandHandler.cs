using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AVS.CoreLib.ConsoleTools.Bootstraping;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ConsoleTools.Commands
{
    interface ICommandStore
    {
        IList<IConsoleCommand> GetAllCommands();
    }

    class CommandHandler : ICommandStore
    {
        private List<IConsoleCommand> Commands { get; set; }

        public CommandHandler()
        {
            Commands = new List<IConsoleCommand>
            {
                new HelpCommand(this),
                new QuitCommand(),
                new ClearScreenCommand()
            };
        }

        public void AddCommand(IConsoleCommand cmd)
        {
            Commands.Add(cmd);
        }

        public virtual void HandleCommand(string input)
        {
            int ind = input.IndexOf(" ", StringComparison.Ordinal);
            string command = input;
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (ind > 0)
            {
                command = input.Substring(0, ind);
                args = ArgsParser.Parse(input.Substring(ind + 1));
            }
            foreach (var cmd in Commands)
            {
                if (cmd.Match(command))
                {
                    cmd.Execute(command, args);
                    return;
                }
            }

            Console.WriteLine("unknown command: " + input);
        }

        public IList<IConsoleCommand> GetAllCommands()
        {
            return Commands;
        }

        public bool MatchShortCut(ConsoleKeyInfo keyInfo)
        {
            foreach (var cmd in Commands)
            {
                if (cmd.HotKey == keyInfo.Key)
                {
                    cmd.Execute(cmd.Name, new ConcurrentDictionary<string, string>());
                    return true;
                }
            }

            return false;
        }
    }
}