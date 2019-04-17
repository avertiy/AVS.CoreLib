using System;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Logging.LogWriters;
using AVS.CoreLib._System.Net;

namespace AVS.CoreLib.Services.Tasks.AppTasks
{
    public class VpnConnectionTask : TaskBase
    {
        public static ScheduleTask DefaultScheduleTask
        {
            get
            {
                var task = new ScheduleTask
                {
                    Name = "VPN Connection",
                    Description = "Ensures VPN connection is alive",
                    Seconds = 300,
                    Enabled = true,
                    StopOnError = false,
                    Type = typeof(VpnConnectionTask).AssemblyQualifiedName
                };
                return task;
            }
        }

        public static bool IsVpnRequired = true;
        protected string Name;
        protected string Username;
        protected string Password;
        protected string TestAddress;

        public VpnConnectionTask(IAppConfig appConfig) 
        {
            if (appConfig.Vpn != null)
            {
                Name = appConfig.Vpn.Name;
                Username = appConfig.Vpn.Username;
                Password = appConfig.Vpn.Password;
                TestAddress = appConfig.Vpn.TestAddress;
            }
        }

        public override void Execute(TaskLogWriter log)
        {
            if (!IsVpnRequired)
            {
                log.WriteDetails("VPN is not required");
                return;
            }
            if (string.IsNullOrEmpty(TestAddress))
            {
                log.WriteFail("VPN TestAddress is missing");
                return;
            }

            if (VpnUtil.TestConnection(TestAddress))
            {
                log.Write("VPN connection is OK");
                return;
            }

            if (string.IsNullOrEmpty(Name))
            {
                log.WriteWarning("VPN connection name is missing");
                return;
            }
            if (string.IsNullOrEmpty(Username))
            {
                log.WriteWarning("VPN connection username is missing");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                log.WriteWarning("VPN connection password is missing");
                return;
            }

            if (VpnUtil.Connect(Name, Username, Password))
                log.Write("VPN connection is OK");
            else
                log.WriteFail($"Unable to connect to {Name} VPN");
        }
    }
}