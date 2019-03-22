using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data.Events;

namespace AVS.CoreLib.Data.Services
{
    public interface IEntityServiceBase<TEntity, TChildEntity> : IEntityServiceBase<TEntity>
        where TEntity : BaseEntity
        where TChildEntity : BaseEntity
    {
        void DeleteItem(TChildEntity item);

        void InsertItem(TChildEntity item);

        void BulkInsert(IEnumerable<TChildEntity> items, int batchSize = 0);

        void UpdateItem(TChildEntity item);

        TChildEntity GetItemById(int id);

        IQueryable<TChildEntity> GetAllItems();

        IList<TChildEntity> GetAllItems(Func<TChildEntity, bool> predicate);
        IPagedList<TChildEntity> GetAllItems(int pageIndex, int pageSize);
    }
    /// <summary>
    /// Generic implemenation of the entity service for the entity with child IList of TItemEntity Items;
    /// </summary>
    /// <typeparam name="TEntity">Parent entity</typeparam>
    /// <typeparam name="TChildEntity">child items of the TEntity</typeparam>
    public abstract class EntityServiceBase<TEntity, TChildEntity> : EntityServiceBase<TEntity>,IEntityServiceBase<TEntity, TChildEntity> 
        where TEntity : BaseEntity
        where TChildEntity : BaseEntity
    {
        protected readonly IRepository<TChildEntity> ItemRepository;

        protected EntityServiceBase(IRepository<TEntity> repository,
            IEventPublisher eventPublisher,
            IRepository<TChildEntity> itemRepository
            ) : base(repository, eventPublisher)
        {
            ItemRepository = itemRepository;
        }

        public void DeleteItem(TChildEntity item)
        {
            ItemRepository.Delete(item);
            EventPublisher.EntityDeleted(item);
        }

        public void InsertItem(TChildEntity item)
        {
            ItemRepository.Insert(item);
            EventPublisher.EntityInserted(item);
        }

        public void BulkInsert(IEnumerable<TChildEntity> items, int batchSize = 0)
        {
            var arr = items.ToArray();
            if (batchSize == 0)
                ItemRepository.BulkInsert(arr);
            else
            {
                var list = new List<TChildEntity>(batchSize + 10);
                foreach (var item in arr)
                {
                    list.Add(item);
                    if (list.Count == batchSize)
                    {
                        ItemRepository.BulkInsert(list);
                        list.Clear();
                    }
                }
                if (list.Count > 0)
                    ItemRepository.BulkInsert(list);
            }
            EventPublisher.EntityBulkInsert(arr);
        }

        public void UpdateItem(TChildEntity item)
        {
            ItemRepository.Update(item);
            EventPublisher.EntityUpdated(item);
        }

        public TChildEntity GetItemById(int id)
        {
            return ItemRepository.GetById(id);
        }

        public IQueryable<TChildEntity> GetAllItems()
        {
            return ItemRepository.Table;
        }

        public IPagedList<TChildEntity> GetAllItems(int pageIndex, int pageSize)
        {
            return new PagedList<TChildEntity>(ItemRepository.Table, pageIndex, pageSize);
        }

        public IList<TChildEntity> GetAllItems(Func<TChildEntity, bool> predicate)
        {
            return ItemRepository.Table.Where(predicate).ToList();
        }
    }

    public interface IEntityServiceBase<TEntity, TChild1Entity, TChild2Entity> : IEntityServiceBase<TEntity>
        where TEntity : BaseEntity
        where TChild1Entity : BaseEntity
        where TChild2Entity : BaseEntity
    {
        void DeleteItem(TChild1Entity item);
        void DeleteItem(TChild2Entity item);

        void InsertItem(TChild1Entity item);
        void InsertItem(TChild2Entity item);

        void BulkInsert(IEnumerable<TChild1Entity> items, int batchSize = 0);
        void BulkInsert(IEnumerable<TChild2Entity> items, int batchSize = 0);

        void UpdateItem(TChild1Entity item);
        void UpdateItem(TChild2Entity item);

        TChild1Entity GetT1ItemById(int id);
        TChild2Entity GetT2ItemById(int id);

        IQueryable<TChild1Entity> GetAllT1Items();
        IQueryable<TChild2Entity> GetAllT2Items();

        IList<TChild1Entity> GetAllItems(Func<TChild1Entity, bool> predicate);
        IList<TChild2Entity> GetAllItems(Func<TChild2Entity, bool> predicate);
    }

