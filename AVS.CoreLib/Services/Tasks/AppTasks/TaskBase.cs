using System;
using System.Collections.Generic;
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
        protected readonly AppConfig.TaskNode Config;

        protected TaskBase(IAppConfig config)
        {
            Config = config.Tasks.GetTaskByType(this.GetType());
        }
        
        public abstract void Execute(TaskLogWriter log);
    }
}