using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions;
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

        public static IEnumerable<T> ParseEnumerable<T>(JArray jArray, Type itemType)
        {
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            foreach (JToken token in jArray)
            {
                if (token.Type != JTokenType.Object)
                {
                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
                var value = (T)serializer.Deserialize(token.CreateReader(), itemType);
                yield return value;
            }
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

        public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue, TProjection>(JObject jObject, Action<TKey, TValue> action = null) 
            where TKey : class
        {
            var dictionary = new Dictionary<TKey, TValue>(jObject.Count);
            if (!jObject.HasValues)
                return dictionary;

            CheckTKeyType<TKey>();
            var itemType = typeof(TProjection);
            var tValue = typeof(TValue);
            
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            if (tValue.IsAssignableFrom(itemType))
            {
                foreach (KeyValuePair<string, JToken> kp in jObject)
                {
                    if (kp.Value.Type != JTokenType.Object)
                    {
                        throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");
                    }

                    var value = (TValue)serializer.Deserialize(kp.Value.CreateReader(), itemType);

                    var key = ConvertToTKey<TKey>(kp.Key);

                    action?.Invoke(key,value);

                    dictionary.Add(key, value);
                }
            }
            else
            {
                try
                {
                    var genericType = Match(tValue, itemType);
                    MethodInfo addMethod = genericType.GetMethod("Add");

                    if (addMethod == null)
                        throw new Exception($"Add method not found");

                    foreach (KeyValuePair<string, JToken> kp in jObject)
                    {
                        if (kp.Value.Type != JTokenType.Array)
                        {
                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");
                        }

                        var list = JsonHelper.ParseGenericType<TValue>((JArray) kp.Value, genericType, addMethod, itemType);
                        var key = ConvertToTKey<TKey>(kp.Key);

                        action?.Invoke(key, list);
                        dictionary.Add(key, list);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"ParseGenericType<{typeof(TKey).Name},{itemType.ToStringNotation()}> failed", ex);
                }
            }

            return dictionary;
        }
        
        public static IDictionary<string, string> ParseDictionary(JObject jObject)
        {
            var dictionary = new Dictionary<string, string>(jObject.Count);

            if (!jObject.HasValues)
                return dictionary;

            foreach (KeyValuePair<string, JToken> kp in jObject)
            {
                if (kp.Value.Type == JTokenType.String)
                {
                    var value = ((JValue) kp.Value).Value<string>();
                    dictionary.Add(kp.Key, value);
                }
            }
        
            return dictionary;
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

        private static TValue ParseGenericType<TValue>(JArray jArray, Type genericType, MethodInfo addMethod, Type itemType)
        {
            if (addMethod == null)
                throw new ArgumentNullException(nameof(addMethod));

            var list = Activator.CreateInstance(genericType);

            if (!jArray.HasValues)
                return (TValue)list;

            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            foreach (JToken token in jArray)
            {
                if (token.Type != JTokenType.Object)
                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                try
                {
                    var value = serializer.Deserialize(token.CreateReader(), itemType);
                    addMethod.Invoke(list, new[] {value});
                }
                catch (Exception ex)
                {
                    throw new JsonException($"unable deserialize JToken [{token.ToString()}] into {itemType.ToStringNotation()} ", ex);
                }
            }
            
            return (TValue)list;
        }

        private static Type Match(Type tValue, Type tProjection)
        {
            if (!tValue.IsGenericType)
            {
                throw new ArgumentException($"{tValue.Name} must be a generic type");
            }

            var genericArgs = tValue.GetGenericArguments();

            if (genericArgs.Length != 1)
                throw new ArgumentException($"{tValue.Name} must have one generic argument");

            if (!genericArgs[0].IsInterface)
                throw new ArgumentException($"{genericArgs[0].Name} must be an interface");

            var interfaces = tProjection.GetInterfaces();
            if (!interfaces.Contains(genericArgs[0]))
            {
                throw new ArgumentException($"{tProjection.Name} is expected to implement {genericArgs[0].Name}");
            }

            Type type = null;
            if (tValue.Name.StartsWith("IList") || tValue.Name.StartsWith("List"))
            {
                type = typeof(List<>);
            }
            else if (tValue.Name.StartsWith("ICollection") || tValue.Name.StartsWith("Collection"))
            {
                type = typeof(Collection<>);
            }

            if (type == null)
                throw new NotSupportedException($"{tValue.Name} must be assignable from List<> or Collection<>");

            var genericType = type.MakeGenericType(genericArgs[0]);
            return genericType;

        }

        private static void CheckTKeyType<TKey>()
        {
            var tKey = typeof(TKey);
            var tString = typeof(string);
            if (tKey == tString)
                return;

            throw new NotSupportedException($"The {tKey.Name} type of TKey is not supported");
        }

        private static TKey ConvertToTKey<TKey>(string str) where TKey : class
        {
            return str as TKey;
        }

        private static T ParseJArray<T>(Type itemType, JsonSerializer serializer, JToken token)
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
    }
}