using System;
using System.Collections.Generic;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Utils
{
    public interface IRequestData
    {
        string ToHttpQueryString();
        void Add(string key, string value);
    }

    public struct RequestData : IRequestData
    {
        private string _data;
        
        public RequestData(string queryStringParameters)
        {
            _data = queryStringParameters ?? string.Empty;
        }

        public string ToHttpQueryString()
        {
            return _data;
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
            var data =  new RequestData();
            data.Add(parameters);
            return data;
        }

        public void Add(string key, string value)
        {
            _data = _data.Length > 0 ? $"{key}={value}&{_data}" : $"{key}={value}";
        }

        public void Add(object[] parameters)
        {
            var str = string.Join("&", parameters);
            _data = string.IsNullOrEmpty(_data) ? str: $"{str}&{_data}";
        }

        public void Add(string[] arguments)
        {
            _data = _data ?? String.Empty;
            if (arguments.Length > 0)
            {
                if (arguments.Length % 2 == 0)
                {
                    string str= $"{arguments[0]}={arguments[1]}"; 
                    for (int i = 2; i < arguments.Length; i++)
                    {
                        str = $"{str}&{arguments[i]}={arguments[i + 1]}";
                        i++;
                    }

                    _data = _data.Length > 0 ? $"{str}&{_data}" : str;
                }
                else
                {
                    throw new ArgumentException("Even number of arguments is expected");
                }
            }
        }

        public static RequestData Empty=> new RequestData(string.Empty);

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
    }
}