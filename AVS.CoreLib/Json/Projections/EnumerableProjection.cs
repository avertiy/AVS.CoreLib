using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class EnumerableProjection<TValue> : Projection
    {
        [DebuggerStepThrough]
        public EnumerableProjection(string jsonText) : base(jsonText)
        {
        }

        private Action<TValue> _itemFunc;

        public EnumerableProjection<TValue> ForEach(Action<TValue> func)
        {
            _itemFunc = func;
            return this;
        }

        public TResult Map<TResult, TProjection>() where TResult : Response<IEnumerable<TValue>>, new()
        {
            TResult response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "[]")
            {
                response = new TResult() { Data = new List<TValue>() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new TResult();

                    JToken token = JToken.Load(reader);

                    if (token.Type == JTokenType.Object)
                    {
                        var jObject = (JObject)token;
                        if (ContainsError(jObject, (Response)response))
                            return response;
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        var jArray = (JArray) token;
                        response.Data = !jArray.HasValues ? 
                            new List<TValue>() : ParseEnumerable((JArray)token, typeof(TProjection));
                        return response;
                    }

                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
            }
            return response;
        }

        public Response<IList<TValue>> Map<TProjection>() where TProjection : TValue, new()
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
                        response.Data = JsonHelper.ParseList<TValue>((JArray)token, typeof(TProjection), _itemFunc);
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
            if (JsonText == null || JsonText.Length < 10000)
                return await Task.FromResult(Map<TProjection>());

            return await Task.Run(() => Map<TProjection>()).ConfigureAwait(false);
        }

        protected IEnumerable<TValue> ParseEnumerable(JArray jArray, Type itemType)
        {
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            foreach (JToken token in jArray)
            {
                if (token.Type != JTokenType.Object)
                {
                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
                var value = (TValue)serializer.Deserialize(token.CreateReader(), itemType);

                _itemFunc?.Invoke(value);
                
                yield return value;
            }
        }

    }
}