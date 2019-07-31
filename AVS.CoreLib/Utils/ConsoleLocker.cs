using System;
using System.Threading;

namespace AVS.CoreLib.Utils
{
    //use locker to sync multi threading access to console read/write operations
    //e.g. using(var locker = ConsoleLocker.Create()){ do read/write operations..}
    public class ConsoleLocker : IDisposable
    {
        private static readonly object Lock = new object();
        private bool _lockWasTaken = false;

        public static ConsoleLocker Create()
        {
            var locker = new ConsoleLocker();
            Monitor.Enter(Lock, ref locker._lockWasTaken);
            return locker;
        }

        public void Dispose()
        {
            if (_lockWasTaken)
                Monitor.Exit(Lock);
        }
    }
}