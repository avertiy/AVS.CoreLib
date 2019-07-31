using System;
using System.Collections.Generic;

namespace AVS.CoreLib._System.Debug
{
    public static class PerformanceHelper
    {
        static Dictionary<string,DateTime> _operations;

        public static void OperationStarted(string name)
        {
            if(_operations == null)
                _operations = new Dictionary<string, DateTime>();
            if(_operations.ContainsKey(name))
                throw new ArgumentException("The operation with such name has already been started");
            _operations.Add(name,DateTime.Now);
        }
        public static void OperationEnded(string name)
        {
            _operations?.Remove(name);
        }
        public static TimeSpan GetElapsedTime(string operation)
        {
            if (_operations == null || !_operations.ContainsKey(operation))
                throw new ArgumentException("The operation has not been started");
            return DateTime.Now - _operations[operation];
        }

        public static string GetElapsedTimeString(string operation)
        {
            var ts = GetElapsedTime(operation);
            return
                $"The operation {operation} started at {_operations[operation]:HH mm ss zzz}. Time elapsed {ts.Milliseconds} ms";
        }


    }
}
