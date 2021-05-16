using System;
using System.Threading.Tasks;
using AVS.CoreLib.Infrastructure.Config;


namespace AVS.CoreLib.Infrastructure
{
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        void Initialize(IAppConfig config);

        Task RunBackgroundTasksAsync();

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}
