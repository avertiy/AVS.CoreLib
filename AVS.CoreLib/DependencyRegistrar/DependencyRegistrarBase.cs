using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using AVS.CoreLib.Caching;
using AVS.CoreLib.Data;
using AVS.CoreLib.Data.EF;
using AVS.CoreLib.Data.Events;
using AVS.CoreLib.Events;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Emails;
using AVS.CoreLib.Services.Logging;
using AVS.CoreLib.Services.Logging.LogBuffers;
using AVS.CoreLib.Services.Logging.Loggers;
using AVS.CoreLib.Services.Logging.LogWriters;
using AVS.CoreLib.Services.Tasks;

namespace AVS.CoreLib.DependencyRegistrar
{
    public abstract class DependencyRegistrarBase: IDependencyRegistrar
    {
        public abstract int Order { get; }

        public virtual bool UseDatabase => true;

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, IAppConfig config)
        {
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("static_cache").InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            RegisterEventConsumers(builder, typeFinder);
            RegisterCoreLibServices(builder, typeFinder, config);
            RegisterServices(builder, typeFinder, config);
            
        }

        protected abstract void RegisterServices(ContainerBuilder builder, ITypeFinder typeFinder, IAppConfig config);

        protected virtual void RegisterCoreLibServices(ContainerBuilder builder, ITypeFinder typeFinder, IAppConfig config)
        {
            if (UseDatabase)
            {
                builder.RegisterType<ScheduleTaskEntityService>().As<IScheduleTaskService>().InstancePerLifetimeScope();
                builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerLifetimeScope();
                builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();
                builder.RegisterType<LogEntityService>().As<ILogEntityService>().InstancePerLifetimeScope();
                builder.RegisterType<DatabaseLogger>().AsSelf().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<ScheduleTaskInMemoryService>().As<IScheduleTaskService>().SingleInstance();
            }
            builder.RegisterType<ConsoleLogger>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<TextLogBuffer>().As<ILogBuffer>().InstancePerDependency();
            builder.RegisterType<TaskLogWriter>().AsSelf().InstancePerLifetimeScope();
            builder.Register(c => (AppConfig)config).As<AppConfig>().SingleInstance();
        }

        protected void RegisterEventConsumers(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
        }

        protected void RegisterDbContext<T>(ContainerBuilder builder, string connectionString)
            where T : IDbContext
        {
            var ctx = (IDbContext)Activator.CreateInstance(typeof(T), new object[] { connectionString });
            builder.Register(c => ctx).InstancePerLifetimeScope();
        }

        protected void RegisterDbContext<T>(ContainerBuilder builder, string connectionString, string contextName)
            where T : IDbContext
        {
            //data layer
            //register named context
            var ctx = (IDbContext)Activator.CreateInstance(typeof(T), new object[] { connectionString });
            builder.Register(c => ctx)
                .Named<IDbContext>(contextName)
                .InstancePerLifetimeScope();

            builder.Register(c => (T)ctx).InstancePerLifetimeScope();
        }
    }
}