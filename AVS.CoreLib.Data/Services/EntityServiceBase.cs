using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data.Events;

namespace AVS.CoreLib.Data.Services
{
    public interface IEntityServiceBase<TEntity>
       where TEntity : BaseEntity
    {
        void Delete(TEntity entity);
        /// <summary>
        /// executes sql command: delete from [table]
        /// </summary>
        int DeleteAll();
        /// <summary>
        /// executes sql command: DELETE FROM [table] WHERE Id IN(ids)
        /// </summary>
        int DeleteMany(IList<int> ids);

        void Insert(TEntity entity);

        /// <summary>
        /// implementation through EF
        /// </summary>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// implementation through SqlBulkCopy (much faster than EF)
        /// </summary>
        void BulkInsert(IEnumerable<TEntity> entities, int batchSize = 0);
        void Update(TEntity entity);

        TEntity GetById(int entityId);

        TEntity FirstOrDefault(Func<TEntity, bool> predicate);
        TEntity FirstOrDefault<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector);
        TEntity LastOrDefault(Func<TEntity, bool> predicate);
        TEntity LastOrDefault<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector);

        bool Any(Func<TEntity, bool> predicate);

        IList<TEntity> GetByIds(int[] ids);

        IList<TEntity> GetAll();
        IList<TEntity> GetAll<TKey>(Func<TEntity, TKey> orderBySelector);
        IList<TEntity> GetAll(Func<TEntity, bool> predicate);
        IList<TEntity> GetAll<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector);
        IPagedList<TEntity> GetAll(int pageIndex, int pageSize);

        IPagedList<TEntity> GetAll(int pageIndex, int pageSize, Func<IQueryable<TEntity>, IList<TEntity>> query);
        IPagedList<TEntity> GetAll<TKey>(int pageIndex, int pageSize, Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector);
        IPagedList<TEntity> GetAll<TKey>(int pageIndex, int pageSize, Func<TEntity, TKey> orderBySelector);
    }

    public abstract class EntityServiceBase<TEntity> : IEntityServiceBase<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IEventPublisher EventPublisher;

        protected EntityServiceBase(IRepository<TEntity> repository, IEventPublisher eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }


        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            Repository.Delete(entity);
            EventPublisher.EntityDeleted(entity);
        }

        public virtual int DeleteAll()
        {
            var efRep = Repository as IEfRepository<TEntity>;
            if(efRep == null)
                throw new NotImplementedException("DeleteAll is implemented only for IEfRepository");
            
            var table = efRep.Context.GetTableName<TEntity>();

            var deletedRowCount = efRep.Context.ExecuteSqlCommand($"delete from {table}");
            return deletedRowCount;
        }

        public virtual int DeleteMany(IList<int> ids)
        {
            var efRep = Repository as IEfRepository<TEntity>;
            if (efRep == null)
                throw new NotImplementedException("DeleteMany is implemented only for IEfRepository");
            var table = efRep.Context.GetTableName<TEntity>();


            if (ids.Count > 1500)
            {
                int deletedCount = 0;
                var currentList = new List<int>();
                for (var i = 0; i < ids.Count; i++)
                {
                    currentList.Add(i);
                    if (currentList.Count == 1500)
                    {
                        deletedCount+=DeleteMany(currentList);
                        currentList.Clear();
                    }
                }

                if(currentList.Any())
                    deletedCount += DeleteMany(currentList);

                return deletedCount;
            }

            var query = $"DELETE FROM {table} WHERE Id IN({string.Join(",",ids)})";
            var deletedRowCount = efRep.Context.ExecuteSqlCommand(query);
            return deletedRowCount;
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            Repository.Insert(entity);
            EventPublisher.EntityInserted(entity);
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            var arr = entities.ToArray();
            Repository.Insert(arr);
            EventPublisher.EntityBulkInsert(arr);
        }

        public virtual void BulkInsert(IEnumerable<TEntity> entities, int batchSize =0)
        {
            var arr = entities.ToArray();
            if (batchSize == 0)
                Repository.BulkInsert(arr);
            else
            {
                var items = new List<TEntity>(batchSize+10);
                foreach (var item in arr)
                {
                    items.Add(item);
                    if (items.Count == batchSize)
                    {
                        Repository.BulkInsert(items);
                        items.Clear();
                    }
                }
                if(items.Count>0)
                    Repository.BulkInsert(items);
            }
            EventPublisher.EntityBulkInsert(arr);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            Repository.Update(entity);
            EventPublisher.EntityUpdated(entity);
        }

        public virtual TEntity FirstOrDefault(Func<TEntity,bool> predicate)
        {
            return Repository.Table.FirstOrDefault(predicate);
        }

        public TEntity FirstOrDefault<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector)
        {
            return Repository.Table.OrderBy(orderBySelector).FirstOrDefault(predicate);
        }

        public virtual TEntity LastOrDefault(Func<TEntity, bool> predicate)
        {
            return Repository.Table.LastOrDefault(predicate);
        }

        public TEntity LastOrDefault<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector)
        {
            return Repository.Table.OrderBy(orderBySelector).LastOrDefault(predicate);
        }

        public bool Any(Func<TEntity, bool> predicate)
        {
            return Repository.Table.Any(predicate);
        }

        public virtual TEntity GetById(int entityId)
        {
            if (entityId == 0)
            {
                return null;
            }

            return Repository.GetById(entityId);
        }

        public IList<TEntity> GetAll()
        {
            return Repository.Table.ToList();
        }

        public IPagedList<TEntity> GetAll(int pageIndex, int pageSize)
        {
            return new PagedList<TEntity>(Repository.Table, pageIndex, pageSize);
        }

        public IList<TEntity> GetAll(Func<TEntity, bool> predicate)
        {
            return Repository.Table.Where(predicate).ToList();
        }

        public IList<TEntity> GetAll<TKey>(Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector)
        {
            return Repository.Table.Where(predicate).OrderBy(orderBySelector).ToList();
        }

        public IList<TEntity> GetAll<TKey>(Func<TEntity, TKey> orderBySelector)
        {
            return Repository.Table.OrderBy(orderBySelector).ToList();
        }

        public IPagedList<TEntity> GetAll<TKey>(int pageIndex, int pageSize, Func<TEntity, bool> predicate, Func<TEntity, TKey> orderBySelector)
        {
            return new PagedList<TEntity>(Repository.Table.Where(predicate).OrderBy(orderBySelector).ToList(), pageIndex, pageSize);
        }

        public IPagedList<TEntity> GetAll(int pageIndex, int pageSize, Func<IQueryable<TEntity>, IList<TEntity>> query)
        {
            var list = query(Repository.Table); 
            return new PagedList<TEntity>(list, pageIndex, pageSize);
        }

        public IPagedList<TEntity> GetAll<TKey>(int pageIndex, int pageSize, Func<TEntity, TKey> orderBySelector)
        {
            return new PagedList<TEntity>(Repository.Table.OrderBy(orderBySelector).ToList(), pageIndex, pageSize);
        }


        /// <summary>
        /// Get items by identifiers
        /// </summary>
        /// <param name="ids">identifiers</param>
        /// <returns>Countries</returns>
        public virtual IList<TEntity> GetByIds(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return new List<TEntity>();

            var query = from c in Repository.Table
                where ids.Contains(c.Id)
                select c;
            var list = query.ToList();
            //sort by passed identifiers
            var result = new List<TEntity>();
            foreach (int id in ids)
            {
                var item = list.Find(x => x.Id == id);
                if (item != null)
                    result.Add(item);
            }
            return result;
        }
    }
}