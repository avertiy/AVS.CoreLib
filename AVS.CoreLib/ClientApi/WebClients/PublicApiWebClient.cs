using System.Net;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi
{
    public class PublicApiWebClient : BaseWebClient, IWebClient
    {
        public string LastUrl { get; set; }

        public HttpWebRequest CreateRequest(string command, RequestData data)
        {
            var url = CreateHttpGetString(GetUrl(command), data);
            return CreateHttpWebRequest("GET", url);
        }

        private string CreateHttpGetString(string url, RequestData data)
        {
            var res = url;
            var queryString = data.ToHttpQueryString();
            if (queryString.Length > 0)
            {
                res += (url.Contains("?") ? "&" : "?");
                res += queryString;
            }
            return res;
        }
    }
}