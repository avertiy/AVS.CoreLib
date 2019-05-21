using System.Collections.Generic;
using System.Xml;
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

        /// <summary>
        /// Task might have some argumetns to execute with
        /// example args="-p1 abc -p2 123 -p3 abc/123"
        /// </summary>
        public string ParametersString { get; protected set; }

        public Dictionary<string, string> Parameters => ArgsParser.Parse(ParametersString);

        public TParameters GetParameters<TParameters>() where TParameters : ITaskParameters, new()
        {
            var args = new TParameters();
            args.Init(ArgsParser.Parse(ParametersString));
            return args;
        }

        public TaskNode(XmlNode node)
        {
            Type = this.GetString(node, "type", true);
            Name = this.GetString(node, "name");
            ParametersString = this.GetString(node, "args")?? this.GetString(node, "parameters");
            Enabled = this.GetBool(node, "enabled");
            StopOnError = this.GetBool(node, "stopOnError");
            Seconds = this.GetInt(node, "seconds");
        }
    }
}