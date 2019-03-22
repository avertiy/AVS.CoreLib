using System.Collections.Generic;

namespace AVS.CoreLib.Data.Events
{
    public static class EventPublisherExtensions
    {
        public static void EntityBulkInsert<T>(this IEventPublisher eventPublisher, IEnumerable<T> entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityBulkInsert<T>(entity));
        }

        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }
}