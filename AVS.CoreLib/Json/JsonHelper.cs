using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public static class JsonHelper
    {
        public static T ParseObject<T>(JObject jObject) where T : new()
        {
            var result = new T();
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
            serializer.Populate(jObject.CreateReader(), result);
            return result;
        }
        
        public static IList<T> ParseList<T>(JArray jArray, Type itemType, Action<T> action = null)
        {
            var list = new List<T>();
            if (!jArray.HasValues)
                return list;

            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            foreach (JToken token in jArray)
            {
                if (token.Type == JTokenType.Object)
                {
                    var value = (T)serializer.Deserialize(token.CreateReader(), itemType);
                    action?.Invoke(value);
                    list.Add(value);
                    continue;
                }

                if (token.Type == JTokenType.Array)
                {
                    T value = ParseJArray<T>(itemType, serializer, token);
                    action?.Invoke(value);
                    list.Add(value);
                    continue;
                }
                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }

            return list;
        }

        public static IList<T> ParseList<T>(JObject jObject, Func<string, JToken, T> convertFunc)
        {
            var list = new List<T>();
            if (!jObject.HasValues)
                return list;

            foreach (KeyValuePair<string, JToken> kp in jObject)
            {
                var item = convertFunc(kp.Key, kp.Value);
                list.Add(item);
            }

            return list;
        }
        
        public static T ParseJArray<T>(Type itemType, JsonSerializer serializer, JToken token)
        {
            try
            {
                return (T)serializer.Deserialize(token.CreateReader(), itemType);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception(
                    $"Unable to parse {itemType.Name} from json array (consider using [ArrayConverter] attribute)", ex);
            }
        }

        public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(JObject jObject,
            Func<dynamic, TKey> keyFunc,
            Func<dynamic, TValue> valFunc
        )
        {
            var dictionary = new Dictionary<TKey, TValue>(jObject.Count);

            if (!jObject.HasValues)
                return dictionary;

            foreach (KeyValuePair<string, JToken> kp in jObject)
            {
                if (kp.Value.Type == JTokenType.String)
                {
                    string value = ((JValue)kp.Value).Value<string>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else if (kp.Value.Type == JTokenType.Boolean)
                {
                    var value = ((JValue)kp.Value).Value<bool>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else if (kp.Value.Type == JTokenType.Integer)
                {
                    var value = ((JValue)kp.Value).Value<int>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else if (kp.Value.Type == JTokenType.Float)
                {
                    var value = ((JValue)kp.Value).Value<float>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else
                {
                    var obj = kp.Value.Value<object>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(obj));
                }
            }

            return dictionary;
        }
    }
}