using System.Data.Entity;
using AVS.CoreLib.Data.Utils;

namespace AVS.CoreLib.Data.EF
{
    public abstract class BasePluginDbContext : BaseDbContext
    {
        public abstract string TableNamePrefix { get; }
        public abstract string[] TableNames { get; }

        #region Ctor

        protected BasePluginDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        #endregion

        #region Utilities

        protected string GetTableName(string entityName)
        {
            return $"{this.TableNamePrefix}{entityName}";
        }

        protected void ForeignKey<TPkEntity, TPluginFkEntity>(SqlStringBuilder sb, string fkColumnName, string pkColumnName = "Id")
        {
            sb.Append_ForeignKey(this.GetTableName(typeof(TPluginFkEntity).Name), typeof(TPkEntity).Name, fkColumnName, pkColumnName);
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //load configuration manually
        }

    }
}