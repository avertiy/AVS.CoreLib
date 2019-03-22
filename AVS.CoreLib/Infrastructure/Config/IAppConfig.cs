using System;
using System.Configuration;
using System.Xml;

namespace AVS.CoreLib.Infrastructure.Config
{
    public interface IAppConfig: IConfigurationSectionHandler
    {
        bool IgnoreStartupTasks { get; }
        bool IgnoreAutoMapper { get; }
        bool NoDatabase { get; }
        AppConfig.AppInstanceNode AppInstance { get; }
        AppConfig.TaskNode Tasks { get; }
        AppConfig.VpnNode Vpn { get; }
    }


    /// <summary>
    /// Configuratuion section handler 
    /// </summary>
    public class AppConfig : XmlConfigNodeBase, IAppConfig
    {
        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; protected set; }
        public bool IgnoreAutoMapper { get; protected set; }
        public bool NoDatabase { get; protected set; }
        
        public TaskNode Tasks { get; protected set; }
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
                Tasks = new TaskNode(tasksNode);

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
                Name = GetString(node, "name");
                Username = GetString(node, "username");
                Password = GetString(node, "password");
                TestAddress = GetString(node, "testAddress");
            }
        }

        public class TaskNode: XmlConfigNodeBase
        {
            public string InstallForAppInstanceId { get; protected internal set; }
            public bool Enabled { get; protected internal set; }
            public bool Install { get; protected internal set; }
            public bool DetailedLogging { get; set; }
            /// <summary>
            /// system messages like start / stop time etc.
            /// </summary>
            public bool SystemLogging { get; set; }

            public TaskNode(XmlNode node)
            {
                Enabled = GetBool(node, "enabled");
                Install = GetBool(node, "install");
                DetailedLogging = GetBool(node, "detailed-logging");
                SystemLogging = GetBool(node, "system-logging");
                InstallForAppInstanceId = GetString(node, "install-for");
            }
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