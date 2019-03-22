using Autofac;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;

namespace AVS.CoreLib.DependencyRegistrar
{
    public interface IDependencyRegistrar
    {
        int Order { get; }

        void Register(ContainerBuilder builder, ITypeFinder typeFinder, IAppConfig config);
    }
}