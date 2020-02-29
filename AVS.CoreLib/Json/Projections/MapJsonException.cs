using System;

namespace AVS.CoreLib.Json.Projections
{
    public class MapJsonException : Exception
    {
        public MapJsonException(string message) : base(message)
        {
        }

        public MapJsonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}