using System.Security.Cryptography;
using AVS.CoreLib.ClientApi.WebClients;
using AVS.CoreLib.Json;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi
{
    public abstract class BaseApiClient
    {
        public PrivateApiWebClient PrivateApiClient { get; protected set; }
        public PublicApiWebClient PublicApiClient { get; protected set; }
        
        protected BaseApiClient(string publicApiKey, string privateApiKey)
        {
            PublicApiClient = new PublicApiWebClient();
            PrivateApiClient = new PrivateApiWebClient(publicApiKey, privateApiKey);
        }

        protected BaseApiClient(string publicApiKey, KeyedHashAlgorithm algorithm)
        {
            PublicApiClient = new PublicApiWebClient();
            PrivateApiClient = new PrivateApiWebClient(publicApiKey, algorithm);
        }

        public JsonResponseResult ExecutePrivateCommand(string command, params string[] arguments)
        {
            var jsonText = PrivateApiClient.Execute("GET", null, command, RequestData.Create(arguments));
            return new JsonResponseResult() { JsonText = jsonText };
        }
    }
}