using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
                if (token.Type != JTokenType.Object)
                {
                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");
                }
                var value = (T)serializer.Deserialize(token.CreateReader(), itemType);
                action?.Invoke(value);
                list.Add(value);
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
            
            var dt = DateTime.Now;

            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            var ts = DateTime.Now - dt;

            var ms = ts.TotalMilliseconds;
            var ticks = ts.Ticks;

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

                    var list = JsonHelper.Parse<TValue>((JArray)kp.Value, genericType, addMethod, itemType);
                    var key = ConvertToTKey<TKey>(kp.Key);

                    action?.Invoke(key, list);
                    dictionary.Add(key, list);
                }
            }

            return dictionary;
        }
        
        private static T Parse<T>(JArray jArray, Type genericType, MethodInfo addMethod, Type itemType)
        {
            if (addMethod == null)
                throw new ArgumentNullException(nameof(addMethod));

            var list = Activator.CreateInstance(genericType);

            if (!jArray.HasValues)
                return (T)list;

            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

            foreach (JToken token in jArray)
            {
                if (token.Type != JTokenType.Object)
                    throw new JsonReaderException($"Unexpected JToken type {token.Type}");

                var value = serializer.Deserialize(token.CreateReader(), itemType);
                addMethod.Invoke(list, new[] { value });
            }
            
            return (T)list;
        }

        private static Type Match(Type tValue, Type tProjection)
        {
            if (!tValue.IsGenericType)
            {
                throw new NotSupportedException($"{tValue.Name} is expected to be a generic type");
            }

            var gArgs = tValue.GetGenericArguments();

            if (gArgs.Length != 1)
                throw new NotSupportedException($"{tValue.Name} is expected to have one generic argument");

            if (!gArgs[0].IsInterface)
                throw new NotSupportedException($"{tValue.Name} is expected to have an interface as a generic argument");

            var interfaces = tProjection.GetInterfaces();
            if (!interfaces.Contains(gArgs[0]))
            {
                throw new NotSupportedException($"{tProjection.Name} is expected to implement {gArgs[0].Name}");
            }

            Type type = null;
            if (tValue.Name.StartsWith("IList") || tValue.Name.StartsWith("List"))
            {
                type = typeof(List<>);
            }

            if (tValue.Name.StartsWith("ICollection") || tValue.Name.StartsWith("Collection"))
            {
                type = typeof(Collection<>);
            }

            if(type == null)
                throw new NotSupportedException($"{tValue.Name} is expected to be asignable from List<> or Collection<>");

            var genericType = type.MakeGenericType(gArgs[0]);
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
    }
}