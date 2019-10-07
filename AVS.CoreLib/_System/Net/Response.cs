using System;
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

        public TResultModel To<TResultModel>(Action<T, TResultModel> onSuccess)
            where TResultModel : Response, new()
        {
            var result = this.Create<TResultModel>();
            if (Success)
            {
                onSuccess(this.Data, result);
            }

            return result;
        }

        public Response<TData> ToResponse<TData>()
        {
            return this.Create<Response<TData>>();
        }

        public Response<TData> ToResponse<TData>(Func<TData> onSuccess)
        {
            var response = this.Create<Response<TData>>();
            if (Success)
            {
                response.Data = onSuccess();
            }
            return response;
        }

        public Response<TData> ToResponse<TData>(Func<Response<TData>, TData> onSuccess)
        {
            var response = this.Create<Response<TData>>();
            if (Success)
            {
                response.Data = onSuccess(response);
            }
            return response;
        }

        private TResultModel Create<TResultModel>() where TResultModel : Response, new()
        {
            var res = new TResultModel { Error = Error };
            return res;
        }
    }
}