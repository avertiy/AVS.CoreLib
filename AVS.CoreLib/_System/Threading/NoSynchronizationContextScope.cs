using System;
using System.Threading;

namespace AVS.CoreLib._System.Threading
{
    ///<summary>
    /// usage: 
    /// using(new NoSynchronizationContextScope.Enter()){ ... }
    /// </summary>
    /// <remarks>
    /// It's a struct not a class because value type that gets put in a using statement will not be boxed. 
    /// This appears to be a C# optimization as boxing is only omitted when a value type that implements IDisposable is in a using statement, 
    /// not in any other context.
    /// </remarks>
    public static class NoSynchronizationContextScope
    {
        public static Disposable Enter()
        {
            var context = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            return new Disposable(context);
        }

        /// <remarks>
        /// Value type that gets put in a using statement will not be boxed! 
        /// This appears to be a C# optimization as boxing is only omitted when a value type that implements IDisposable is in a using statement, 
        /// not in any other context.
        /// </remarks>
        public struct Disposable : IDisposable
        {
            private readonly SynchronizationContext _synchronizationContext;

            public Disposable(SynchronizationContext synchronizationContext)
            {
                _synchronizationContext = synchronizationContext;
            }

            public void Dispose() =>
                SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }
    }
}