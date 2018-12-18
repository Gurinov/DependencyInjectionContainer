namespace TestProject.additionallyFiles
{
    public class GenericImplementation4<T> : IGenericInterface<T>
    {
        public T field;

        public GenericImplementation4(T dep)
        {
            field = dep;
        }
        
        public GenericImplementation4()
        {
        }
        
    }
}