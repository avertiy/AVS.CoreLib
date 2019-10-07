using System;
using AVS.CoreLib._System.Debug;

namespace AVS.CoreLib._System.Net
{
    public class LoadDataException : Exception
    {
        public LoadDataException(Response response) : base($"{DebugUtil.GetCallerName()} failed: {response.Error}")
        {
        }
        public LoadDataException(string arg, Response response) : base($"{DebugUtil.GetCallerName()} for {arg} failed: {response.Error}")
        {
        }
    }
}