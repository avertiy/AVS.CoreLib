using System;
using System.Collections.Generic;
using System.Xml;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.Infrastructure.Config
{
    public class TaskNode : XmlConfigNodeBase
    {
        public string Type { get; protected set; }
        public string Name { get; protected set; }
        public bool Enabled { get; protected set; }
        public bool StopOnError { get; protected set; }
        public int Seconds { get; protected set; }
        public LogLevel LogLevel { get; protected set; }

        /// <summary>
        /// Task might have some argumetns to execute with
        /// example args="-p1 abc -p2 123 -p3 abc/123"
        /// </summary>
        public string Parameters { get; protected set; }

        public TaskNode(XmlNode node)
        {
            Type = this.GetString(node, "type", true);
            Name = this.GetString(node, "name");
            Parameters = this.GetString(node, "args")?? this.GetString(node, "parameters");
            Enabled = this.GetBool(node, "enabled");
            LogLevel = Enum.TryParse<LogLevel>(this.GetString(node, "log-level"),out LogLevel lvl) ? lvl : LogLevel.INFO;
            StopOnError = this.GetBool(node, "stopOnError");
            Seconds = this.GetInt(node, "seconds");
        }

        public override string ToString()
        {
            return $"{Type} {Seconds} sec. {(Enabled?"enabled":"disabled")} args: {Parameters}";
        }
    }
}