using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AVS.CoreLib._System.Net;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class ListProjection<TValue> : Projection
    {
        private Action<TValue> _itemAction;
        [DebuggerStepThrough]
        public ListProjection(string jsonText) : base(jsonText)
        {
        }
        
        public ListProjection<TValue> ForEach(Action<TValue> action)
        {
            _itemAction = action;
            return this;
        }

        public TResult Map<TResult,TProjection>() where TResult : Response<IList<TValue>>, new()
        {
            TResult response = new TResult();
            if (IsEmpty)
            {
                response.Data = new List<TValue>();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    JToken token = JToken.Load(reader);

                    if (token.Type == JTokenType.Object)
                    {
                        var jObject = (JObject)token;
                        if (ContainsError(jObject, (Response)response))
                            return response;
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        try
                        {
                            response.Data = JsonHelper.ParseList<TValue>((JArray)token, typeof(TProjection), _itemAction);
                        }
                        catch (Exception ex)
                        {
                            throw new MapJsonException($"ParseList<{typeof(TValue).ToStringNotation()}> with projection of {typeof(TProjection).ToStringNotation()}> failed", ex);
                        }
                        return response;
                    }

                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
            }
            return response;
        }

        public Response<IList<TValue>> Map<TProjection>() where TProjection:TValue, new()
        {
            Response<IList<TValue>> response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "[]")
            {
                response = new Response<IList<TValue>>() { Data = new List<TValue>() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new Response<IList<TValue>>();

                    JToken token = JToken.Load(reader);

                    if (token.Type == JTokenType.Object)
                    {
                        var jObject = (JObject)token;
                        if (ContainsError(jObject, (Response)response))
                            return response;
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        try
                        {
                            response.Data = JsonHelper.ParseList<TValue>((JArray)token, typeof(TProjection), _itemAction);
                        }
                        catch (Exception ex)
                        {
                            throw new MapJsonException($"ParseList<{typeof(TValue).ToStringNotation()}> with projection of {typeof(TProjection).ToStringNotation()}> failed", ex);
                        }
                        return response;
                    }

                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
            }
            return response;

        }

        /// <summary>
        /// Use MapAsync when expected number ot items (i.e. to parse json more than 100 otherwise it has no sence to run maaping in async manner)
        /// </summary>
        public async Task<Response<IList<TValue>>> MapAsync<TProjection>() where TProjection : TValue, new()
        {
            if(JsonText == null || JsonText.Length < 10000)
                return await Task.FromResult(Map<TProjection>());

            return await Task.Run(() => Map<TProjection>()).ConfigureAwait(false);
        }
    }
}