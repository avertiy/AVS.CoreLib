using System;
using System.Text.RegularExpressions;
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

        public virtual bool IsEmpty => string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]";

        protected bool ContainsError(JObject jObject, Response response)
        {
            JToken token = jObject["error"] ?? jObject["Error"];
            if (token != null)
            {
                response.Error = (token.Value<string>());
            }
            return !string.IsNullOrEmpty(response.Error);
        }

        protected bool ContainsError(out string error)
        {
            var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>...*?)[\"',}]", RegexOptions.IgnoreCase);
            var match = re.Match(JsonText);
            error = null;
            if (match.Success)
            {
                error = match.Groups["error"].Value;
            }

            return match.Success;
        }
        
        protected Response<TData> CreateResponse<TData>() where TData : new()
        {
            var response = Response.Create<TData>(); 
            if (IsEmpty)
            {
                response.Data = new TData();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            return response;
        }

        protected Response<T> CreateResponse<T, TProjection>() where TProjection : T, new()
        {
            var response = Response.Create<T>();
            if (IsEmpty)
            {
                response.Data = new TProjection();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            return response;
        }

    }
}