using System.Data.Entity;
using AVS.CoreLib.Data.EF;
using AVS.CoreLib.Data.Initializers;

namespace AVS.CoreLib.Infrastructure
{
    /// <summary>
    /// Uses IInstallationService to seed database with initial data
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class EfStartUpTaskBase<TContext> : IStartupTask where TContext : BaseDbContext
    {
        protected abstract CreateTablesIfNotExist<TContext> GetInitializer();

        public virtual void Execute()
        {
            var initializer = GetInitializer();
            Database.SetInitializer(initializer);
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}