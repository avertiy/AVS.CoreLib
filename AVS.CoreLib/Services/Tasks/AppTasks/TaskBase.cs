using System;
using System.Text;

namespace AVS.CoreLib.Services.Tasks.AppTasks
{
/*    public abstract partial class TaskBase : ITask
    {
        protected TaskLogWriter Log;
        
        protected TaskBase(TaskLogWriter log)
        {
            Log = log;
        }

        public void Execute()
        {
            Log.StartTask(this.GetType().Name);
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Log.TaskFailed(ex);
            }
            
            Log.EndTask();
            
            Log.Clear();
        }

        protected abstract bool Run();
    }*/
}