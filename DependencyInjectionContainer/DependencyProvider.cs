using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DependencyInjectionContainer.Model
{
    public class DependencyProvider
    {
        private readonly DependenciesConfiguration configuration;
        private readonly ConcurrentDictionary<int, Stack<Type>> recursionTypes;

        public IEnumerable<TDependency> Resolve<TDependency>() 
            where TDependency : class
        {
            Type dependencyType = typeof(TDependency);

            if (recursionTypes.TryGetValue(Thread.CurrentThread.ManagedThreadId, out Stack<Type> types))
            {
                types.Clear();
            }
            else
            {
                recursionTypes[Thread.CurrentThread.ManagedThreadId] = new Stack<Type>();
            }

            return Resolve(dependencyType).OfType<TDependency>();
        }

        private IEnumerable<object> Resolve(Type dependency)
        {
            if (dependency.IsGenericType || dependency.IsGenericTypeDefinition)
            {
                return ResolveGeneric(dependency);
            }
            return ResolveNonGeneric(dependency);
        }

        private IEnumerable<object> ResolveGeneric(Type dependency)
        {
            List<object> result = new List<object>();
            IEnumerable<ImplementationContainer> implementationContainers 
                = configuration.GetImplementations(dependency)
                .Where(impl => !recursionTypes[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));

            object instance;
            foreach (ImplementationContainer implementationContainer in implementationContainers)
            {
                instance = CreateByConstructor(implementationContainer.ImplementationType.GetGenericTypeDefinition()
                    .MakeGenericType(dependency.GenericTypeArguments));

                if (instance != null)
                {
                    result.Add(instance);
                }
            }

            return result;
        }

        private IEnumerable<object> ResolveNonGeneric(Type dependency)
        {
            
            IEnumerable<ImplementationContainer> implementationContainers = 
                configuration.GetImplementations(dependency)
                .Where(impl => !recursionTypes[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));
            
            List<object> result = new List<object>();
            object dependencyInstance;

            foreach (ImplementationContainer container in implementationContainers)
            {
                if (container.ContainerLifetime == Lifetime.Singleton)
                {
                    if (container.SingletonInstance == null)
                    {
                        lock (container)
                        {
                            if (container.SingletonInstance == null)
                            {
                                container.SingletonInstance = CreateByConstructor(container.ImplementationType);
                            }
                        }
                    }
                    dependencyInstance = container.SingletonInstance;
                }
                else
                {
                    dependencyInstance = CreateByConstructor(container.ImplementationType);
                }

                if (dependencyInstance != null)
                {
                    result.Add(dependencyInstance);
                }
            }
            return result;
        }

        private object CreateByConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors()
                .OrderBy(constructor => constructor.GetParameters().Length).ToArray();
            object instance = null;
            recursionTypes[Thread.CurrentThread.ManagedThreadId].Push(type);

            for (int constructor = 0; constructor < constructors.Length && instance == null; ++constructor)
            {
                try
                {
                    List<object> parameters = new List<object>();
                    foreach (ParameterInfo constructorParameter in constructors[constructor].GetParameters())
                    {
                        parameters.Add(Resolve(constructorParameter.ParameterType));
                    }
                    instance = constructors[constructor].Invoke(parameters.ToArray());
                }
                catch
                {}
            }

            recursionTypes[Thread.CurrentThread.ManagedThreadId].Pop();
            return instance;
        }

        public DependencyProvider(DependenciesConfiguration configuration)
        {
            this.configuration = configuration;
            recursionTypes = new ConcurrentDictionary<int, Stack<Type>>();
        }
        
        
    }
}