using System.Runtime.Serialization;

namespace AVS.CoreLib._System.Net.WebSockets
{
    [DataContract]
    public class PrivateChannelCommand: PublicChannelCommand
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }
        [DataMember(Name = "payload")]
        public string Payload { get; set; }
        [DataMember(Name = "sign")]
        public string Signature { get; set; }
    }
}