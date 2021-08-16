using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DIContainer
{
    private readonly List<ConstructorInfo> _objectGraph;
    private readonly Dictionary<Type, DependencyObject> _serviceDictionary;

    private static readonly object SyncRoot = new object();

    private readonly DIService _diService;

    public DIContainer(Dictionary<Type, DependencyObject> serviceDictionary, DIService diService)
    {
        _objectGraph = new List<ConstructorInfo>();
        _serviceDictionary = serviceDictionary;
        _diService = diService;
    }

    private object ResolveProperty(Type serviceType)
    {
        lock (SyncRoot)
        {
            var impl = DoResolve(serviceType);

            var properties = impl.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute(typeof(ResolveAttribute));

                if (attr == null) continue;

                var propertyImpl = DoResolve(property.PropertyType);
                property.SetValue(impl, propertyImpl);
            }

            return impl;
        }
    }

    private object ResolveField(Type serviceType)
    {
        lock (SyncRoot)
        {
            var impl = DoResolve(serviceType);

            var fields = impl.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute(typeof(ResolveAttribute));

                if (attr == null) continue;

                var fieldImpl = DoResolve(field.FieldType);
                field.SetValue(impl, fieldImpl);
            }

            return impl;
        }
    }

    private object ResolveMethod(Type serviceType)
    {
        lock (SyncRoot)
        {
            var impl = DoResolve(serviceType);

            var methods = impl.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute(typeof(ResolveAttribute));

                if (attr == null) continue;

                var parameters = method.GetParameters().Select(p => DoResolve(p.ParameterType)).ToArray();

                method.Invoke(impl, parameters);
            }

            return impl;
        }
    }

    private object DoResolve(Type serviceType)
    {
        lock (SyncRoot)
        {
            if (!_serviceDictionary.TryGetValue(serviceType, out var dependencyObject))
                throw new Exception($"Service of type {serviceType} is not found");

            if (dependencyObject.Implementation != null)
            {
                return dependencyObject.Implementation;
            }

            var actualType = dependencyObject.ImplementationType ?? dependencyObject.ServiceType;

            if (actualType.IsAbstract || actualType.IsInterface)
            {
                throw new Exception($"Cannot instantiate abstract classes or interfaces {actualType}");
            }

            if (actualType.GetConstructors().Length == 0)
            {
                throw new Exception($"{actualType} has no public constructors ");
            }

            var constructorInfo = actualType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length > 0);
            if (constructorInfo == null)
            {
                constructorInfo = actualType.GetConstructors().First();
            }
            try
            {
                if (_objectGraph.Any(c => c.GetHashCode() == constructorInfo.GetHashCode()))
                    throw new Exception($"{constructorInfo.DeclaringType} has circular dependency!");

                _objectGraph.Add(constructorInfo);

                var parameters = constructorInfo.GetParameters().Select(p => DoResolve(p.ParameterType)).ToArray();

                var implementation = Activator.CreateInstance(actualType, parameters);

                if (dependencyObject.LifeTime == ServiceLifeTime.Singleton)
                {
                    dependencyObject.Implementation = implementation;
                }

                _objectGraph.Clear();
                return implementation;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception($"{e.Message}");             
            }
        }
    }

    public object Resolve(Type serviceType)
    {
        lock (SyncRoot)
        {
            if (!_serviceDictionary.ContainsKey(serviceType) &&
                (!serviceType.IsAbstract && !serviceType.IsInterface))
            {
                _diService.Register(serviceType);
            }
        }

        return DoResolve(serviceType);
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    public T ResolveMethod<T>()
    {
        return (T)ResolveMethod(typeof(T));
    }

    public T ResolveProperty<T>()
    {
        return (T)ResolveProperty(typeof(T));
    }

    public T ResolveField<T>()
    {
        return (T)ResolveField(typeof(T));
    }

    public void Release<T>() where T : IDisposable
    {
        Release(typeof(T));
    }

    public void Release(Type type)
    {
        lock (SyncRoot)
        {
            if (!_serviceDictionary.TryGetValue(type, out var value)) return;
            if (value.Implementation == null) return;
            try
            {
                foreach (var service in _serviceDictionary)
                {
                    if (service.Value.Implementation == null) continue;
                    var fields = service.Value.Implementation.GetType()
                        .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        if (field.FieldType == type)
                        {
                            field.SetValue(service.Value.Implementation, null);
                        }
                    }
                }
                
                var impl = (IDisposable)value.Implementation;
                impl.Dispose();
                EventController.CheckSubscriber(impl);
                value.Implementation = null;
            }
            catch (Exception e) when (e is InvalidCastException)
            {
                throw new Exception($"{value.Implementation} does not implement IDisposable");
            }
            finally
            {
                value.Implementation = null;
            }
        }
    }

    public void ReleaseAll()
    {
        lock (SyncRoot)
        {
            foreach (var dependencyObject in _serviceDictionary)
            {
                if (dependencyObject.Value.Implementation == null || dependencyObject.Value.Implementation is MonoBehaviour)
                    continue;

                try
                {
                    var impl = (IDisposable)dependencyObject.Value.Implementation;
                    impl.Dispose();
                    dependencyObject.Value.Implementation = null;

                }
                catch (Exception e) when (e is InvalidCastException)
                {
                    throw new Exception($"{dependencyObject.Value.Implementation} does not implement IDisposable");
                }
            }
            _serviceDictionary.Clear();
        }

        _objectGraph.Clear();
    }
}