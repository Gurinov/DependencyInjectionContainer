using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainer.Model;
using NUnit.Framework;
using TestProject.additionallyFiles;

namespace TestProject
{
    [TestFixture]
    public class Tests
    {
        
        DependenciesConfiguration config;
        DependencyProvider provider;

        [SetUp]
        public void SetUp()
        {
            config = new DependenciesConfiguration();
        }

        [Test]
        public void RegisterTests()
        {
            config.Register<Interface1, Class1>();
            config.Register<Interface2, Class3>();
            
            var registeredImplementations = config.GetImplementations(typeof(Interface1)).ToList();
            Assert.AreEqual(1, registeredImplementations.Count);
            Assert.AreEqual(registeredImplementations.First().ImplementationType, typeof(Class1));
            
            registeredImplementations = config.GetImplementations(typeof(Interface2)).ToList();
            Assert.AreEqual(1, registeredImplementations.Count);
            Assert.AreEqual(registeredImplementations.First().ImplementationType, typeof(Class3));
        }
        
        [Test]
        public void MultipleImplementationsForOneDependencyTest()
        {
            config.Register<Interface1, Class1>();
            config.Register<Interface1, Class2>();
            
            var registeredImplementations = config.GetImplementations(typeof(Interface1)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);
            Assert.AreEqual(registeredImplementations[0].ImplementationType, typeof(Class1));
            Assert.AreEqual(registeredImplementations[1].ImplementationType, typeof(Class2));
        }
        
        [Test]
        public void OpenGenericTypeRegisterTest()
        {
            config.Register(typeof(IGenericInterface<>), typeof(GenericImplementation1<>));
            config.Register(typeof(IGenericInterface<>), typeof(GenericImplementation2<>));
            
            var registeredImplementations = config.GetImplementations(typeof(IGenericInterface<>)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);
            Assert.AreEqual(registeredImplementations[0].ImplementationType, typeof(GenericImplementation1<>));
            Assert.AreEqual(registeredImplementations[1].ImplementationType, typeof(GenericImplementation2<>));
            
        }
        
        [Test]
        public void OpenGenericTypeProviderTest()
        {
            config.Register(typeof(IGenericInterface<>), typeof(GenericImplementation4<>));
            config.Register<Interface1, Class1>();
            
            provider = new DependencyProvider(config);

            Assert.AreEqual(provider.Resolve<IGenericInterface<Interface1>>().Count(), 1);
            Assert.AreEqual(provider.Resolve<IGenericInterface<Interface1>>().First().GetType(), typeof(GenericImplementation4<Interface1>));
            
        }
        
        [Test]
        public void ResolveTest()
        {
            config.Register<Interface1, Class1>();
            config.Register<Interface1, Class2>();
            
            provider = new DependencyProvider(config);

            var interface1 = provider.Resolve<Interface1>();
            
            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(Class1),
                typeof(Class2)
            };
            CollectionAssert.AreEquivalent(expectedInstancesTypes,
                interface1.Select(instance => instance.GetType()).ToList());
        }
        
        [Test]
        public void SingletonResolveTest()
        {
            config.Register<Interface1, Class1>(Lifetime.Singleton);
            provider = new DependencyProvider(config);

            Assert.AreEqual(provider.Resolve<Interface1>().First(), provider.Resolve<Interface1>().First());
        }
        
        [Test]
        public void RecursionTest()
        {
            config.Register<IRecursion1, RecursionClass>();
            config.Register<IRecursion2, RecursionClass2>();
            provider = new DependencyProvider(config);

            Assert.AreEqual(provider.Resolve<IRecursion2>().Count(), 1);
        }
        
    }
}