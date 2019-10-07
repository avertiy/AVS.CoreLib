using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Utils
{
    public interface IRequestData
    {
        string ToHttpQueryString();
        void Add(string key, string value);
    }

    public class RequestData : IRequestData
    {
        private readonly List<string> _items = new List<string>();

        public RequestData()
        {
        }
        
        public RequestData(string queryStringParameters)
        {
            Add(queryStringParameters);
        }
        
        public string ToHttpQueryString()
        {
            if(_items.Count == 0)
                return String.Empty;
            if (_items.Count == 1)
                return _items[0];
            return string.Join("&",_items.OrderBy(i=>i));
        }

        public static implicit operator RequestData(string str)
        {
            return new RequestData(str);
        }

        public static implicit operator RequestData(Dictionary<string, string> dict)
        {
            return new RequestData(dict.ToHttpPostString());
        }

        public static implicit operator RequestData(Dictionary<string, object> dict)
        {
            return new RequestData(dict.ToHttpPostString());
        }

        public static implicit operator RequestData(object[] parameters)
        {
            var data = new RequestData();
            data.Add(parameters);
            return data;
        }

        public void Add(string key, string value)
        {
            _items.Add($"{key}={value}");
        }

        public void Add(string queryStringParameters)
        {
            if (!string.IsNullOrEmpty(queryStringParameters))
                _items.Add(queryStringParameters);
        }

        public void Add(object[] parameters)
        {
            foreach (var parameter in parameters) 
                _items.Add(parameter.ToString());
        }

        public void Add(string[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                var p = arguments[i];
                if (!arguments[i].Contains("=") && i + 1 < arguments.Length)
                {
                    Add(arguments[i], arguments[i + 1]);
                    i++;
                }
                else
                {
                    _items.Add(arguments[i]);
                }
            }
        }

        public static RequestData Empty=> new RequestData();

        public static RequestData Create(string queryStringParameters)
        {
            return new RequestData(queryStringParameters);
        }

        public static RequestData Create(object[] parameters)
        {
            var data = new RequestData();
            data.Add(parameters);
            return data;
        }

        public static RequestData Create(string[] parameters)
        {
            var data = new RequestData();
            data.Add(parameters);
            return data;
        }
    }
}