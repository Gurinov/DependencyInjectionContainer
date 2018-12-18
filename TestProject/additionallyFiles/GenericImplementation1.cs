namespace TestProject.additionallyFiles
{
    public class GenericImplementation1<T> : IGenericInterface<T>
        where T: Interface1
    {
        public T field;

        public GenericImplementation1(T dep)
        {
            field = dep;
        }
    }
}