using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer.Model
{
    public class DependenciesConfiguration
    {
        private readonly Dictionary<Type, List<ImplementationContainer>> implementations;

        public void Register<TDependency, TImplementation>(Lifetime lifetime = Lifetime.InstancePerDependency)
            where TDependency : class
            where TImplementation : TDependency
        {
            Register(typeof(TDependency), typeof(TImplementation), lifetime);
        }

        public void Register(Type dependency, Type implementation, Lifetime lifetime = Lifetime.InstancePerDependency)
        {
            ImplementationContainer container = new ImplementationContainer(implementation, lifetime);
            if (dependency.IsGenericType)
            {
                dependency = dependency.GetGenericTypeDefinition();
            }

            List<ImplementationContainer> dependencyImplementations;
            lock (implementations)
            {
                if (!implementations.TryGetValue(dependency, out dependencyImplementations))
                {
                    dependencyImplementations = new List<ImplementationContainer>();
                    implementations[dependency] = dependencyImplementations;
                }
            }
            lock (dependencyImplementations)
            {
                dependencyImplementations.Add(container);
            }
        }

        public IEnumerable<ImplementationContainer> GetImplementations(Type type)
        {
            Type collectionType;

            if (type.IsGenericType)
            {
                collectionType = type.GetGenericTypeDefinition();
            }
            else
            {
                collectionType = type;
            }

            if (implementations.TryGetValue(collectionType, 
                out List<ImplementationContainer> dependencyImplementations))
            {
                IEnumerable<ImplementationContainer> result = 
                    new List<ImplementationContainer>(dependencyImplementations);
                if (type.IsGenericType)
                {
                    result = result.Where(impl => impl.ImplementationType.IsGenericTypeDefinition 
                                                    || type.IsAssignableFrom(impl.ImplementationType));
                }

                return result;
            }
            return new List<ImplementationContainer>();
        }

        public DependenciesConfiguration()
        {
            implementations = new Dictionary<Type, List<ImplementationContainer>>();
        }
    }
}