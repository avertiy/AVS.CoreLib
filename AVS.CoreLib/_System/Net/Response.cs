using System;
using AVS.CoreLib.Json.Converters;
using Newtonsoft.Json;

namespace AVS.CoreLib._System.Net
{
    [JsonConverter(typeof(ResponseConverter))]
    public class Response : IResponse
    {
        public string Error { get; set; }
        public bool ShouldSerializeError()
        {
            return Error != null;
        }

        public virtual bool Success => string.IsNullOrEmpty(Error);

        public override string ToString()
        {
            return Success ? "Response=> OK" : $"Response=> Fail [{Error}]";
        }

        public static implicit operator bool(Response foo)
        {
            return foo.Success;
        }

        public static Response<T> Create<T>(string source = null)
        {
            return new Response<T>();
        }

        public static Response<T> Create<T>(T data, Exception ex)
        {
            return new Response<T>() { Data = data, Error = ex.Message };
        }

        public static Response<T> Create<T>(T data, string error = null)
        {
            return new Response<T>() { Data = data, Error = error };
        }

        public Response<T> OnSuccess<T>(Func<T> func)
        {
            var response = new Response<T>() { Error = Error };
            if (Success)
            {
                try
                {
                    response.Data = func();
                }
                catch (Exception ex)
                {
                    response.Error = $"Unhandled exception: {ex.Message}\r\n\r\n{ex.StackTrace}";
                }
            }
            return response;
        }
    }

    [JsonConverter(typeof(ResponseTResultConverter))]
    public class Response<T> : Response, IResponse<T> 
    {
        public virtual T Data { get; set; }

        public override bool Success => base.Success && !Equals(Data, default(T));
        
        public static implicit operator T(Response<T> foo)
        {
            return foo.Data;
        }

        public TResultModel To<TResultModel>(Action<T, TResultModel> onSuccess)
            where TResultModel : Response, new()
        {
            var result = new TResultModel { Error = Error };
            if (Success)
            {
                onSuccess(this.Data, result);
            }
            return result;
        }
        
        public Response<TData> AsResponse<TData>(Func<T, TData> transform)
        {
            var response = new Response<TData>() { Error = Error };
            if (Error == null)
                response.Data = transform(Data);
            return response;
        }
    }
}