using System;
using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public class PublicApiWebClient : IWebClient
    {
        public string LastRequestedUrl { get; protected set; }
        public ClientOptions Options = new ClientOptions();
        public int TotalRequestsMade { get; protected set; }

        public Task<string> QueryAsync(string method, string url)
        {
            var request = WebRequestHelper.ConstructHttpWebRequest(method, url);
            request.Proxy = Options.Proxy;
            return request.FetchResponseAsync();
        }

        public virtual string Execute(string method, string apiVersion, string command, RequestData data)
        {
            var request = CreateRequest(method, apiVersion, command, data);
            TotalRequestsMade++;
            return request.FetchResponse();
        }

        public Task<string> ExecuteAsync(string method, string apiVersion, string command, RequestData data)
        {
            var request = CreateRequest(method, apiVersion, command, data);
            return request.FetchResponseAsync();
        }

        protected HttpWebRequest CreateRequest(string method, string apiVersion, string command, RequestData data)
        {
            method = method ?? Options.DefaultVerb;
            var url = Options.GetBaseUrl(command, apiVersion);
            OnRequestCreating(url, data, command, method);

            if(method == "GET")
                url = UrlHelper.Combine(url, data.ToHttpQueryString());
            var request = WebRequestHelper.ConstructHttpWebRequest(method, url);
            if (Options.Proxy != null)
                request.Proxy = Options.Proxy;
            LastRequestedUrl = url;
            OnRequestCreated(request, data);
            return request;
        }

        protected virtual void OnRequestCreating(string url, RequestData data, string command, string method)
        {
        }

        protected virtual void OnRequestCreated(HttpWebRequest request, RequestData data)
        {
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {Options.ToString()}";
        }

        public void Validate()
        {
            if(string.IsNullOrEmpty(Options.BaseAddress))
                throw new Exception($"{this.GetType().Name} options are not set. BaseAddress is required");
        }
    }
}