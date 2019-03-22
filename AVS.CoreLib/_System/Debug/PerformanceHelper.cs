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

    /*public class TimeTracker
    {
        private Dictionary<string, DateTime> _ongoingOperations = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> _endedOperations = new Dictionary<string, DateTime>();
        private readonly ILogger _logger;
        private bool _enabled;
        private readonly string _title;

        private static TimeTracker _instance;
        public static TimeTracker Instance => _instance ?? (_instance = new TimeTracker("Default TimeTracker"));

        public TimeTracker(string title)
        {
            _logger = EngineContext.Current.Resolve<ILogger>();
            _enabled = _logger.IsEnabled(LogLevel.Debug);
            _title = title;
        }

        public void Start(string name)
        {
            if (!_enabled)
                return;
            if (_ongoingOperations.ContainsKey(name))
                throw new ArgumentException("The operation with such name has already been started");
            _ongoingOperations.Add(name, DateTime.Now);
        }

        public void End(string name)
        {
            if (!_enabled)
                return;
            if (!_ongoingOperations.ContainsKey(name))
                throw new ArgumentException($"The operation {name} has not been started");

            _endedOperations[name] = _ongoingOperations[name];
        }

        public TimeSpan GetElapsedTime(string name)
        {
            if (!_enabled)
                return TimeSpan.Zero;
            var isOngoing = _ongoingOperations.ContainsKey(name);
            if (!isOngoing && !_endedOperations.ContainsKey(name))
                throw new ArgumentException("The operation has not been started");

            return DateTime.Now - (isOngoing ? _ongoingOperations[name] : _endedOperations[name]);
        }

        public string GetInfo(string name)
        {
            if (!_enabled)
                return "";
            var ts = GetElapsedTime(name);
            var isOngoing = _ongoingOperations.ContainsKey(name);
            return $"The operation {name} started at {(isOngoing ? _ongoingOperations[name] : _endedOperations[name]):HH mm ss zzz}. Time elapsed {ts.Milliseconds} ms";
        }

        public void Reset()
        {
            _ongoingOperations = new Dictionary<string, DateTime>();
            _endedOperations = new Dictionary<string, DateTime>();
        }

        public void FlushToLog()
        {
            if (!_enabled)
                return;
            _logger.Debug<TimeTracker>(_title, this.ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#{_ongoingOperations.Count} operations in progress; #{_endedOperations.Count} completed");
            sb.AppendLine("In progress: ");
            foreach (var kp in _ongoingOperations)
            {
                var ts = DateTime.Now - kp.Value;
                sb.AppendLine($"[{kp.Key}] - {ts.Milliseconds}ms elapsed");
            }

            sb.AppendLine("Completed: ");
            foreach (var kp in _endedOperations)
            {
                var ts = DateTime.Now - kp.Value;
                sb.AppendLine($"[{kp.Key}] - {ts.Milliseconds}ms elapsed");
            }
            return sb.ToString();
        }
    }*/
}
