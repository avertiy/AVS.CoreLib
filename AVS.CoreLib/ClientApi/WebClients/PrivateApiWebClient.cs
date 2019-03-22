using System.Net;
using System.Text;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi
{
    public class PrivateApiWebClient: BaseWebClient, IWebClient
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
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            Authenticator.SwitchKeys(publicKey, privateKey);
        }

        public HttpWebRequest CreateRequest(string command, RequestData data)
        {
            data.Add("command", command);
            data.Add("nonce", NonceHelper.GetNonce());
            return CreatePostRequest(GetUrl(command), data.ToHttpQueryString());
        }
        
        protected HttpWebRequest CreatePostRequest(string url, string postData)
        {
            var request = CreateHttpWebRequest("POST", url);
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Authenticator.GetBytes(postData, out string signature);
            request.ContentLength = bytes.Length;
            request.Headers["Key"] = Authenticator.PublicKey;
            request.Headers["Sign"] = signature;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            return request;
        }
    }
}