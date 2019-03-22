using System.Diagnostics;
using System.IO;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class ObjectProjection<TValue> : Projection 
    {
        [DebuggerStepThrough]
        public ObjectProjection(string jsonText) : base(jsonText)
        {
        }

        public T Map<T,TProjection>() 
            where T : Response<TValue>, new()
            where TProjection : TValue, new()
        {
            T response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]")
            {
                response = new T() { Data = new TProjection() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new T();

                    JToken token = JToken.Load(reader);

                    if (token.Type != JTokenType.Object)
                    {
                        throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                    }

                    var jObject = (JObject) token;
                    if (ContainsError(jObject, (Response) response))
                        return response;
                    response.Data = JsonHelper.ParseObject<TProjection>(jObject);
                }
            }
            return response;
        }

        public Response<TValue> Map<TProjection>() 
            where TProjection : TValue, new()
        {
            Response<TValue> response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "{}")
            {
                response = new Response<TValue>() { Data = new TProjection() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new Response<TValue>();

                    JToken token = JToken.Load(reader);

                    if (token.Type != JTokenType.Object)
                    {
                        throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                    }

                    var jObject = (JObject) token;
                    if (ContainsError(jObject, (Response) response))
                        return response;
                    response.Data = JsonHelper.ParseObject<TProjection>(jObject);
                }
            }
            return response;
        }
    }
}