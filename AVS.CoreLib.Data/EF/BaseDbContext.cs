using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Data.Utils;

namespace AVS.CoreLib.Data.EF
{
    /// <summary>
    /// Object context
    /// </summary>
    public abstract class BaseDbContext : DbContext, IDbContext
    {
        public TypeConfigurationsLoader ConfigurationsLoader { get; private set; }
        protected string ConnectionString2 { get; set; }

        #region Ctor

        protected BaseDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ConfigurationsLoader = new TypeConfigurationsLoader();
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }


        /// <param name="nameOrConnectionString"></param>
        /// <param name="nameOrConnectionString2">SqlBulkCopy requires a separate connection, it can't use the connection used by EF DbContenxt</param>
        protected BaseDbContext(string nameOrConnectionString, string nameOrConnectionString2)
            : base(nameOrConnectionString)
        {
            if(string.IsNullOrEmpty(nameOrConnectionString2))
                throw new ArgumentNullException(nameof(nameOrConnectionString2));
            ConnectionString2 = nameOrConnectionString.Contains("Initial Catalog") 
                ? nameOrConnectionString2 
                : SqlConnectionHelper.GetConnectionString(nameOrConnectionString2);
            ConfigurationsLoader = new TypeConfigurationsLoader();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// implement as return Assembly.GetExecutingAssembly();
        /// </summary>
        /// <returns></returns>
        protected abstract Assembly GetDbContextAssembly();
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var loader = ConfigurationsLoader ?? new TypeConfigurationsLoader();
            //1. load CoreLib configurations
            loader.AddAssembly(Assembly.GetExecutingAssembly());
            //2. load configurations from assembly where DbContext lies
            loader.AddAssembly(GetDbContextAssembly());
            
            loader.RegisterTypeConfigurations(modelBuilder);

            //release memory taken by loader
            ConfigurationsLoader.Clear();

            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //attach new entity
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //entity is already loaded
            return alreadyAttached;
        }

        /// <summary>
        /// adds Foregin Keys to Nop tables, which could not be specified via fluent EF mapping 
        /// due to limitation on different dbcontext in use
        /// </summary>
        /// <returns></returns>
        protected virtual void AddFK_Constraints(SqlStringBuilder sb) { }

        #endregion

        #region Methods

        /// <summary>
        /// Create database script
        /// </summary>
        /// <returns>SQL to generate database</returns>
        public string CreateDatabaseScript()
        {
            var sb = new SqlStringBuilder(false);
            var createDbScript = ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
            sb.Append(createDbScript);
            this.AddFK_Constraints(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
        
        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //add parameters to command
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //performance hack applied as described here - http://www.nopcommerce.com/boards/t/25483/fix-very-important-speed-improvement.aspx
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }
        
        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public IEnumerable<string> SqlQuery(string sql)
        {
            return this.Database.SqlQuery<string>(sql);
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter) this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }

        /// <summary>
        /// Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                string message = $"{ex.Message}\r\n{ex.InnerException?.Message}\r\n{ex.InnerException?.InnerException?.Message}";
                //note datetime2 conversion to datetime error might be caused by entity DateTime property which is not initialized
                throw new DbUpdateException(message, ex.InnerException);
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ",
                    ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("BaseDbContext.SaveChanges() Failed", ex);
            }
        }

        protected void SqlBulkCopy<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity
        {
            using (var reader = entities.GetDataReader())
            using (SqlConnection connection = new SqlConnection(ConnectionString2))
            using (SqlBulkCopy bcp = new SqlBulkCopy(connection))
            {
                connection.Open();
                
                bcp.DestinationTableName = $"{this.GetTableName<TEntity>()}";//typeof(TEntity).Name

                foreach (var prop in reader.Properties)
                {
                    if (!prop.CanRead || !prop.CanWrite)
                        continue;

                    var type = prop.PropertyType;
                    var flag = type.IsPrimitive || type == typeof(string) || type == typeof(DateTime)
                        || type == typeof(decimal)
                        || type.IsEnum;

                    if (!flag)
                        continue;

                    if (prop.CustomAttributes.Any(a=>a.AttributeType == typeof(NotMappedAttribute)))
                        continue;

                    bcp.ColumnMappings.Add(prop.Name, prop.Name);
                }

                bcp.WriteToServer(reader);
            }
        }

        public virtual void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            SqlBulkCopy(entities);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        #endregion
    }
}