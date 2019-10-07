using System.Net;
using System.Security.Cryptography;
using System.Text;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public class PrivateApiWebClient : PublicApiWebClient
    {
        protected Authenticator Authenticator { get; set; }
        public Encoding Encoding
        {
            get => Authenticator.Encoding;
            set => Authenticator.Encoding = value;
        }

        public PrivateApiWebClient(string publicKey, string privateKey)
        {
            Authenticator = new Authenticator(publicKey, privateKey);
            Options.DefaultVerb = "POST";
        }

        public PrivateApiWebClient(string publicKey, KeyedHashAlgorithm algorithm)
        {
            Authenticator = new Authenticator(publicKey, algorithm);
            Options.DefaultVerb = "POST";
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            Authenticator.SwitchKeys(publicKey, privateKey);
        }

        protected override void OnRequestCreating(string url, RequestData data, string command, string method)
        {
            if(Options.AddCommandArg)
                data.Add("command", command);
            data.Add("nonce", NonceHelper.GetNonce());
        }

        protected override void OnRequestCreated(HttpWebRequest request, RequestData data)
        {
            var bytes = Authenticator.GetBytes(data.ToHttpQueryString(), out string signature);
            request.Headers["Key"] = Authenticator.PublicKey;
            request.Headers["Sign"] = signature;
            //content data could be sent only with POST verb
            request.Method = "POST";
            request.WriteBytes(bytes);
        }
    }
}