using System;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Specs.FluentInterface
{
    public abstract class ProviderSpec
    {
        public static ClassMapping map_as_class<T>(Action<ClassMap<T>> setup)
        {
            var provider = new ClassMap<T>();

            setup(provider);

            return provider.GetClassMapping();
        }

        public static SubclassMapping map_as_subclass<T>(Action<SubclassMap<T>> setup)
        {
            var provider = new SubclassMap<T>();

            setup(provider);

            return (SubclassMapping)((IProvider)provider).GetMapping();
        }
    }
}