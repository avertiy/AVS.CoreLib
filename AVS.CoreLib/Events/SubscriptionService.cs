using System.Collections.Generic;
using AVS.CoreLib.Data.Events;
using AVS.CoreLib.Infrastructure;

namespace AVS.CoreLib.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {

        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            return EngineContext.Current.ResolveAll<IConsumer<T>>();
        }
    }
}