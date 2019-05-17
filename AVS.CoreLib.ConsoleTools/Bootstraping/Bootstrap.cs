using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using AVS.CoreLib.ConsoleTools.Utils;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Installation;
using AVS.CoreLib.Services.Tasks;
using AVS.CoreLib.Services.Tasks.AppTasks;
using AVS.CoreLib._System.Net;
using AVS.CoreLib._System.Net.Proxy;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    /// <summary>
    /// usage example: 
    /// Bootstrap.Run(b =>
    ///  {
    ///      b.AddWebApiHost("Poloniex API", "https://poloniex.com/public?command=returnTicker");
    ///      b.AddWebApiHost("Exmo API", "https://api.exmo.com/v1/ticker/");
    ///      b.TestWebApiHosts(false);
    ///      b.SetupCulture("en");
    ///      b.InitConfig<MyAppConfig>();
    ///      b.InitializeEngineContext(false);
    ///      b.InstallScheduledTasks(args.Length > 0 && args[0] == "-clear", true);
    ///      b.StartTaskManager();
    ///  });
    /// </summary>
    public class Bootstrap
    {
        internal Bootstrap() { }
        private AppConfig _config;
        private bool IsConsoleApp { get; set; }
        private IBootstrapLogger Logger { get; set; }

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
            _engineContextInitialized = true;
        }

        public void UseVpn()
        {
            VpnConnectionTask.IsVpnRequired = true;

            WriteLine($"Sending ping to {_config.Vpn.TestAddress}..");

            if (!VpnUtil.TestConnection(_config.Vpn.TestAddress))
            {
                ClearLine();
                WriteLine($"Establishing connection with {_config.Vpn.Name} VPN");
                if (VpnUtil.Connect(_config.Vpn.Name, _config.Vpn.Username, _config.Vpn.Password))
                {
                    ClearLine();
                    WriteLine($"VPN {_config.Vpn.Name} is connected");
                }
                WriteLine($"VPN {_config.Vpn.Name} connection failed");
            }
            else
            {
                WriteLine($"{_config.Vpn.Name} VPN connection is OK");
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
            WriteLine("Task Manager has been started.");
            WriteLine(TaskManager.Instance.ToString());
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
                    Logger.WriteLine($"{kp.Key} test request is OK");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"{kp.Key} is not accessible\r\n{0} \r\n", ex);
                }
            }
        }

        private void ClearLine()
        {
            Logger.ClearLine();
        }

        private void WriteLine(string text)
        {
            Logger.WriteLine(text);
        }

        #endregion

        internal static void Build(Action<Bootstrap> configuration, IBootstrapLogger logger)
        {
            Bootstrap bootstrap = new Bootstrap()
            {
                IsConsoleApp = Environment.UserInteractive,
                Logger = logger
            };
            

            VpnConnectionTask.IsVpnRequired = false;
            configuration(bootstrap);
        }

        public static void Run(string serviceName, Action<BootstrapAsService> configuration)
        {
            if (Environment.UserInteractive)
            {
                var x = new BootstrapAsService();
                configuration(x);
            }
            else
            {
                // running as service
                using (var service = new WindowsService(serviceName, configuration))
                {
                    ServiceBase.Run(service);
                }
            }
        }

        public static void Run(Action<Bootstrap> configuration)
        {
            ConsoleExt.SetDefaultColor();
            Console.WriteLine("Starting application..");
            try
            {
                Bootstrap.Build(configuration, new ConsoleWriter());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program unable to start");
                ConsoleExt.WriteException(ex, stackTrace: true);
            }
            ConsoleExt.SetGrayColor();
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }

        private static void Test()
        {
            Bootstrap.Run("myService", 
                x => x.OnStart(b =>
                    {
                        b.AddWebApiHost("Poloniex API", "https://poloniex.com/public?command=returnTicker");
                        b.AddWebApiHost("Exmo API", "https://api.exmo.com/v1/ticker/");
                        b.TestWebApiHosts(false);
                        b.SetupCulture("en");
                        //b.InitConfig<MyAppConfig>();
                        b.InitializeEngineContext(false);
                        //b.InstallScheduledTasks(args.Length > 0 && args[0] == "-clear", true);
                        b.StartTaskManager();
                    }
                ).OnStop(null));
            Bootstrap.Run(b => b.InitializeEngineContext());
        }
    }


    

    

    
}