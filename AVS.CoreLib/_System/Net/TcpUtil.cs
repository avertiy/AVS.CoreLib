using System;
using System.Net.Sockets;

namespace AVS.CoreLib._System.Net
{
    public static class TcpUtil
    {
        public static bool PingHost(string host, int port = 80)
        {
            try
            {
                var client = new TcpClient();
                if (client.ConnectAsync(host, port).Wait(1000))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}