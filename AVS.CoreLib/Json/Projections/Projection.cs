using System;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json
{
    public abstract class Projection
    {
        protected string JsonText { get; set; }

        protected Projection(string jsonText)
        {
            JsonText = jsonText;
        }

        protected bool ContainsError(JObject jObject, Response response)
        {
            JToken token = jObject["error"] ?? jObject["Error"];
            if (token != null)
            {
                response.Error = (token.Value<string>());
            }
            return !string.IsNullOrEmpty(response.Error);
        }

    }
}