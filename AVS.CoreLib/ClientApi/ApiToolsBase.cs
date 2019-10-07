using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AVS.CoreLib.ClientApi.WebClients;
using AVS.CoreLib.Json;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi
{
    public abstract class ApiToolsBase
    {
        private readonly IWebClient _client;
        protected string LastRequestedUrl;
        protected ApiToolsBase(IWebClient client)
        {
            _client = client;
        }

        protected virtual JsonResponseResult Execute(string command, RequestData data, string apiVersion = null, string method = null)
        {
            int attempt = 0;

            start:
            var jsonText = _client.Execute(method, apiVersion, command, data);
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }

            LastRequestedUrl = _client.LastRequestedUrl;
            Debug.WriteLine($"{method} {_client.LastRequestedUrl}=> {jsonText}");
            return new JsonResponseResult() { JsonText = jsonText };
        }

        protected virtual async Task<JsonResponseResult> ExecuteAsync(string command, RequestData data, string apiVersion = null, string method = null)
        {
            int attempt = 0;

            start:
            var jsonText = await _client.ExecuteAsync(method, apiVersion, command, data).ConfigureAwait(false);
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }

            LastRequestedUrl = _client.LastRequestedUrl;
            Debug.WriteLine("LastRequestedUrl: {0}", _client.LastRequestedUrl);
            return new JsonResponseResult() { JsonText = jsonText };
        }

        protected virtual async Task<JsonResponseResult> QueryAsync(string url, string method = null)
        {
            int attempt = 0;
            start:
            var jsonText = await _client.QueryAsync(method, url).ConfigureAwait(false);
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }
            return new JsonResponseResult() { JsonText = jsonText };
        }
    }
    /*
    public abstract class ApiToolsBase
    {
        /// <summary>
        /// returns JSON from the last response
        /// </summary>
        protected string LastRequestedUrl => WebClient.LastRequestedUrl;
        protected IWebClient WebClient { get; set; }

        protected ApiToolsBase(IWebClient webClient)
        {
            WebClient = webClient;
        }

        public virtual JsonResponseResult ExecuteCommand(string command, params string[] arguments)
        {
            var cmd = new Command(command, WebClient);
            return cmd.Execute(arguments);
        }

        protected virtual JsonResponseResult ExecuteCommand(string command, Dictionary<string, object> postData)
        {
            var cmd = new Command(command, WebClient);
            return cmd.Execute(postData);
        }

        protected virtual Task<JsonResponseResult> ExecuteCommandAsync(string command, Dictionary<string, object> postData)
        {
            var cmd = new Command(command, WebClient);
            return cmd.ExecuteAsync(postData);
        }

        [Obsolete]
        protected virtual Command CreateCommand(string command)
        {
            return new Command(command, WebClient);
        }

        protected virtual Command Command(string command)
        {
            return new Command(command, WebClient);
        }

        [Obsolete]
        protected virtual T PostData<T>(string command, Dictionary<string, object> postData)
        {
            var cmd = new Command(command, WebClient);
            return cmd.Execute<T>(postData);
        }

        [Obsolete]
        protected virtual T GetData<T>(string command, params object[] parameters)
        {
            var cmd = new Command(command, WebClient);
            return cmd.Execute<T>(parameters);
        }
    }
    */
}