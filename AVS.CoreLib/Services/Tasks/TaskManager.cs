using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;

namespace AVS.CoreLib.Services.Tasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskManager
    {
        private static TaskManager _taskManager = null;
        public static TaskManager Instance => _taskManager ?? (_taskManager = new TaskManager());

        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();
        private int _notRunTasksInterval = 60 * 30; //30 minutes

        private TaskManager()
        {
        }


        /// <summary>
        /// Initializes the task manager with schedule tasks stored in database
        /// You might call in program start InstallScheduledTasks of IInstallationService 
        /// </summary>
        public void Initialize()
        {
            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            IList<ScheduleTask> scheduleTasks = taskService.GetAll();

            Initialize(scheduleTasks.ToArray());
        }

        public void RegisterTasks(IAppConfig config, params ScheduleTask[] scheduleTasks)
        {
            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            foreach (var scheduleTask in scheduleTasks)
            {
                scheduleTask.ApplicationInstanceId = config.AppInstance.Id;
                var t = taskService.GetTaskByType(scheduleTask.Type);
                if(t == null)
                    taskService.Insert(scheduleTask);
                else
                    taskService.Update(scheduleTask);
            }
            Initialize(scheduleTasks);
        }

        private void Initialize(ScheduleTask[] scheduleTasks)
        {
            this._taskThreads.Clear();
            //group by threads with the same seconds
            foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread()
                {
                    Seconds = scheduleTaskGrouped.Key
                };
                foreach (var scheduleTask in scheduleTaskGrouped)
                {
                    var task = new TaskThreadItem(scheduleTask);
                    taskThread.AddTask(task);
                }

                this._taskThreads.Add(taskThread);
            }

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time
            var notRunTasks = scheduleTasks
                .Where(x => x.Seconds >= _notRunTasksInterval)
                .Where(x => !x.LastStartUtc.HasValue || x.LastStartUtc.Value.AddSeconds(_notRunTasksInterval) < DateTime.UtcNow)
                .ToList();
            //create a thread for the tasks which weren't run for a long time
            if (notRunTasks.Count > 0)
            {
                var taskThread = new TaskThread()
                {
                    RunOnlyOnce = true,
                    Seconds = 60 * 5 //let's run such tasks in 5 minutes after application start
                };
                foreach (var scheduleTask in notRunTasks)
                {
                    var task = new TaskThreadItem(scheduleTask);
                    taskThread.AddTask(task);
                }

                this._taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads
        {
            get
            {
                return new ReadOnlyCollection<TaskThread>(this._taskThreads);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Task threads #{_taskThreads.Count}");
            for (var index = 0; index < _taskThreads.Count; index++)
            {
                var taskThread = _taskThreads[index];
                sb.Append($"Run every {taskThread.Seconds} sec.:");
                foreach (var group in taskThread.Tasks.GroupBy(t=>t.Group))
                {
                    sb.Append(!string.IsNullOrEmpty(@group.Key) ? $"\r\n\t{@group.Key}:" : $"\r\n\t");
                    foreach (var task in group)
                    {
                        sb.Append($"\"{task.Name}\", ");
                    }
                }
                sb.Length-=2;
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}