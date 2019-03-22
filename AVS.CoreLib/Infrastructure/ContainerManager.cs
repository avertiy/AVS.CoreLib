using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace AVS.CoreLib.Infrastructure
{
    public class ContainerManager
    {
        private readonly IContainer _container;

        public virtual IContainer Container => this._container;

        public ContainerManager(IContainer container)
        {
            this._container = container;
        }

        public virtual T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
                scope = this.Scope();
            if (string.IsNullOrEmpty(key))
                return scope.Resolve<T>();
            return scope.ResolveKeyed<T>((object)key);
        }

        public virtual object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
                scope = this.Scope();
            return scope.Resolve(type);
        }

        public virtual T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
                scope = this.Scope();
            if (string.IsNullOrEmpty(key))
                return scope.Resolve<IEnumerable<T>>().ToArray<T>();
            return scope.ResolveKeyed<IEnumerable<T>>((object)key).ToArray<T>();
        }

        public virtual T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return this.ResolveUnregistered(typeof(T), scope) as T;
        }

        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
                scope = this.Scope();
            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                try
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    List<object> objectList = new List<object>();
                    foreach (ParameterInfo parameterInfo in parameters)
                    {
                        object obj = this.Resolve(parameterInfo.ParameterType, scope);
                        if (obj == null)
                            throw new ApplicationException("Unkown dependency");
                        objectList.Add(obj);
                    }
                    return Activator.CreateInstance(type, objectList.ToArray());
                }
                catch (ApplicationException ex)
                {
                }
            }
            throw new ApplicationException("No contructor was found that had all the dependencies satisfied.");
        }

        public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
                scope = this.Scope();
            return scope.TryResolve(serviceType, out instance);
        }

        public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
                scope = this.Scope();
            return scope.IsRegistered(serviceType);
        }

        public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
                scope = this.Scope();
            return scope.ResolveOptional(serviceType);
        }

        public virtual ILifetimeScope Scope()
        {
            //try
            //{
            return _container.BeginLifetimeScope();
                
            //}
            //catch (Exception ex)
            //{
            //    return this.Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            //}
        }
    }
}