using System;
using AVS.CoreLib.ConsoleTools.Utils;

namespace AVS.CoreLib.ConsoleTools.Bootstraping
{
    public class BootstrapAsService
    {
        internal Action StopCallback;
        public BootstrapAsService OnStart(Action<Bootstrap> configuration)
        {
            Bootstrap.Build(configuration, new BootstrapLogger());
            return this;
        }

        public BootstrapAsService OnStop(Action onStop = null)
        {
            StopCallback = onStop;
            return this;
        }
    }
}