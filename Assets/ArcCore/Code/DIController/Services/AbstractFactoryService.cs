
using System;

public abstract class AbstractFactoryService<T> : IFactoryService<T>
{
    private readonly DIContainer _container;

    protected AbstractFactoryService(DIContainer container)
    {
        _container = container;
    }

    public virtual T GetService(Type type)
    {
        return (T)_container.Resolve(type);
    }

    public virtual void Dispose()
    {
    }
}
