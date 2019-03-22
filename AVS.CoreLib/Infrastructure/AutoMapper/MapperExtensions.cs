using System;
using System.Reflection;
using AutoMapper;

namespace AVS.CoreLib.Infrastructure
{
    public static class MapperExtensions
    {
        public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (sourceType.GetProperty(property.Name, flags) == null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }

        public static TResult GetWithDefault<TSource, TResult>(this TSource instance,
            Func<TSource, TResult> getter,
            TResult defaultValue = default(TResult))
            where TSource : class
        {
            return instance != null ? getter(instance) : defaultValue;
        }
    }
}