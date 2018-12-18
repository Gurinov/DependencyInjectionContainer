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
   
        
    }
}