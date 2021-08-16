using System;
using System.Collections.Generic;

public class DIService
{
    private readonly Dictionary<Type, DependencyObject> _serviceDictionary;

    private static readonly object SyncRoot = new object();

    public DIService()
    {
        _serviceDictionary = new Dictionary<Type, DependencyObject>();
    }

    /// <summary>
    /// Store an instance from an implemented class
    /// </summary>
    public void RegisterInstance<TService>(TService implementation)
    {
        lock (SyncRoot)
        {
            if (_serviceDictionary.TryGetValue(typeof(TService), out var value))
            {
                value.Implementation = implementation;
                return;
            }

            _serviceDictionary[typeof(TService)] = new DependencyObject(typeof(TService), implementation);
        }
    }

    /// <summary>
    /// Create an instance from class without NEW keyword
    /// </summary>
    public void Register<TService>(ServiceLifeTime serviceLifeTime = ServiceLifeTime.Singleton)
    {
        lock (SyncRoot)
        {
            if (_serviceDictionary.TryGetValue(typeof(TService), out var value))
                throw new Exception($"Already exist {value.ServiceType}");

            _serviceDictionary[typeof(TService)] = new DependencyObject(typeof(TService), serviceLifeTime);
        }
    } 
    
    /// <summary>
    /// Create an instance from class without NEW keyword
    /// </summary>
    public void Register(Type type, ServiceLifeTime serviceLifeTime = ServiceLifeTime.Singleton)
    {
        lock (SyncRoot)
        {
            if (_serviceDictionary.TryGetValue(type, out var value))
                throw new Exception($"Already exist {value.ServiceType}");

            _serviceDictionary[type] = new DependencyObject(type, serviceLifeTime);
        }
    }

    public void Register<TService, TImplementation>(ServiceLifeTime serviceLifeTime = ServiceLifeTime.Singleton) where TImplementation : class, TService 
    {
        lock (SyncRoot)
        {
            if (_serviceDictionary.TryGetValue(typeof(TService), out var value))
                throw new Exception($"Already exist {value.ServiceType}");

            _serviceDictionary[typeof(TService)] =
                new DependencyObject(typeof(TService), typeof(TImplementation), serviceLifeTime);
        }
    }

    public DIContainer GenerateContainer()
    {
        lock (SyncRoot)
        {
            return new DIContainer(_serviceDictionary, this);
        }
    }
}
