using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.CoreLib._System.Net.Proxy
{
    public interface IProxyServerAddress
    {
        string Address { get; }
    }

    public abstract class BaseProxyServerAddress : IProxyServerAddress
    {
        protected int Port = 8081;
        protected abstract string Host { get; }
        public string Address => Host + ":" + Port;

        public override string ToString()
        {
            return $"{Address}";
        }
    }

    public class ProxyServerAddress : BaseProxyServerAddress
    {
        protected override string Host { get; }

        public ProxyServerAddress(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
