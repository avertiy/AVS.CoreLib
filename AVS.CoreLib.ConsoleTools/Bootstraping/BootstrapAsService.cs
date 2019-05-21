using System;
using AVS.CoreLib.ConsoleTools.Utils;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    public class BootstrapAsService
    {
        internal Action StopCallback;
        /// <summary>
        /// Logger prints (console or text file or whatever) log messages when program/service is starting
        /// </summary>
        public IBootstrapLogger Logger { get; set; } = new BootstrapLogger();

        public BootstrapAsService OnStart(Action<Bootstrap> configuration)
        {
            Bootstrap.Build(configuration, Logger);
            return this;
        }

        public BootstrapAsService OnStop(Action onStop = null)
        {
            StopCallback = onStop;
            return this;
        }
    }
}