using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public interface IWebClient
    {
        string LastResponse { get; }
        HttpWebRequest CreateRequest(string command, RequestData data);
        string GetResponse(HttpWebRequest createRequest);
        Task<string> GetResponseAsync(HttpWebRequest request);
        void Validate();
    }
}