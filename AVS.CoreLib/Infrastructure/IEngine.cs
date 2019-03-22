using System;
using System.Configuration;
using AVS.CoreLib.Infrastructure.Config;


namespace AVS.CoreLib.Infrastructure
{
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        void Initialize(IAppConfig config);

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}
