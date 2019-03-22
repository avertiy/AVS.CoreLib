using System.Data;
using System.Xml;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Services.Logging;

namespace AVS.CoreLib._System.Debug
{
    public class WatchLogger : QuickWatch
    {
        public WatchLogger(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger;

        public bool LoggingEnabled
        {
            get => Logger != null;
            set => Logger = value ? EngineContext.Current.Resolve<ILogger>() : null;
        }

        public override string Watch(DataTable dt, int rowCount = 5)
        {
            var text= base.Watch(dt, rowCount);
            Logger?.Debug("QuickWatch.Watch => DataTable", text);
            return text;
        }

        public override string Watch(DataTable dt, DataRow row)
        {
            var text = base.Watch(dt, row);
            Logger?.Debug("QuickWatch.Watch => DataTable", text);
            return text;
        }

        public override string Watch(XmlDocument xmlDoc)
        {
            var text = base.Watch(xmlDoc);
            Logger?.Debug("QuickWatch.Watch => XmlDocument", text);
            return text;
        }
    }
}