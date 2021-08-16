using System;

public class DependencyObject
{
    public Type ServiceType { get; }

    public Type ImplementationType { get; }

    public object Implementation { get; internal set; }

    public ServiceLifeTime LifeTime { get; }


    public DependencyObject(Type serviceType, object implementation)
    {
        Implementation = implementation;
        ServiceType = serviceType;
    }

    public DependencyObject(Type serviceType, ServiceLifeTime lifeTime)
    {
        ServiceType = serviceType;
        LifeTime = lifeTime;
    }

    public DependencyObject(Type serviceType, Type implementationType, ServiceLifeTime lifeTime)
    {
        ServiceType = serviceType;
        LifeTime = lifeTime;
        ImplementationType = implementationType;
    }
}
