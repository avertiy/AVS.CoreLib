using System;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json.Converters
{
    public class ResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Response).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            object result = null;
            if (token.Type == JTokenType.Object)
            {
                result = ParseObject((JObject)token, objectType, serializer);
            }
            else
            {
                throw new JsonReaderException("JTokenType.Object token is expected");
            }

            return result;
        }
        
        protected virtual object CreateInstance(JObject jObject, Type objectType)
        {
            return Activator.CreateInstance(objectType);
        }

        protected virtual void Populate(JObject jObject, object target, JsonSerializer serializer)
        {
            JToken token = jObject["error"] ?? jObject["Error"];
            if (token != null)
            {
                ((Response)target).Error = token.Value<string>();
            }

            if(string.IsNullOrEmpty(((Response)target).Error))
            {
                serializer.Populate(jObject.CreateReader(), target);
            }
        }
        
        protected virtual object ParseObject(JObject jObject, Type objectType, JsonSerializer serializer)
        {
            var instance = CreateInstance(jObject, objectType);
            Populate(jObject, instance, serializer);
            return instance;
        }
    }
}