    /// <summary>
    /// Generic implemenation of the entity service for the entity with a two child IList of TChild1Entity Items and IList of TChild2Entity Items;
    /// </summary>
    public abstract class EntityServiceBase<TEntity, TChild1Entity, TChild2Entity> : 
        EntityServiceBase<TEntity>, IEntityServiceBase<TEntity, TChild1Entity, TChild2Entity>
        where TEntity : BaseEntity
        where TChild1Entity : BaseEntity
        where TChild2Entity : BaseEntity
    {
        protected readonly IRepository<TChild1Entity> Item1Repository;
        protected readonly IRepository<TChild2Entity> Item2Repository;

        protected EntityServiceBase(IRepository<TEntity> repository,
            IEventPublisher eventPublisher,
            IRepository<TChild1Entity> item1Repository,
            IRepository<TChild2Entity> item2Repository
        ) : base(repository, eventPublisher)
        {
            Item1Repository = item1Repository;
            Item2Repository = item2Repository;
        }

        public void DeleteItem(TChild1Entity item)
        {
            Item1Repository.Delete(item);
            EventPublisher.EntityDeleted(item);
        }

        public void DeleteItem(TChild2Entity item)
        {
            Item2Repository.Delete(item);
            EventPublisher.EntityDeleted(item);
        }

        public void InsertItem(TChild1Entity item)
        {
            Item1Repository.Insert(item);
            EventPublisher.EntityInserted(item);
        }

        public void InsertItem(TChild2Entity item)
        {
            Item2Repository.Insert(item);
            EventPublisher.EntityInserted(item);
        }

        public void BulkInsert(IEnumerable<TChild1Entity> items, int batchSize = 0)
        {
            var arr = items.ToArray();
            if (batchSize == 0)
                Item1Repository.BulkInsert(arr);
            else
            {
                var list = new List<TChild1Entity>(batchSize + 10);
                foreach (var item in arr)
                {
                    list.Add(item);
                    if (list.Count == batchSize)
                    {
                        Item1Repository.BulkInsert(list);
                        list.Clear();
                    }
                }
                if (list.Count > 0)
                    Item1Repository.BulkInsert(list);
            }
            EventPublisher.EntityBulkInsert(arr);
        }

        public void BulkInsert(IEnumerable<TChild2Entity> items, int batchSize = 0)
        {
            var arr = items.ToArray();
            if (batchSize == 0)
                Item2Repository.BulkInsert(arr);
            else
            {
                var list = new List<TChild2Entity>(batchSize + 10);
                foreach (var item in arr)
                {
                    list.Add(item);
                    if (list.Count == batchSize)
                    {
                        Item2Repository.BulkInsert(list);
                        list.Clear();
                    }
                }
                if (list.Count > 0)
                    Item2Repository.BulkInsert(list);
            }
            EventPublisher.EntityBulkInsert(arr);
        }

        public void UpdateItem(TChild1Entity item)
        {
            Item1Repository.Update(item);
            EventPublisher.EntityUpdated(item);
        }

        public void UpdateItem(TChild2Entity item)
        {
            Item2Repository.Update(item);
            EventPublisher.EntityUpdated(item);
        }

        public TChild1Entity GetT1ItemById(int id)
        {
            return Item1Repository.GetById(id);
        }

        public TChild2Entity GetT2ItemById(int id)
        {
            return Item2Repository.GetById(id);
        }

        public IQueryable<TChild1Entity> GetAllT1Items()
        {
            return Item1Repository.Table;
        }

        public IQueryable<TChild2Entity> GetAllT2Items()
        {
            return Item2Repository.Table;
        }

        public IList<TChild1Entity> GetAllItems(Func<TChild1Entity, bool> predicate)
        {
            return Item1Repository.Table.Where(predicate).ToList();
        }

        
        public IList<TChild2Entity> GetAllItems(Func<TChild2Entity, bool> predicate)
        {
            return Item2Repository.Table.Where(predicate).ToList();
        }
    }

}