using Autofac;
using Autofac.Builder;
using Autofac.Core;
using AVS.CoreLib.Caching;
using AVS.CoreLib.Data;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.DependencyRegistrar
{
    /// <summary>
    /// Extensions for DependencyRegistrar
    /// </summary>
    public static class DependencyRegistrarExtensions
    {
        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            WithStaticCacheParameter<T>(this IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder)
        {
            return builder.WithParameter(ResolvedParameter.ForNamed<ICacheManager>("static_cache"));
        }
        public static void RegisterServiceWithNopCacheStatic<TService, TInterface>(this ContainerBuilder builder)
        {
            builder.RegisterType<TService>()
                .As<TInterface>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("static_cache"))
                .InstancePerRequest();
        }

        public static void RegisterDbContextForEfRepository<T>(this ContainerBuilder builder, string ctxName)
            where T : BaseEntity
        {
            builder.RegisterType<EfRepository<T>>().As<IRepository<T>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ctxName)).InstancePerLifetimeScope();
        }


        
    }
}