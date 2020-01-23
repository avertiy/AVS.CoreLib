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
            if (Success)
            {
                if (Data.ContainsKey(key))
                {
                    response.Data = Data[key];
                }
            }
            else
            {
                response.Error = Error;
            }
            return response;
        }
    }
}