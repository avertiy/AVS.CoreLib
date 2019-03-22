using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Data;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Data.Events;
using AVS.CoreLib.Data.Services;

namespace AVS.CoreLib.Services.Tasks
{
    /// <summary>
    /// Task service interface
    /// </summary>
    public partial interface IScheduleTaskService 
    {
        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        ScheduleTask GetTaskByType(string type);
        void Update(ScheduleTask task);
        IList<ScheduleTask> GetAll();
        void Insert(ScheduleTask task);
        void Delete(ScheduleTask task);
        IList<ScheduleTask> GetAll(Func<ScheduleTask, bool> predicate);
    }

    public class ScheduleTaskInMemoryService : IScheduleTaskService
    {
        private readonly List<ScheduleTask> _tasks;

        public ScheduleTaskInMemoryService()
        {
            _tasks = new List<ScheduleTask>();
        }

        public ScheduleTask GetTaskByType(string type)
        {
            return _tasks.FirstOrDefault(t => t.Type == type);
        }

        public void Update(ScheduleTask task)
        {
            var item = GetTaskByType(task.Type);
            item.LastEndUtc = task.LastEndUtc;
            item.LastStartUtc = task.LastStartUtc;
            item.LastSuccessUtc = task.LastSuccessUtc;
            item.Group = task.Group;
            item.ApplicationInstanceId = task.ApplicationInstanceId;
            item.Name = task.Name;
            item.Description = task.Description;
            item.Seconds = task.Seconds;
            item.StopOnError = task.StopOnError;
            item.Enabled = task.Enabled;
        }

        public IList<ScheduleTask> GetAll()
        {
            return _tasks;
        }

        public void Insert(ScheduleTask task)
        {
            _tasks.Add(task);
        }

        public void Delete(ScheduleTask task)
        {
            var item = GetTaskByType(task.Type);
            if (item != null)
                _tasks.Remove(item);
        }

        public IList<ScheduleTask> GetAll(Func<ScheduleTask, bool> predicate)
        {
            return _tasks.Where(predicate).ToList();
        }
    }

    public class ScheduleTaskEntityService : EntityServiceBase<ScheduleTask>, IScheduleTaskService, IEntityServiceBase<ScheduleTask>
    {
        public ScheduleTaskEntityService(IRepository<ScheduleTask> repository, IEventPublisher eventPublisher) : base(repository, eventPublisher)
        {
        }

        public ScheduleTask GetTaskByType(string type)
        {
            var query = Repository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);
            var task = query.FirstOrDefault();
            return task;
        }
    }
}