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

    public abstract class ParameterizedTask<TParameters> : ITask 
        where TParameters : class, ITaskParameters, new()
    {
        protected TParameters Parameters { get; set; }
        protected IAppConfig AppConfig;
        protected ParameterizedTask(IAppConfig config)
        {
            AppConfig = config;
            var task = config.Tasks[GetType()];
            Parameters = task.GetParameters<TParameters>();
        }

        public abstract void Execute(TaskLogWriter log);
    }
}