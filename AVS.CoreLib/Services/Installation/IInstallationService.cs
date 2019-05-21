using System;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Tasks;

namespace AVS.CoreLib.Services.Installation
{
    /// <summary>
    /// The installation service is called when database is created by EfStartUpTaskBase
    /// </summary>
    public partial interface IInstallationService
    {
        void InstallScheduledTasks(bool reinitialize = false);
        void InstallData();
        void ClearData();
    }

    public abstract class InstallationServiceBase : IInstallationService
    {
        protected readonly IScheduleTaskService ScheduleTaskService;
        protected readonly AppConfig Config;

        protected InstallationServiceBase(AppConfig config, IScheduleTaskService scheduleTaskService)
        {
            Config = config;
            ScheduleTaskService = scheduleTaskService;
        }

        protected abstract ScheduleTask[] Tasks { get; }

        public void InstallScheduledTasks(bool reinitialize = false)
        {
            if (!Config.Tasks.Install)
                return;

            //if already installed
            var scheduleTasks = ScheduleTaskService.GetAll(t => t.ApplicationInstanceId == Config.AppInstance.Id)
                .ToArray();

            if (!reinitialize && scheduleTasks.Any())
                return;

            foreach (var task in scheduleTasks)
            {
                ScheduleTaskService.Delete(task);
            }

            foreach (var scheduleTask in Tasks)
            {
                if (!string.IsNullOrEmpty(Config.Tasks.InstallForAppInstanceId))
                    scheduleTask.ApplicationInstanceId = Config.Tasks.InstallForAppInstanceId;
                Debug.WriteLine($"ScheduledTask {scheduleTask.Name} has been inserted");
                ScheduleTaskService.Insert(scheduleTask);
            }
        }

        public virtual void InstallData()
        {
        }

        public virtual void ClearData()
        {
        }
    }
}
