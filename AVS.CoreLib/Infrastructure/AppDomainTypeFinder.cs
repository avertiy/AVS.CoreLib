using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Infrastructure
{
    public class AppDomainTypeFinder : ITypeFinder
    {
        private bool ignoreReflectionErrors = true;
        private AssemblyLoader _loader = new AssemblyLoader();
        
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return this.FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            var assemblies = GetAssemblies();
            return this.FindClassesOfType(assignTypeFrom, assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return this.FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, 
            IEnumerable<Assembly> assemblies, 
            bool onlyConcreteClasses = true)
        {
            List<Type> typeList = new List<Type>();
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    Type[] typeArray = (Type[])null;
                    try
                    {
                        typeArray = assembly.GetTypes();
                    }
                    catch(Exception ex)
                    {
                        Debug.Write(ex.ToString(), "AppDomainTypeFinder");
                        if (!this.ignoreReflectionErrors)
                            throw;
                    }
                    if (typeArray == null)
                        continue;

                    foreach (Type type in typeArray)
                    {
                        if ((assignTypeFrom.IsAssignableFrom(type) || assignTypeFrom.IsGenericTypeDefinition 
                            && this.DoesTypeImplementOpenGeneric(type, assignTypeFrom)) 
                            && !type.IsInterface)
                        {
                            if (onlyConcreteClasses)
                            {
                                if (type.IsClass && !type.IsAbstract)
                                    typeList.Add(type);
                            }
                            else
                                typeList.Add(type);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                string message = string.Empty;
                foreach (Exception loaderException in ex.LoaderExceptions)
                    message = message + loaderException.Message + Environment.NewLine;
                Exception exception = new Exception(message, (Exception)ex);
                Debug.WriteLine(exception.Message, new object[1]
                {
                    (object) exception
                });
                throw exception;
            }
            return (IEnumerable<Type>)typeList;
        }

        public virtual IList<Assembly> GetAssemblies()
        {
            return _loader.AppAssemblies;
        }

        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                Type genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (Type type1 in type.FindInterfaces((TypeFilter)((objType, objCriteria) => true), (object)null))
                {
                    if (type1.IsGenericType)
                        return genericTypeDefinition.IsAssignableFrom(type1.GetGenericTypeDefinition());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public class AssemblyLoader
    {
        List<string> _addedAssemblyNames = null;
        List<Assembly> _appAssemblies = null;

        public IList<Assembly> AppAssemblies => _appAssemblies ?? LoadAssemblies();

        public string AssemblyRestrictToLoadingPattern { get; set; } = ".*";

        public string AssemblySkipLoadingPattern { get; set; } = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease|^Wamp|^WebSocket|^AVS.ProxyUtil";

        protected IList<Assembly> LoadAssemblies()
        {
            //load assemblies from bin directory
            _addedAssemblyNames = new List<string>();
            _appAssemblies = new List<Assembly>();
            var path = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(file);
                if(Matches(assemblyName.FullName))
                    Assembly.Load(assemblyName);                
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                AddAppAssembly(assembly);
            }
            return _appAssemblies;
        }

        protected void AddAppAssembly(Assembly assembly)
        {
            if (this.Matches(assembly.FullName) && !_addedAssemblyNames.Contains(assembly.FullName))
            {
                _appAssemblies.Add(assembly);
                _addedAssemblyNames.Add(assembly.FullName);
            }
        }

        protected virtual bool Matches(string assemblyFullName)
        {
            return !this.Matches(assemblyFullName, this.AssemblySkipLoadingPattern)
                   && this.Matches(assemblyFullName, this.AssemblyRestrictToLoadingPattern);
        }

        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}