using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.ConsoleTools.Commands
{
    public interface IConsoleCommand
    {
        string Name { get; }
        string Description { get; }
        bool Match(string command);
        void Execute(string command, IDictionary<string, string> args);
        string GetUsage();
        string GetOptionsDescription();
        ConsoleKey HotKey { get; }
    }

    public abstract class ConsoleCommand : IConsoleCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string[] Shortcuts { get; }

        public virtual ConsoleKey HotKey { get; }

        public virtual bool Match(string command)
        {
            return Shortcuts.Contains(command);
        }

        public abstract void Execute(string command, IDictionary<string, string> args);

        public virtual string GetUsage()
        {
            return string.Join(" or ", Shortcuts);
        }

        public virtual string GetOptionsDescription()
        {
            return null;
        }
    }
}