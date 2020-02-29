using System;
using System.Diagnostics;
using System.IO;
using AVS.CoreLib._System.Net;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class ObjectProjection<TValue> : Projection 
    {
        private Action<TValue> _action;
        [DebuggerStepThrough]
        public ObjectProjection(string jsonText) : base(jsonText)
        {
        }    

        public ObjectProjection<TValue> PreProcess(Action<TValue> action)
        {
            _action = action;
            return this;
        }


        public T Map<T,TProjection>(Action<TProjection> action = null) 
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

                    try
                    {
                        var projection = JsonHelper.ParseObject<TProjection>(jObject);
                        _action?.Invoke(projection);
                        action?.Invoke(projection);
                        response.Data = projection;
                    }
                    catch (Exception ex)
                    {
                        throw new MapJsonException($"ParseObject<{typeof(TProjection).ToStringNotation()}> failed", ex);
                    }
                }
            }
            return response;
        }

        public Response<TValue> Map<TProjection>(Action<TProjection> action = null) 
            where TProjection : TValue, new()
        {
            Response<TValue> response = null;
            if (IsEmpty)
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

                    try
                    {
                        var projection = JsonHelper.ParseObject<TProjection>(jObject);
                        _action?.Invoke(projection);
                        action?.Invoke(projection);
                        response.Data = projection;
                    }
                    catch (Exception ex)
                    {
                        throw new MapJsonException($"ParseObject<{typeof(TProjection).ToStringNotation()}> failed", ex);
                    }
                }
            }
            return response;
        }
    }
}