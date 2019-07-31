using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Infrastructure;

namespace AVS.CoreLib._System.Debug
{
    public class DebugUtil
    {
        private static IQuickWatch _qucikWatch = null;

        public static IQuickWatch QuickWatch {
            get=> _qucikWatch ?? (_qucikWatch = EngineContext.Current.Resolve<IQuickWatch>());
            set => _qucikWatch = value;
        }

        public static string GetCallerName(int skipFrames = 1)
        {
            return new StackFrame(skipFrames, true).GetMethod().Name;
        }

        public static string GetCallStack(int deep = 3, int skip = 1)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames == null || stackFrames.Length == 0)
                return String.Empty;
            StringBuilder log = new StringBuilder();

            log.Append("Call stack: ");

            string prev = null;
            string prevTypeName = null;
            for (int i = skip, k = 0; i <= deep; i++)
            {
                if (i + k >= stackFrames.Length)
                    break;
                var method = stackFrames[i + k].GetMethod().Name;
                string typeName = string.Empty;
                var reflectedType = stackFrames[i + k].GetMethod().ReflectedType;
                if (reflectedType != null)
                {
                    typeName = reflectedType.Name;
                }
                if (prev == method && prevTypeName == typeName || typeName.StartsWith("<>"))
                {
                    k++; //skip overload calls and generic auto generated classes
                    continue;
                }
                var line = stackFrames[i + k].GetFileLineNumber();
                if (prev == null)
                {
                    log.AppendFormat("=>{0}.{1}", typeName, method);
                }
                else
                {
                    log.AppendFormat("{0}.{1}", typeName, method);
                }
                if (line > 0)
                {
                    log.Append($" line {line}");
                }
                log.Append("; ");
                prev = method;
                prevTypeName = typeName;
            }
            return log.ToString();
        }

        #region Stopwatch

        public static Stopwatch Stopwatch(Action action)
        {
            var watch = new Stopwatch();
            watch.Execute(action);
            return watch;
        }

        public static Stopwatch Stopwatch<T>(Func<T> action, out T result)
        {
            var watch = new Stopwatch();
            result = watch.Execute(action);
            return watch;
        }

        #endregion
    }
}