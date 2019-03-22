using System.Collections.Generic;

namespace AVS.CoreLib.Data.Events
{
    public class EntityBulkInsert<T> where T : BaseEntity
    {
        public EntityBulkInsert(IEnumerable<T> entities)
        {
            this.Entities = entities;
        }

        public IEnumerable<T> Entities { get; private set; }
    }
}