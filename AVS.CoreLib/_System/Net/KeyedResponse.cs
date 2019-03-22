using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AVS.CoreLib._System.Net
{
    public class KeyedResponse<TKey, TValue> : Response<IDictionary<TKey, TValue>>
    {
        public Response<TValue> ToResponse(TKey key)
        {
            var response = new Response<TValue>();
            if (HasError)
            {
                response.Error = Error;
            }
            else
            {
                if (Data.ContainsKey(key))
                {
                    response.Data = Data[key];
                }
            }
            return response;
        }
    }
}