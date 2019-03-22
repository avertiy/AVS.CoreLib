using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AVS.CoreLib.Data.EF
{
    public class InMemoryDbContext : IDbContext
    {
        private readonly Dictionary<string, object> _dbSets = new Dictionary<string, object>();
        public IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).FullName;
            if (_dbSets.ContainsKey(key))
                return (IDbSet<TEntity>)_dbSets[key];

            var dbSet = new InMemoryDbSet<TEntity>();
            _dbSets.Add(key, dbSet);
            return dbSet;
        }

        public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            var dbSet = Set<TEntity>();
            foreach (var entity in entities)
            {
                dbSet.Add(entity);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            return new List<TEntity>();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return new List<TElement>();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            return 0;
        }

        public void Detach(object entity)
        {
        }

        public bool ProxyCreationEnabled { get; set; }
        public bool AutoDetectChangesEnabled { get; set; }
        public DbContextConfiguration Configuration => null;
    }
}