using System.Collections.Generic;
using AVS.CoreLib.Json.Converters;
using Newtonsoft.Json;

namespace AVS.CoreLib._System.Net
{
    [JsonConverter(typeof(ResponseConverter))]
    public class Response : IResponse
    {
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
        public virtual bool Success => string.IsNullOrEmpty(Error);

        public Response()
        {
        }
        public Response(IResponse response)
        {
            Error = response.Error;
        }

        public override string ToString()
        {
            return Success ? "Response=> OK" : $"Response=> Fail [{Error}]";
        }
    }

    [JsonConverter(typeof(ResponseTResultConverter))]
    public class Response<T> : Response, IResponse<T> 
    {
        public T Data { get; set; }

        public override bool Success => !HasError && !Equals(Data, default(T));
        
        public static implicit operator bool(Response<T> foo)
        {
            return foo.Success;
        }

        public static implicit operator T(Response<T> foo)
        {
            return foo.Data;
        }
    }
}