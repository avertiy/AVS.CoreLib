using System;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    public class WindowsService : ServiceBase
    {
        protected readonly Action<BootstrapAsService> Configuration;
        protected Action StopCallback;
        public WindowsService(string serviceName, Action<BootstrapAsService> configuration)
        {
            ServiceName = serviceName;
            Configuration = configuration;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var x = new BootstrapAsService();
                Configuration(x);
                StopCallback = x.StopCallback;
                File.AppendAllText($"c:\\temp\\{ServiceName}.txt", $"{DateTime.Now} started{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{DateTime.Now} unable to start service {ex.GetType().Name} has occured: {ex.Message}");
                sb.AppendLine(ex.StackTrace);
                File.AppendAllText($"c:\\temp\\{ServiceName}.txt", sb.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                StopCallback?.Invoke();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{DateTime.Now} when stopping service unhandled {ex.GetType().Name} has occured: {ex.Message}");
                sb.AppendLine(ex.StackTrace);
                File.AppendAllText($"c:\\temp\\{ServiceName}.txt", sb.ToString());
            }
            File.AppendAllText($"c:\\temp\\{ServiceName}.txt", $"{DateTime.Now} stopped{Environment.NewLine}");
        }
    }
}