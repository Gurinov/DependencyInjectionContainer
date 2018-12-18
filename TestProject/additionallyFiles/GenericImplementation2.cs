namespace TestProject.additionallyFiles
{
    public class GenericImplementation2<T> : IGenericInterface<T>
        where T : Interface2
    {
        public T field;

        public GenericImplementation2(T dep)
        {
            field = dep;
        }
    }
}