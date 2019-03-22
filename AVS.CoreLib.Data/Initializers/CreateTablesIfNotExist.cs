using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : BaseDbContext
    {
        private readonly string[] _tablesToValidate;
        private readonly string[] _customCommands;

        //public bool DatabaseHasBeenRecreated { get; protected set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        /// <param name="customCommands">A list of custom commands to execute</param>
        public CreateTablesIfNotExist(string[] tablesToValidate, string [] customCommands)
        {
            this._tablesToValidate = tablesToValidate;
            this._customCommands = customCommands;
        }

        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (dbExists)
            {
                bool createTables;
                if (_tablesToValidate != null && _tablesToValidate.Length > 0)
                {
                    //we have some table names to validate
                    var existingTableNames = new List<string>(context.Database.SqlQuery<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"));
                    createTables = !existingTableNames.Intersect(_tablesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
                }
                else
                {
                    //check whether tables are already created
                    int numberOfTables = 0;
                    foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                        numberOfTables = t1;

                    createTables = numberOfTables == 0;
                }

                if (createTables)
                {
                    var createDbScript = context.CreateDatabaseScript();
                    Debug.WriteLine("");
                    Debug.WriteLine(createDbScript, "CreateDatabaseScript\r\n");
                    //create db
                    context.Database.ExecuteSqlCommand(createDbScript);
                    //Seed(context);
                    context.SaveChanges();

                    if (_customCommands != null && _customCommands.Length > 0)
                    {
                        Debug.WriteLine("");
                        foreach (var command in _customCommands)
                        {
                            Debug.WriteLine(command, "CreateDatabaseScript custom command");
                            context.Database.ExecuteSqlCommand(command);
                        }
                    }
                    Debug.WriteLine("");
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
    }


    
}
