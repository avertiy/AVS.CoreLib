using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Threading;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Installation;
using AVS.CoreLib.Services.Tasks;
using AVS.CoreLib.Services.Tasks.AppTasks;
using AVS.CoreLib._System.Net;
using AVS.CoreLib._System.Net.Proxy;

namespace AVS.CoreLib.Utils
{
    public class Bootstrap
    {
        private Bootstrap() { }
        private AppConfig _config;
        private bool _engineContextInitialized = false;
        private Dictionary<string, string> _webapihosts;

        public void SetupCulture(string culture = "en")
        {
            //ensure culture is en
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        }

        public void InitConfig<T>() where T : AppConfig
        {
            _config = (T)ConfigurationManager.GetSection("AppConfig");

            if (_config == null)
            {
                throw new ApplicationException("Application config is missing");
            }
        }

        public void InstallScheduledTasks(bool clearData = false, bool reinitialize = true)
        {
            if (!_engineContextInitialized)
                InitializeEngineContext();
            var installation = EngineContext.Current.Resolve<IInstallationService>();
            if (clearData)
                installation.ClearData();
            installation.InstallScheduledTasks(reinitialize);
        }

        public void InitializeEngineContext(bool forceRecreate = false)
        {
            //startup tasks are executed here, incl. DbContext initialization (EfStartupTask)
            EngineContext.Initialize(forceRecreate, _config);
        }

        public void UseVpn()
        {
            VpnConnectionTask.IsVpnRequired = true;

            Console.WriteLine($"Sending ping to {_config.Vpn.TestAddress}..");

            if (!VpnUtil.TestConnection(_config.Vpn.TestAddress))
            {
                ConsoleExt.ClearLine();
                Console.WriteLine($"Establishing connection with {_config.Vpn.Name} VPN");
                if (VpnUtil.Connect(_config.Vpn.Name, _config.Vpn.Username, _config.Vpn.Password))
                {
                    ConsoleExt.ClearLine();
                    Console.WriteLine($"VPN {_config.Vpn.Name} is connected");
                }
                Console.WriteLine($"VPN {_config.Vpn.Name} connection failed");
            }
            else
            {
                Console.WriteLine($"{_config.Vpn.Name} VPN connection is OK");
            }
        }

        public void AddWebApiHost(string name, string url)
        {
            if (_webapihosts == null)
                _webapihosts = new Dictionary<string, string>();
            _webapihosts.Add(name, url);
        }

        public void TestWebApiHosts(bool useVpn = true)
        {
            if (_webapihosts == null || _webapihosts.Count == 0)
                return;

            try
            {
                //send test requests to web api hosts without VPN connection
                SendTestRequests();
            }
            catch (ApplicationException ex)
            {
                if (useVpn)
                {
                    UseVpn();
                    SendTestRequests();
                }
                else
                {
                    throw;
                }
            }
        }

        public void StartTaskManager()
        {
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
            Console.WriteLine("Task Manager has been started.");
            Console.WriteLine(TaskManager.Instance.ToString());
        }

        #region private methods

        private void SendTestRequests()
        {
            //ProxyHelper.DontUseProxy = true;
            foreach (var kp in _webapihosts)
            {
                try
                {
                    var content = ProxyHelper.SendTestWebRequest(kp.Value, useProxy: false);
                    Console.WriteLine($"{kp.Key} test request is OK");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"{kp.Key} is not accessible\r\n{0} \r\n", ex);
                }
            }
        }

        #endregion

        public static void Run(Action<Bootstrap> configure)
        {
            var bootstrap = new Bootstrap();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Starting application..");
            VpnConnectionTask.IsVpnRequired = false;
            try
            {
                configure(bootstrap);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program unable to start");
                ConsoleExt.WriteException(ex, stackTrace: true);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
    }
}