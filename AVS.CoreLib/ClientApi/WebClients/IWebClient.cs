using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.ClientApi.WebClients
{
    public interface IWebClient
    {
        string LastRequestedUrl { get; }
        string Execute(string method, string apiVersion, string command, RequestData data);
        Task<string> ExecuteAsync(string method, string apiVersion, string command, RequestData data);
        Task<string> QueryAsync(string method, string url);
    }
}