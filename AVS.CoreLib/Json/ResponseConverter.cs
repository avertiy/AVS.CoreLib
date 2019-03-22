using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public class ResponseConverter<T, TResult>
        where T : TResult, new()

    {
        public virtual Response<TKeyedCollection> ParseKeyedCollection<TKeyedCollection>(string jsonText,
            JsonSerializer serializer)
        {
            using (var stringReader = new StringReader(jsonText))
            using (var reader = new JsonTextReader(stringReader))
            {
                var response = new Response<TKeyedCollection>();
                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Array)
                {
                    response.Data = ParseKeyedCollection<TKeyedCollection>((JArray)token, serializer);
                    return response;
                }

                if (token.Type == JTokenType.Object)
                {
                    var jObject = (JObject)token;
                    if (ContainsError(jObject, (Response)response))
                        return response;

                    throw new NotImplementedException("ParseDictionary #1");
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }
        }

        private TKeyedCollection ParseKeyedCollection<TKeyedCollection>(JArray token, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public virtual Response<Dictionary<string, TResult>> ParseDictionary(string jsonText, JsonSerializer serializer)
        {
            using (var stringReader = new StringReader(jsonText))
            using (var reader = new JsonTextReader(stringReader))
            {
                var response = new Response<Dictionary<string, TResult>>();
                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Object)
                {
                    var jObject = (JObject)token;
                    if (!ContainsError(jObject, (Response)response))
                        response.Data = ParseDictionary(jObject, serializer);
                    return response;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }
        }

        public virtual Response<List<TResult>> ParseList(string jsonText, JsonSerializer serializer)
        {
            using (var stringReader = new StringReader(jsonText))
            using (var reader = new JsonTextReader(stringReader))
            {
                var response = new Response<List<TResult>>();
                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Array)
                {
                    response.Data = ParseList((JArray)token, serializer);
                    return response;
                }

                if (token.Type == JTokenType.Object)
                {
                    var jObject = (JObject)token;
                    if (ContainsError(jObject, (Response)response))
                        return response;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }
        }

        public virtual Response<TCollection> ParseCollection<TCollection>(string jsonText, JsonSerializer serializer)
        {
            using (var stringReader = new StringReader(jsonText))
            using (var reader = new JsonTextReader(stringReader))
            {
                var response = new Response<TCollection>();
                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Array)
                {
                    response.Data = ParseCollection<TCollection>((JArray)token, serializer);
                    return response;
                }

                if (token.Type == JTokenType.Object)
                {
                    var jObject = (JObject)token;
                    if (ContainsError(jObject, (Response)response))
                        return response;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }
        }

        private TCollection ParseCollection<TCollection>(JArray token, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public virtual Response<TResult> Parse(string jsonText, JsonSerializer serializer)
        {
            using (var stringReader = new StringReader(jsonText))
            using (var reader = new JsonTextReader(stringReader))
            {
                var response = new Response<TResult>();

                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Object)
                {
                    var jObject = (JObject)token;

                    if (!ContainsError(jObject, (Response)response))
                    {
                        response.Data = ParseObject((JObject)token, serializer);
                        return response;
                    }

                    return response;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }
        }

        protected Dictionary<string, TResult> ParseDictionary(JObject jObject, JsonSerializer serializer)
        {
            var dictionary = new Dictionary<string, TResult>(jObject.Count);
            if (!jObject.HasValues)
                return dictionary;
            
            foreach (KeyValuePair<string, JToken> kp in jObject)
            {
                if (kp.Value.Type != JTokenType.Object)
                {
                    throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");
                }
                var value = (TResult)serializer.Deserialize(kp.Value.CreateReader(), typeof(T));
                dictionary.Add(kp.Key, value);
            }

            return dictionary;
        }

        protected List<TResult> ParseList(JArray jArray, JsonSerializer serializer)
        {
            var list = new List<TResult>();
            if (!jArray.HasValues)
                return list;

            foreach (JToken token in jArray)
            {
                if (token.Type == JTokenType.Object)
                {
                    var value = (TResult)serializer.Deserialize(token.CreateReader(), typeof(T));
                    list.Add(value);
                }
                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }

            return list;
        }

        protected TResult ParseObject(JObject jObject, JsonSerializer serializer)
        {
            var result = new T();
            serializer.Populate(jObject.CreateReader(), result);
            return result;
        }

        protected bool ContainsError(JObject jObject, Response response)
        {
            JToken token = jObject["error"] ?? jObject["Error"];
            if (token != null)
            {
                response.Error = (token.Value<string>());
                return true;
            }
            return false;
        }
    }
}