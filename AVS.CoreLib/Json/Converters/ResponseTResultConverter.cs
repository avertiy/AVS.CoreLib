using System;
using AVS.CoreLib._System.Net;

namespace AVS.CoreLib.Json.Converters
{
    public class ResponseTResultConverter : ResponseConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IResponse<>).IsAssignableFrom(objectType);
        }
    }
}