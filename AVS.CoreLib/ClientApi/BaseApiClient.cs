using AVS.CoreLib.Json;

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

        public JsonResponseResult ExecutePrivateCommand(string command, params string[] arguments)
        {
            var cmd = new Command(command, PrivateApiClient);
            return cmd.Execute(arguments);
        }
    }
}