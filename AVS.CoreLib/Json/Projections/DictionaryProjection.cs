using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib._System.Net;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;


namespace AVS.CoreLib.Json
{
    public class DictionaryProjection<TKey, TValue> : Projection
        where TKey : class
    {
        private Action<TKey, TValue> _itemAction = null;
        private Func<string, TKey> _keyFunc = null;
        private Func<TValue, TValue> _valueFunc = null;
        private Func<string, bool> _where = null;

        [DebuggerStepThrough]
        public DictionaryProjection(string jsonText) : base(jsonText)
        {
        }

        public DictionaryProjection<TKey, TValue> ForEach(Action<TKey, TValue> action)
        {
            _itemAction = action;
            return this;
        }
        
        public DictionaryProjection<TKey, TValue> Key(Func<string, TKey> keyConverter)
        {
            _keyFunc = keyConverter;
            return this;
        }

        public DictionaryProjection<TKey, TValue> Where(Func<string, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public DictionaryProjection<TKey, TValue> Value(Func<TValue, TValue> valueFunc)
        {
            _valueFunc = valueFunc;
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
                                response.Data = ParseDictionary<TProjection>(jObject);
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
                                response.Data = ParseDictionary<TProjection>(jObject);
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


        private IDictionary<TKey, TValue> ParseDictionary<TProjection>(JObject jObject)
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
                    if (_where != null && !_where(kp.Key))
                        continue;

                    if (kp.Value.Type != JTokenType.Object)
                        throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

                    var value = (TValue)serializer.Deserialize(kp.Value.CreateReader(), itemType);

                    var key = _keyFunc == null ? ConvertToTKey<TKey>(kp.Key) : _keyFunc(kp.Key);
                    value = _valueFunc != null ? _valueFunc(value) : value;

                    _itemAction?.Invoke(key, value);
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
                        if (_where != null && !_where(kp.Key))
                            continue;

                        if (kp.Value.Type != JTokenType.Array)
                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

                        var list = ParseGenericType<TValue>((JArray)kp.Value, genericType, addMethod, itemType);

                        var key = _keyFunc == null ? ConvertToTKey<TKey>(kp.Key) : _keyFunc(kp.Key);
                        list = _valueFunc != null ? _valueFunc(list) : list;

                        _itemAction?.Invoke(key, list);
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

        internal static TValue ParseGenericType<TValue>(JArray jArray, Type genericType, MethodInfo addMethod, Type itemType)
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
                    addMethod.Invoke(list, new[] { value });
                }
                catch (Exception ex)
                {
                    throw new JsonException($"unable deserialize JToken [{token.ToString()}] into {itemType.ToStringNotation()} ", ex);
                }
            }

            return (TValue)list;
        }

        internal static Type Match(Type tValue, Type tProjection)
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

            Type[] interfaces = tProjection.GetInterfaces();
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

        internal static void CheckTKeyType<TKey>()
        {
            var tKey = typeof(TKey);
            var tString = typeof(string);
            if (tKey == tString)
                return;

            throw new NotSupportedException($"The {tKey.Name} type of TKey is not supported");
        }

        internal static TKey ConvertToTKey<TKey>(string str) where TKey : class
        {
            return str as TKey;
        }
    }
}