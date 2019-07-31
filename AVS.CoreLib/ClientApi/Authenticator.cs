using System.Security.Cryptography;
using System.Text;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Utils;
using AVS.CoreLib._System.Net.WebSockets;

namespace AVS.CoreLib.ClientApi
{
    public class Authenticator
    {
        public Encoding Encoding = Encoding.ASCII;
        public HMACSHA512 Encryptor { get; } = new HMACSHA512();
        public string PublicKey { get; private set; }
        
        public Authenticator(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            Encryptor.Key = Encoding.GetBytes(privateKey);
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            Encryptor.Key = Encoding.GetBytes(privateKey);
        }

        public byte[] GetBytes(string postData, out string signature)
        {
            byte[] postBytes = Encoding.GetBytes(postData);
            signature = Encryptor.ComputeHash(postBytes).ToStringHex();
            return postBytes;
        }

        public string Sign(string message)
        {
            byte[] postBytes = Encoding.GetBytes(message);
            return Encryptor.ComputeHash(postBytes).ToStringHex();
        }
    }

    public static class AuthenticatorExtensions
    {
        public static void Sign(this Authenticator authenticator, PrivateChannelCommand cmd)
        {
            cmd.Payload = $"nonce={NonceHelper.GetNonce()}";
            cmd.Key = authenticator.PublicKey;
            cmd.Signature = authenticator.Sign(cmd.Payload);
        }
    }
}