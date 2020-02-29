using System.Net;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public class ClientOptions
    {
        public string BaseAddress { get; set; }
        public string ApiVersion { get; set; }
        public string RelativeUrl { get; set; }

        public string DefaultVerb { get; set; } = "GET";
        public bool AddCommandArg { get; set; }
        /// <summary>
        /// Proxy = ProxyHelper.GetWebProxy();
        /// </summary>
        public IWebProxy Proxy { get; private set; }
        public string GetBaseUrl(string command, string apiVersion)
        {
            if (apiVersion != null)
            {
                if (apiVersion.StartsWith("https"))
                {
                    return $"{apiVersion}{command}";
                }
                return $"{BaseAddress}{apiVersion}{RelativeUrl}{command}";
            }
            return $"{BaseAddress}{ApiVersion}{RelativeUrl}{command}";
        }

        public override string ToString()
        {
            return $"ClientOptions: {BaseAddress} {ApiVersion} {RelativeUrl} {(Proxy!=null? Proxy.ToString(): "")}";
        }
    }
}