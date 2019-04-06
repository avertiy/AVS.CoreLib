using System;
using System.IO;
using System.Net;
using System.Threading;

namespace AVS.CoreLib._System.Net.Proxy
{
    /// <summary>
    /// specify proxy servers list in app setting, the setting key is "proxy_servers" (hosts should be comma separated)
    /// specify proxy port, the setting key is "proxy_port"
    /// </summary>
    public static class ProxyHelper
    {
        /// <summary>
        /// When true GetWebProxy() will return null
        /// this is for performance, resolving AvailableProxyServer takes some time
        /// if no need in proxy - no need to resolve proxy server address
        /// Note: GetWebProxy() might be called by SendTestWebRequest
        /// </summary>
        public static bool DontUseProxy = false;

        public static bool AcceptAllCertifications = true;
        private static bool _proxyServerInitialized = false;
        public static IProxyServerAddress DefaultProxyServer = null;
        /// <summary>
        /// If address is null returns null
        /// if address is not null returns WebProxy and assings callback to accept All Certificates 
        /// on ServicePointManager.ServerCertificateValidationCallback
        /// </summary>
        public static WebProxy GetWebProxy(IProxyServerAddress addr)
        {
            if (addr != null)
            {
                if (AcceptAllCertifications)
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertificationsHandler);
                return new WebProxy(addr.Address);
            }
            return null;
        }

        public static WebProxy GetWebProxy()
        {
            if (DefaultProxyServer == null && !_proxyServerInitialized)
            {
                _proxyServerInitialized = true;
                if (!DontUseProxy)
                    DefaultProxyServer = ProxyServerAutoResolver.Instance.GetAvailableProxyServer();
            }
            return GetWebProxy(DefaultProxyServer);
        }

        public static string SendTestWebRequest(string url = "https://www.google.com/", bool useProxy = true)
        {
            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";
                request.UserAgent = "ProxyHelperUtil";
                request.Timeout = Timeout.Infinite;
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                if (useProxy)
                    request.Proxy = ProxyHelper.GetWebProxy();
                return request.GetResponseString();
            }
            catch (Exception ex)
            {
                if (useProxy)
                    throw new ApplicationException($"Request to {url} through proxy {ProxyHelper.DefaultProxyServer} FAILED", ex);
                else
                    throw new ApplicationException($"Request to {url} FAILED", ex);
            }
        }

        public static bool SendTestWebRequest(out string error, string url = "https://www.google.com/", bool useProxy = true)
        {
            try
            {
                var content = SendTestWebRequest(url, useProxy);
                error = null;
                return !string.IsNullOrEmpty(content);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
        }

        private static bool AcceptAllCertificationsHandler(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static string GetResponseString(this HttpWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}