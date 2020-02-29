using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using AVS.CoreLib.Extensions;
using Newtonsoft.Json;

namespace AVS.CoreLib.Json
{
    public class JsonResponseResult
    {
        public string JsonText { get; set; }

        public string Error { get; set; }      

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

         /// <summary>
        /// 
        /// </summary>
        /// <param name="regex_pattern">pattern should contain a named group "data"</param>
        /// <param name="errorText">when pattern does not match and errorText is provided the JsonText will be set as the following json object { "error": "errorText"}</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool Take(string regex_pattern, string errorText = null, RegexOptions options = RegexOptions.None)
        {
            var re = new Regex(regex_pattern, options);
            var match = re.Match(JsonText);

            if (match.Success)
            {
                JsonText = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
            }
            else if (!string.IsNullOrEmpty(errorText))
            {
                JsonText = $"{{\"error\":\"{errorText}\"}}";
            }
            return match.Success;
        }

        public JsonResponseResult TakeMany(string regex_pattern, string errorText = null, RegexOptions options = RegexOptions.None)
        {
            var re = new Regex(regex_pattern, options);
            var match = re.Match(JsonText);
            if (match.Success)
            {
                var items = new List<string>();
                while (match.Success)
                {
                    items.Add(match.Groups["data"].Success ? match.Groups["data"].Value : match.Value);
                    match = match.NextMatch();
                }
                JsonText = $"[{string.Join(",", items)}]";
            }
            else if (!string.IsNullOrEmpty(errorText))
            {
                JsonText = $"{{\"error\":\"{errorText}\"}}";
            }
            return this;
        }

        public static implicit operator string(JsonResponseResult result)
        {
            return result?.JsonText;
        }

        public bool HasError
        {
            get
            {
                var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>.*?)[\"',}]", RegexOptions.IgnoreCase);
                var match = re.Match(JsonText);

                if (match.Success)
                {
                    Error = match.Groups["error"].Value;
                }

                return match.Success;
            }
        }
    }
}