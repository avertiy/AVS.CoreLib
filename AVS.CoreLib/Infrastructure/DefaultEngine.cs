using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using AutoMapper;
using AVS.CoreLib.DependencyRegistrar;
using AVS.CoreLib.Infrastructure.Config;

namespace AVS.CoreLib.Infrastructure
{
    /// <summary>
    /// When Engine is intialized it
    /// -builds Autofac container
    /// -registers Automapper configurations
    /// -runs startup tasks  
    /// (EfStartupTask sets Database initializer and seeds initial data)
    /// NOTE: you need to implement 
    /// EfStartupTask and IInstallationService to seed initial data
    /// Register DbContext and other dependencies 
    /// </summary>
    public class DefaultEngine : IEngine
    {
        private ContainerManager _containerManager;
        private ITypeFinder _typeFinder = null;
        public ContainerManager ContainerManager => this._containerManager;

        /// <summary> 
        /// 1. Builds Autofac container
        /// 2. Registers Automapper configurations
        /// 3. Runs startup tasks 
        /// </summary>
        public void Initialize(IAppConfig config)
        {
            //register dependencies
            this.RegisterDependencies(config);

            if (!config.IgnoreAutoMapper)
            {
                //register mapper configurations
                RegisterMapperConfiguration(config);
            }

            if (config.IgnoreStartupTasks)
                return;
            this.RunStartupTasks();
        }
        
        public T Resolve<T>() where T : class
        {
            return this.ContainerManager.Resolve<T>("", (ILifetimeScope)null);
        }

        public object Resolve(Type type)
        {
            return this.ContainerManager.Resolve(type, (ILifetimeScope)null);
        }

        public T[] ResolveAll<T>()
        {
            return this.ContainerManager.ResolveAll<T>("", (ILifetimeScope)null);
        }

        protected virtual ITypeFinder TypeFinder => _typeFinder ?? (_typeFinder = new AppDomainTypeFinder());
        

        protected virtual void RegisterDependencies(IAppConfig config)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterInstance<IAppConfig>(config).As<IAppConfig>().SingleInstance();
            builder.RegisterInstance<IEngine>(this).As<IEngine>().SingleInstance();

            var typeFinder = TypeFinder;
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //dependencies
            var list = new List<IDependencyRegistrar>();
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>(true);

            foreach (Type type in drTypes)
                list.Add((IDependencyRegistrar)Activator.CreateInstance(type));

            //sort dependncies
            list = list.AsQueryable().OrderBy(d => d.Order).ToList();
            foreach (IDependencyRegistrar dependencyRegistrar in list)
                dependencyRegistrar.Register(builder, typeFinder, config);

            IContainer container = builder.Build();
            this._containerManager = new ContainerManager(container);
            //DependencyResolver.SetResolver((IDependencyResolver)new AutofacDependencyResolver((ILifetimeScope)container));
        }

        protected virtual void RunStartupTasks()
        {
            var classesOfType = ContainerManager.Resolve<ITypeFinder>("", (ILifetimeScope)null)
                .FindClassesOfType<IStartupTask>(true);

            var tasks = new List<IStartupTask>();

            foreach (Type type in classesOfType)
                tasks.Add((IStartupTask)Activator.CreateInstance(type));

            //order by Order property
            Expression<Func<IStartupTask, int>> keySelector = st => st.Order;
            tasks = tasks.AsQueryable().OrderBy<IStartupTask, int>(keySelector).ToList<IStartupTask>();
            foreach (IStartupTask startupTask in tasks)
                startupTask.Execute();
        }

        /// <summary>
        /// Register mapping
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterMapperConfiguration(IAppConfig config)
        {
            //dependencies
            var typeFinder = TypeFinder;

            //register mapper configurations provided by other assemblies
            var mcTypes = typeFinder.FindClassesOfType<IMapperConfiguration>();
            var mcInstances = new List<IMapperConfiguration>();
            foreach (var mcType in mcTypes)
                mcInstances.Add((IMapperConfiguration)Activator.CreateInstance(mcType));
            //sort
            mcInstances = mcInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //get configurations
            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
            foreach (var mc in mcInstances)
                configurationActions.Add(mc.GetConfiguration());
            //register
            AutoMapperConfiguration.Init(configurationActions);
        }
    }
}