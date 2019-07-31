using System.Runtime.Serialization;

namespace AVS.CoreLib._System.Net.WebSockets
{
    public enum CommandType
    {
        //[EnumMember(Value = "private")]
        //Private,
        [EnumMember(Value = "unsubscribe")]
        Unsubscribe,
        [EnumMember(Value = "subscribe")]
        Subscribe
    }
}