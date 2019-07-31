using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.ClientApi.WebClients;
using AVS.CoreLib.Json;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi
{
    public struct Command
    {
        private readonly string _command;
        public IWebClient Client { get; set; }

        public Command(string command, IWebClient client)
        {
            _command = command;
            Client = client;
        }

        public JsonResponseResult Execute(string[] arguments)
        {
            var data = new RequestData();
            data.Add(arguments);
            var jsonText = Client.GetResponse(CreateRequest(data));
            return new JsonResponseResult() { JsonText = jsonText };
        }

        public T Execute<T>(RequestData postData)
        {
            var jsonText = Client.GetResponse(CreateRequest(postData));
            if (string.IsNullOrEmpty(jsonText))
            {
                return default(T);
            }

            return new JsonResponseResult(jsonText).Deserialize<T>();
        }

        public JsonResponseResult Execute(RequestData postData)
        {
            var request = CreateRequest(postData);
            var jsonText = Client.GetResponse(request);
            return new JsonResponseResult(jsonText);
        }
        
        public async Task<JsonResponseResult> ExecuteAsync(RequestData data)
        {
            int attempt = 0;
            start:
            var jsonText = await Client.GetResponseAsync(CreateRequest(data));
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }
            return new JsonResponseResult() { JsonText = jsonText };
        }

        private HttpWebRequest CreateRequest(RequestData data)
        {
            return Client.CreateRequest(_command, data);
        }
    }
  
}