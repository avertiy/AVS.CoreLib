using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Logging.LogWriters;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.Services.Tasks.AppTasks
{
    public abstract partial class TaskBase : ITask
    {
        public abstract void Execute(TaskLogWriter log);
    }
    
    /// <summary>
    /// Parameterized task is a task that has a corresponding task node in AppConfig with an attribute args|parameters
    /// e.g. 
    /// <Task type="MyTask" args="-param1 value -param2 value2" enabled="true" stopOnError="true" seconds="60" ></Task>
    /// ="-x Poloniex -pair BTC_MAID"
    /// </summary>
    public abstract class ParameterizedTask<TAppConfig, TParameters> : ITask 
        where TAppConfig: class, IAppConfig
        where TParameters : class, IDictionary<string, string>, new()
    {
        protected TParameters Parameters { get; set; }
        protected TAppConfig AppConfig;
        protected TaskNode Config;

        protected ParameterizedTask(TAppConfig config)
        {
            AppConfig = config;
            Config = config.Tasks[GetType()];
            Parameters  = ArgsParser.Parse<TParameters>(Config.Parameters);
        }
        
        public virtual void Execute(TaskLogWriter log)
        {
            BeforeExecute(log);
            Execute(log, Parameters);
            AfterExecute(log);
        }

        public abstract void Execute(TaskLogWriter log, TParameters parameters);

        public virtual void BeforeExecute(TaskLogWriter log)
        {
        }

        public virtual void AfterExecute(TaskLogWriter log)
        {
        }
    }


}