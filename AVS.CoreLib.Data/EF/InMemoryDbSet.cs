using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace AVS.CoreLib.Data.EF
{
    public class InMemoryDbSet<TEntity> : IDbSet<TEntity>
        where TEntity : BaseEntity
    {
        private readonly List<TEntity> _items = new List<TEntity>();
        public IEnumerator<TEntity> GetEnumerator()
        {

            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression => _items.AsQueryable().Expression;
        public Type ElementType => _items.AsQueryable().ElementType;
        public IQueryProvider Provider => _items.AsQueryable().Provider;

        public TEntity Find(params object[] keyValues)
        {
            return _items.Find(i => i.Id == (int)keyValues[0]);
        }

        public TEntity Add(TEntity entity)
        {
            _items.Add(entity);
            return entity;
        }

        public TEntity Remove(TEntity entity)
        {
            _items.Remove(entity);
            return entity;
        }

        public TEntity Attach(TEntity entity)
        {
            if (!_items.Contains(entity))
                Add(entity);
            return entity;
        }

        public TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<TEntity> Local => null;
    }
}