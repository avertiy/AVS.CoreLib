using System;
using System.Diagnostics;
using System.IO;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;

namespace AVS.CoreLib.Json
{
    public class JsonResponseResult
    {
        public string JsonText { get; set; }

        public JsonResponseResult()
        {
        }

        public JsonResponseResult(string jsonText)
        {
            JsonText = jsonText;
        }

        public DictionaryProjection<TKey, TValue> AsDictionary<TKey, TValue>() where TKey : class
        {
            return new DictionaryProjection<TKey, TValue>(JsonText);
        }

        public ListProjection<T> AsList<T>()
        {
            return new ListProjection<T>(JsonText);
        }

        public EnumerableProjection<T> AsIEnumerable<T>()
        {
            return new EnumerableProjection<T>(JsonText);
        }

        public ObjectProjection<T> AsObject<T>() //where T : new()
        {
            return new ObjectProjection<T>(JsonText);
        }

        public T Deserialize<T>()
        {
            using (var stringReader = new StringReader(JsonText))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    try
                    {
                        var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                        return (T)serializer.Deserialize(jsonTextReader, typeof(T));
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine($"\r\nJSON Response:\r\n{JsonText.Truncate(1000)}\r\n\r\n", "JsonResponseResult");
#endif
                        throw new Exception($"Deserialization of type {typeof(T).Name} failed", ex);
                    }
                }
            }
        }
    }
}