using System;
using Autofac;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Logging.LogWriters;

namespace AVS.CoreLib.Services.Tasks
{
    /// <summary>
    /// Instantiates and execute the ITask instance of type provided by ScheduleTask
    /// updates ScheduleTask propertiesw when running the task
    /// </summary>
    public partial class TaskThreadItem
    {
        #region Ctor

        /// <summary>
        /// Ctor for Task
        /// </summary>
        private TaskThreadItem()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public TaskThreadItem(ScheduleTask task)
        {
            this.Type = task.Type;
            this.Enabled = task.Enabled;
            this.StopOnError = task.StopOnError;
            this.Name = task.Name;
            this.Group = task.Group;
            this.LastSuccessUtc = task.LastSuccessUtc;
        }

        #endregion

        #region Utilities

        private static ITask ResolveTaskInstance(ILifetimeScope scope, string typeStr)
        {
            ITask task = null;
            Type type = System.Type.GetType(typeStr);
            if (type != null)
            {
                if (!EngineContext.Current.ContainerManager.TryResolve(type, scope, out var instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(type, scope);
                }
                task = instance as ITask;
            }
            return task;
        }

        #endregion

        private static T Resolve<T>(string key, ILifetimeScope scope) where T : class
        {
            return EngineContext.Current.ContainerManager.Resolve<T>(key, scope);
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether eexception should be thrown if some error happens</param>
        public void Execute(bool throwException = false)
        {
            if(!this.Enabled)
                return;
            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed
            using (ILifetimeScope scope = EngineContext.Current.ContainerManager.Scope())
            {
                var logWriter = Resolve<TaskLogWriter>("", scope);
                
                    var task = ResolveTaskInstance(scope, this.Type);
                    if (task == null)
                    {
                        logWriter.Write($"Unable to resolve task {this.Type} instance");
                        return;
                    }

                    var scheduleTaskService = Resolve<IScheduleTaskService>("", scope);
                    var scheduleTask = scheduleTaskService.GetTaskByType(this.Type);
                    if (scheduleTask == null)
                    {
                        logWriter.Write($"ScheduleTask {this.Type} not found");
                        return;
                    }

                    var cfg = Resolve<AppConfig>("", scope);
                    var appInstanceId = cfg.AppInstance.Id;
                    //multiple apps might use the database to execute tasks 
                    //or the app might be run from several machines to avoid executing tasks on multiple nodes/apps 
                    //the app instance id is used
                    if (!string.IsNullOrEmpty(scheduleTask.ApplicationInstanceId) &&
                        appInstanceId != scheduleTask.ApplicationInstanceId)
                        return;

                    this.IsRunning = true;


                    this.LastStartUtc = DateTime.UtcNow;
                    //update appropriate datetime properties
                    scheduleTask.LastStartUtc = this.LastStartUtc;
                    scheduleTaskService.Update(scheduleTask);
                try
                {
                    //execute task
                    logWriter.StartTask(scheduleTask.Name);
                    task.Execute(logWriter);
                    logWriter.EndTask(scheduleTask.Name);

                    this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    this.LastEndUtc = DateTime.UtcNow;

                    if (this.StopOnError)
                    {
                        this.Enabled = false;
                        logWriter.WriteWarning($"{scheduleTask.Name} has been stopped",exc.ToString());
                    }
                    else
                    {
                        logWriter.WriteError($"{scheduleTask.Name} unhandled error has occured", exc);
                    }

                    if (throwException)
                    {
                        throw;
                    }

                }
                finally
                {
                    logWriter.Flush();
                }

                //update appropriate datetime properties
                scheduleTask.LastEndUtc = this.LastEndUtc;
                scheduleTask.LastSuccessUtc = this.LastSuccessUtc;
                scheduleTaskService.Update(scheduleTask);
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name { get; private set; }

        public string Group { get; private set; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}