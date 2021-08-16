public class ServiceProvider : IServiceProvider
{
    private readonly DIContainer _container;

    public ServiceProvider(DIContainer container)
    {
        _container = container;
    }

    public T GetService<T>()
    {
        return _container.Resolve<T>();
    }

    public void Dispose()
    {
    }
}
