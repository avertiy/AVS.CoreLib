using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVS.CoreLib.Json;

namespace AVS.CoreLib.ClientApi
{
    public abstract class ApiToolsBase
    {
        /// <summary>
        /// returns JSON from the last response
        /// </summary>
        protected string JsonResponse => WebClient.LastResponse;
        protected IWebClient WebClient { get; set; }

        protected ApiToolsBase(IWebClient webClient)
        {
            webClient.Validate();
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
}