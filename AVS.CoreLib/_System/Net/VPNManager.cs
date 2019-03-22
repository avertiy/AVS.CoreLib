using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace AVS.CoreLib._System.Net
{
    public static class VpnUtil
    {
        public static bool TestConnection(string vpnAddress)
        {
            bool result = false;
            try
            {
                using (Ping ping = new Ping())
                {
                    var reply = ping.Send(vpnAddress);
                    if (reply != null && reply.Status == IPStatus.Success)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                global::System.Diagnostics.Debug.Write(ex.ToString(), "VPNManager");
            }
            return result;
        }

        public static bool Connect(string name, string username, string password)
        {
            bool result = false;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rasdial.exe {name} {username} {password}"
                };

                Process.Start(startInfo);
                global::System.Threading.Thread.Sleep(1000);
                result = true;
            }
            catch (Exception ex)
            {
                global::System.Diagnostics.Debug.Write(ex.ToString(), "VPNManager");
            }
            return result;
        }

        public static bool Disconnect(string name)
        {
            bool result = false;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rasdial.exe {name} /DISCONNECT"
                };

                Process.Start(startInfo);
                //Process.Start($"C:\\WINDOWS\\system32\\rasdial {name} /DISCONNECT");
                global::System.Threading.Thread.Sleep(4000);
                result = true;
            }
            catch (Exception ex)
            {
                global::System.Diagnostics.Debug.Write(ex.ToString(), "VPNManager");
            }
            return result;
        }
    }
}