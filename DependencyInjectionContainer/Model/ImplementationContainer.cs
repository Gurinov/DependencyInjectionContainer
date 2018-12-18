using System;

namespace DependencyInjectionContainer.Model
{
    
    public class ImplementationContainer
    {

        public Type ImplementationType
        {
            get;
        }

        public Lifetime ContainerLifetime
        {
            get;
        }

        public object SingletonInstance
        {
            get; set;
        }

        public ImplementationContainer(Type implementationType, Lifetime lifetime)
        {
            ImplementationType = implementationType;
            ContainerLifetime = lifetime;
            SingletonInstance = null;
        }
    }
}