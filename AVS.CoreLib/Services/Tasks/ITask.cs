using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Services.Logging.LogWriters;

namespace AVS.CoreLib.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute(TaskLogWriter log);
    }
}