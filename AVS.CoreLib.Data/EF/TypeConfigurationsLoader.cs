using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace AVS.CoreLib.Data.EF
{
    public class TypeConfigurationsLoader
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Insert(0, assembly);
        }

        public void RegisterTypeConfigurations(DbModelBuilder modelBuilder)
        {
            foreach (var assembly in GetAssemblies())
            {
                DynamicTypeConfigurationLoad(modelBuilder, assembly);
            }
        }

        public void Clear()
        {
            _assemblies.Clear();
        }

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            //var arr = _assemblies.ToArray().Reverse();
            return _assemblies.ToArray();
        }

        protected void DynamicTypeConfigurationLoad(DbModelBuilder modelBuilder, Assembly assembly)
        {
            //System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            //load type configurations of the assembly
            var typesToRegister = assembly.GetTypes()
                .Where(type => !String.IsNullOrEmpty(type.Namespace)
                               && !type.IsAbstract
                               && !type.IsGenericType
                               && type.GetInterfaces().Contains(typeof(IDynamicLoadEntityTypeConfiguration)));

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
        }

       
    }
}