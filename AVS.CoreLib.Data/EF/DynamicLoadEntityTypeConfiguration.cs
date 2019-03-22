using System.Data.Entity.ModelConfiguration;

namespace AVS.CoreLib.Data.EF
{
    public interface IDynamicLoadEntityTypeConfiguration { }

    public abstract class DynamicLoadEntityTypeConfiguration<TEntityType> : EntityTypeConfiguration<TEntityType>, IDynamicLoadEntityTypeConfiguration
        where TEntityType : class
    {
        /// <summary>
        /// Configures the table name that this entity type is mapped to.
        /// </summary>
        /// <param name="tableName">The name of the table, e.g. entity name</param>
        /// <param name="resolver">TableNameResolver to resolve table name according to the rules, e.g. add table name prefix</param>
        /// <returns></returns>
        public EntityTypeConfiguration<TEntityType> ToTable(string tableName, ITableNameResolver resolver)
        {
            return base.ToTable(resolver.ResolveTableName(tableName), resolver.SchemaName);
        }
    }

    public interface ITableNameResolver
    {
        string ResolveTableName(string tableName);
        string SchemaName { get; }
    }

    public class CoreLibTableNameResolver : ITableNameResolver
    {
        public string ResolveTableName(string tableName)
        {
            //return "__CoreLib_" + tableName;
            return tableName;
        }

        public string SchemaName => "CoreLib";
    }

    public class TableNameResolver : ITableNameResolver
    {
        public TableNameResolver(string tableNamePrefix)
        {
            TableNamePrefix = tableNamePrefix;
            SchemaName = "dbo";
        }

        public TableNameResolver(string tableNamePrefix, string schemaName)
        {
            TableNamePrefix = tableNamePrefix;
            SchemaName = schemaName;
        }

        public string TableNamePrefix { get; protected set; }
        public string SchemaName { get; protected set; }

        public string ResolveTableName(string tableName)
        {
            return TableNamePrefix + tableName;
        }

    }

}