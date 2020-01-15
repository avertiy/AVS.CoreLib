using System;
using AVS.CoreLib.ConsoleTools.Utils;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    public class BootstrapWrapper
    {
        internal Action StopCallback;
        /// <summary>
        /// Logger prints (console or text file or whatever) log messages when program/service is starting
        /// </summary>
        public IBootstrapLogger Logger { get; set; } = new BootstrapLogger();

        public BootstrapWrapper OnStart(Action<Bootstrap> configuration)
        {
            Bootstrap.Build(configuration, Logger);
            return this;
        }

        public BootstrapWrapper OnStop(Action onStop = null)
        {
            StopCallback = onStop;
            return this;
        }
    }
}