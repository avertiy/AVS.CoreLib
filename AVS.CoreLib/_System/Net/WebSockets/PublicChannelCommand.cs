using System.Runtime.Serialization;
using AVS.CoreLib.ClientApi;
using AVS.CoreLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AVS.CoreLib._System.Net.WebSockets
{
    public interface IChannelCommand
    {
        string ToJsonMessage();
    }

    [DataContract]
    public class PublicChannelCommand: IChannelCommand
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember(Name = "command")]
        public CommandType Command { get; set; }

        [DataMember(Name = "channel")]
        public int Channel { get; set; }

        [DataMember(Name = "id")]
        public int? Id { get; set; }

        public virtual string ToJsonMessage()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}