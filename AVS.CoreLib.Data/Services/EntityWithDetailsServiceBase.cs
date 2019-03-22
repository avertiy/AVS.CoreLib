using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data.Events;

namespace AVS.CoreLib.Data.Services
{
    public interface IEntityWithDetailsServiceBase<TEntity, TEntityDetails> : IEntityServiceBase<TEntity>
        where TEntity : BaseEntity
    {
        void DeleteDetails(TEntityDetails entity);

        void InsertDetails(TEntityDetails entity);

        void UpdateDetails(TEntityDetails entity);

        TEntityDetails GetDetailsById(int entityId);

        IList<TEntityDetails> GetAllDetails();

        IPagedList<TEntityDetails> GetAllDetails(int pageIndex, int pageSize);

        IPagedList<TEntityDetails> GetAllDetails(int pageIndex, int pageSize, Func<IQueryable<TEntityDetails>, IList<TEntityDetails>> query);
        IPagedList<TEntityDetails> GetAllDetails<TKey>(int pageIndex, int pageSize, Func<TEntityDetails, bool> predicate, Func<TEntityDetails, TKey> orderBySelector);
        IPagedList<TEntityDetails> GetAllDetails<TKey>(int pageIndex, int pageSize, Func<TEntityDetails, TKey> orderBySelector);
    }

    public abstract class EntityWithDetailsServiceBase<TEntity, TEntityDetails> : EntityServiceBase<TEntity>, IEntityWithDetailsServiceBase<TEntity, TEntityDetails>
        where TEntity : BaseEntity
        where TEntityDetails : BaseEntity
    {
        protected readonly IRepository<TEntityDetails> RepositoryDetails;

        protected EntityWithDetailsServiceBase(IRepository<TEntity> repository, IRepository<TEntityDetails> repositoryDetails,
            IEventPublisher eventPublisher) : base(repository, eventPublisher)
        {
            RepositoryDetails = repositoryDetails;
        }

        public void DeleteDetails(TEntityDetails entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            RepositoryDetails.Delete(entity);
            EventPublisher.EntityDeleted(entity);
        }

        public virtual void InsertDetails(TEntityDetails entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            RepositoryDetails.Insert(entity);
            EventPublisher.EntityInserted(entity);
        }

        public virtual void UpdateDetails(TEntityDetails entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            RepositoryDetails.Update(entity);
            EventPublisher.EntityUpdated(entity);
        }
        
        public virtual TEntityDetails GetDetailsById(int entityId)
        {
            if (entityId == 0)
            {
                return null;
            }

            return RepositoryDetails.GetById(entityId);
        }

        public IList<TEntityDetails> GetAllDetails()
        {
            return RepositoryDetails.Table.ToList();
        }

        public IPagedList<TEntityDetails> GetAllDetails(int pageIndex, int pageSize)
        {
            return new PagedList<TEntityDetails>(RepositoryDetails.Table, pageIndex, pageSize);
        }

        public IPagedList<TEntityDetails> GetAllDetails<TKey>(int pageIndex, int pageSize, Func<TEntityDetails, bool> predicate, Func<TEntityDetails, TKey> orderBySelector)
        {
            return new PagedList<TEntityDetails>(RepositoryDetails.Table.Where(predicate).OrderBy(orderBySelector).ToList(), pageIndex, pageSize);
        }

        public IPagedList<TEntityDetails> GetAllDetails(int pageIndex, int pageSize, Func<IQueryable<TEntityDetails>, IList<TEntityDetails>> query)
        {
            var list = query(RepositoryDetails.Table);
            return new PagedList<TEntityDetails>(list, pageIndex, pageSize);
        }

        public IPagedList<TEntityDetails> GetAllDetails<TKey>(int pageIndex, int pageSize, Func<TEntityDetails, TKey> orderBySelector)
        {
            return new PagedList<TEntityDetails>(RepositoryDetails.Table.OrderBy(orderBySelector).ToList(), pageIndex, pageSize);
        }
    }
}