using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using AVS.CoreLib.Services.Tasks.AppTasks;

namespace AVS.CoreLib.Infrastructure.Config
{
    public interface IAppConfig: IConfigurationSectionHandler
    {
        bool IgnoreStartupTasks { get; }
        bool IgnoreAutoMapper { get; }
        bool NoDatabase { get; }
        AppConfig.AppInstanceNode AppInstance { get; }
        AppConfig.TasksNode Tasks { get; }
        AppConfig.VpnNode Vpn { get; }
    }


    /// <summary>
    /// Configuratuion section handler 
    /// </summary>
    public partial class AppConfig : XmlConfigNodeBase, IAppConfig
    {
        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; protected set; }
        public bool IgnoreAutoMapper { get; protected set; }
        public bool NoDatabase { get; protected set; }
        
        public TasksNode Tasks { get; protected set; }
        public VpnNode Vpn { get; protected set; }
        public AppInstanceNode AppInstance { get; protected set; }

        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            Initialize(configContext,section);
            return this;
        }

        protected virtual void Initialize(object configContext, XmlNode section)
        {
            XmlNode startupNode = section.SelectSingleNode("Startup");
            IgnoreStartupTasks = GetBool(startupNode, "ignoreStartupTasks");
            IgnoreAutoMapper = GetBool(startupNode, "ignoreAutoMapper");
            NoDatabase = GetBool(startupNode, "noDatabase");

            var vpnNode = section.SelectSingleNode("VPN");
            if (vpnNode != null)
                Vpn = new VpnNode(vpnNode);

            var tasksNode = section.SelectSingleNode("Tasks");
            if(tasksNode!=null)
                Tasks = new TasksNode(tasksNode);

            var appInstanceNode = section.SelectSingleNode("AppInstance");
            AppInstance = new AppInstanceNode(appInstanceNode);
        }

        public class VpnNode : XmlConfigNodeBase
        {
            public string Name { get; protected internal set; }
            public string Username { get; protected internal set; }
            public string Password { get; protected internal set; }
            public string TestAddress { get; protected internal set; }
            public VpnNode(XmlNode node)
            {
                Name = GetString(node, "name", true);
                Username = GetString(node, "username", true);
                Password = GetString(node, "password", true);
                TestAddress = GetString(node, "testAddress", true);
            }
        }

        public class TasksNode: XmlConfigNodeBase
        {
            public string InstallForAppInstanceId { get; protected set; }
            public bool Enabled { get; protected set; }
            public bool Install { get; protected set; }
            public bool DetailedLogging { get; set; }

            public List<TaskNode> Tasks { get; protected set; }

            /// <summary>
            /// system messages like start / stop time etc.
            /// </summary>
            public bool SystemLogging { get; set; }

            public TasksNode(XmlNode node)
            {
                Enabled = GetBool(node, "enabled");
                Install = GetBool(node, "install");
                DetailedLogging = GetBool(node, "detailed-logging");
                SystemLogging = GetBool(node, "system-logging");
                InstallForAppInstanceId = GetString(node, "install-for");
                
                XmlNodeList nodes = node?.SelectNodes("Task");
                if (nodes == null)
                    return;
                Tasks = new List<TaskNode>(nodes.Count);
                foreach (XmlNode taskNode in nodes)
                {
                    Tasks.Add(new TaskNode(taskNode));
                }
            }

            public TaskNode GetTaskByType(Type type)
            {
                var task = Tasks.FirstOrDefault(t => t.Type == type.Name || t.Type == type.FullName);
                if(task ==null)
                    throw new ConfigurationErrorsException($"Missing Task configuration [ensure <Task type='{type.Name}'/> exists]");
                return task;
            }

            public TaskNode this[Type type]=> GetTaskByType(type);
        }

        public class AppInstanceNode : XmlConfigNodeBase
        {
            public string Id { get; protected internal set; }

            public AppInstanceNode(XmlNode node)
            {
                Id = GetString(node, "id") ?? Environment.MachineName + ":" + System.AppDomain.CurrentDomain.FriendlyName;
            }
        }
    }
}