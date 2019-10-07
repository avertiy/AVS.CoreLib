using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib._System.Net;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class DictionaryProjection<TKey, TValue> : Projection
        where TKey : class
    {
        private Action<TKey, TValue> _itemAction = null;

        [DebuggerStepThrough]
        public DictionaryProjection(string jsonText) : base(jsonText)
        {
        }

        public DictionaryProjection<TKey, TValue> ForEach(Action<TKey, TValue> action)
        {
            _itemAction = action;
            return this;
        }

        /// <summary>
        /// Maps json into specified type T
        /// </summary>
        /// <typeparam name="T">Type inherited from Response &lt;IDictionary&lt;TKey, TValue>></typeparam>
        /// <typeparam name="TProjection">implementation of the TValue interface, in case TValue is IList&lt;IEntity> TProjection must be implementation of IEntity</typeparam>
        public T Map<T, TProjection>() 
            where T : Response<IDictionary<TKey, TValue>>, new()
            where TProjection : new()
        {
            T response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]")
            {
                response = new T() { Data = new Dictionary<TKey, TValue>() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new T();

                    JToken token = JToken.Load(reader);

                    if (token.Type == JTokenType.Object)
                    {
                        var jObject = (JObject)token;
                        if (!ContainsError(jObject, (Response) response))
                        {
                            try
                            {
                                response.Data = JsonHelper.ParseDictionary<TKey, TValue, TProjection>(jObject, _itemAction);
                            }
                            catch (Exception ex)
                            {
                                throw new MapJsonException($"ParseDictionary<{typeof(TKey).Name},{typeof(TValue).ToStringNotation()}> with projection of {typeof(TProjection).ToStringNotation()}> failed", ex);
                            }
                        }
                            
                        return response;
                    }

                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
            }
            return response;
        }

        /// <summary>
        /// Maps json into Response&lt;IDictionary&lt;TKey, TValue>>
        /// </summary>
        /// <typeparam name="TProjection">implementation of the TValue interface, in case TValue is IList&lt;IEntity> TProjection must be implementation of IEntity</typeparam>
        public Response<IDictionary<TKey, TValue>> Map<TProjection>()
            where TProjection : new()
        {
            Response<IDictionary<TKey, TValue>> response = null;
            if (string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]")
            {
                response = new Response<IDictionary<TKey, TValue>>() { Data = new Dictionary<TKey, TValue>() };
            }
            else
            {
                using (var stringReader = new StringReader(JsonText))
                using (var reader = new JsonTextReader(stringReader))
                {
                    response = new Response<IDictionary<TKey, TValue>>();

                    JToken token = JToken.Load(reader);

                    if (token.Type == JTokenType.Object)
                    {
                        var jObject = (JObject)token;
                        if (!ContainsError(jObject, (Response) response))
                        {
                            try
                            {
                                response.Data = JsonHelper.ParseDictionary<TKey, TValue, TProjection>(jObject, _itemAction);
                            }
                            catch (Exception ex)
                            {
                                throw new MapJsonException($"ParseDictionary<{typeof(TKey).Name},{typeof(TValue).ToStringNotation()}> with projection of {typeof(TProjection).ToStringNotation()}> failed", ex);
                            }
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
        public async Task<Response<IDictionary<TKey, TValue>>> MapAsync<TProjection>() where TProjection : TValue, new()
        {
            if (JsonText == null || JsonText.Length < 10000)
                return await Task.FromResult(Map<TProjection>());

            return await Task.Run(() => Map<TProjection>()).ConfigureAwait(false);
        }
    }
}