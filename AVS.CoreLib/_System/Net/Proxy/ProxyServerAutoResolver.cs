using System;
using System.Collections.Generic;
using System.Configuration;

namespace AVS.CoreLib._System.Net.Proxy
{
    public class ProxyServerAutoResolver
    {
        public int Port { get; set; }
        private readonly List<string> _hosts = new List<string>();
        public void AddProxyHost(string host)
        {
            _hosts.Add(host);
        }

        public IProxyServerAddress GetAvailableProxyServer()
        {
            IProxyServerAddress result = null;

            if (_hosts.Count == 0)
                _hosts.AddRange(GetProxyHosts());

            if (Port == 0)
                Port = GetProxyPort();

            foreach (var host in _hosts)
            {
                if (TcpUtil.PingHost(host, Port))
                {
                    result = new ProxyServerAddress(host, Port);
                    break;
                }
            }

            return result;
        }

        private string[] GetProxyHosts()
        {
            var str = ConfigurationManager.AppSettings["proxy_servers"];
            if (string.IsNullOrEmpty(str))
            {
                str = "192.168.220.207, 52.57.3.222";
            }
            var hosts = str.Split(new[] { ", ", "; " }, StringSplitOptions.RemoveEmptyEntries);
            return hosts;
        }

        private int GetProxyPort()
        {
            var str = ConfigurationManager.AppSettings["proxy_port"];
            if (string.IsNullOrEmpty(str))
            {
                str = "8081";
            }
            return int.Parse(str);
        }

        private static ProxyServerAutoResolver _instance = null;

        public static ProxyServerAutoResolver Instance
        {
            get
            {
                _instance = _instance ?? new ProxyServerAutoResolver();
                return _instance;
            }
        }
    }
}