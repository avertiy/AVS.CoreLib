using System.Threading.Tasks;

namespace AVS.CoreLib.Infrastructure
{
    /// <summary>
    /// as opposed to <see cref="IStartupTask"/>
    /// the background tasks does not stop app execution and could be executed in background
    /// also background task should be registered in DI as it is resolved from DI 
    /// </summary>
    public interface IBackgroundTask
    {
        int Order { get; }
        Task ExecuteAsync();
    }
}