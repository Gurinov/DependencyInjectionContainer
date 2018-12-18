namespace TestProject.additionallyFiles
{
    public class GenericImplementation3<T> : IGenericInterface<T>
        where T: Interface1
    {
        public T field;

        public GenericImplementation3(T dep)
        {
            field = dep;
        }
    }
}