using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Debug = UnityEngine.Debug;

internal struct WeakDelegate
{
    public WeakReference Target { get; private set; }
    public string EventMessage { get; private set; }
    public string ObjectName { get; private set; }

    public WeakDelegate(WeakReference target, string eventMessage, string objectName)
    {
        Target = target;
        EventMessage = eventMessage;
        ObjectName = objectName;
    }
}

internal struct State
{
    public object Arg1 { get; private set; }
    public object Arg2 { get; private set; }
    public object Arg3{ get; private set; }
    public object Arg4{ get; private set; }
    public object Arg5 { get; private set; }

    public State(object arg1 = null, object arg2 = null, object arg3 = null, object arg4 = null, object arg5 = null)
    {
        Arg1 = arg1;
        Arg2 = arg2;
        Arg3 = arg3;
        Arg4 = arg4;
        Arg5 = arg5;
    }
}

public static class EventController
{
    private static readonly Dictionary<string, Delegate> _events = new Dictionary<string, Delegate>();
    private static readonly Dictionary<Delegate, WeakDelegate> _weakRefCache = new Dictionary<Delegate, WeakDelegate>();
    private static readonly Dictionary<string, State> _states = new Dictionary<string, State>();

    /// <summary>
    /// Checking if have all mono subscribers been unsubscribed
    /// </summary>
    public static void CheckMonoSubscribers()
    {
        foreach (var weakDelegate in _weakRefCache.ToList())
        {
            if (weakDelegate.Value.Target.IsAlive && weakDelegate.Value.Target.Target.ToString() == "null")
            {
                Debug.LogError($"Unsubscribed Method - {weakDelegate.Key.Method}, Object - {weakDelegate.Value.ObjectName}");
                Remove(weakDelegate.Value.EventMessage, weakDelegate.Key);
            }
        }
    }

    public static void CheckSubscriber(object impl)
    {
        foreach (var weakDelegate in _weakRefCache.ToList())
        {
            if (weakDelegate.Value.Target.Target == impl)
            {
                Debug.LogError($"Unsubscribed method {weakDelegate.Key.Method} - {weakDelegate.Value.Target.Target}");
                Remove(weakDelegate.Value.EventMessage, weakDelegate.Key);
                break;
            }
        }
    }

    #region Remove

    public static void RemoveAll()
    {
        _events.Clear();
        _weakRefCache.Clear();
    }


    private static void Remove(string eventMessage, Delegate method)
    {
        if (!_events.TryGetValue(eventMessage, out var value))
        {
            return;
        }

        try
        {
            if (_weakRefCache.ContainsKey(method))
            {
                _weakRefCache.Remove(method);
            }

            var d = Delegate.Remove(value, method);

            if (d != null)
                _events[eventMessage] = d;
            else
                _events.Remove(eventMessage);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Debug.LogError($"Remove! Signatures do not match: new - {method.Method} <> old - {value.Method}");
        }
    }

    public static void RemoveListener(string eventMessage, Action method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T>(string eventMessage, Action<T> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2>(string eventMessage, Action<T1, T2> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3>(string eventMessage, Action<T1, T2, T3> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3, T4>(string eventMessage, Action<T1, T2, T3, T4> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3, T4, T5>(string eventMessage, Action<T1, T2, T3, T4, T5> method)
    {
        Remove(eventMessage, method);
    }
    #endregion

    #region Add

    private static bool Add(string eventMessage, Delegate method)
    {
        if (!_events.TryGetValue(eventMessage, out var value))
        {
            _events[eventMessage] = method;
            _weakRefCache[method] = new WeakDelegate(new WeakReference(method.Target), eventMessage, method.Target.ToString());
        }
        else
        {
            try
            {
                if (value.GetInvocationList().Contains(method))
                {
                    Debug.LogError($"{method.Target} - {method.Method.Name} Already exist");
                    return true;
                }

                _weakRefCache[method] = new WeakDelegate(new WeakReference(method.Target), eventMessage, method.Target.ToString());
                _events[eventMessage] = Delegate.Combine(value, method);
            }
            catch (Exception e)
            {
                var methodParameters = method.GetMethodInfo().GetParameters();
                var valueParameters = value.GetMethodInfo().GetParameters();

                if (methodParameters.Length != valueParameters.Length)
                {
                    Debug.LogError($"Subscribe! Signatures do not match: new - {method.Method} <> old - {value.Method}");
                    return false;
                }
                
                for (var i = 0; i < valueParameters.Length; i++)
                {
                    if (methodParameters[i].ParameterType != valueParameters[i].ParameterType)
                    {
                        Debug.LogError($"Subscribe! Signatures do not match: new - {method.Method} <> old - {value.Method}");
                        return false;
                    }
                }
                
                Debug.LogError(e);
                return false;
            }
        }

        return true;
    }

    public static void AddListener(string eventMessage, Action method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke();
            }
        }
    }

    public static void AddListener<T>(string eventMessage, Action<T> method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke((T)_states[eventMessage].Arg1);
            }
        }
    }

    public static void AddListener<T1, T2>(string eventMessage, Action<T1, T2> method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke(
                    (T1)_states[eventMessage].Arg1, 
                    (T2)_states[eventMessage].Arg2);
            }
        }
    }

    public static void AddListener<T1, T2, T3>(string eventMessage, Action<T1, T2, T3> method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke(
                    (T1)_states[eventMessage].Arg1, 
                    (T2)_states[eventMessage].Arg2, 
                    (T3)_states[eventMessage].Arg3);
            }
        }
    }

    public static void AddListener<T1, T2, T3, T4>(string eventMessage, Action<T1, T2, T3, T4> method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke(
                    (T1)_states[eventMessage].Arg1, 
                    (T2)_states[eventMessage].Arg2,
                    (T3)_states[eventMessage].Arg3,
                    (T4)_states[eventMessage].Arg4);
            }
        }
    }

    public static void AddListener<T1, T2, T3, T4, T5>(string eventMessage, Action<T1, T2, T3, T4, T5> method, bool instantNotify = false)
    {
        if (Add(eventMessage, method))
        {
            if (instantNotify && _states.ContainsKey(eventMessage))
            {
                method?.Invoke(
                    (T1)_states[eventMessage].Arg1,
                    (T2)_states[eventMessage].Arg2,
                    (T3)_states[eventMessage].Arg3,
                    (T4)_states[eventMessage].Arg4,
                    (T5)_states[eventMessage].Arg5);
            }
        }
    }
    #endregion

    #region Invoke
    public static void Invoke(string eventMessage)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action)value;
            _states[eventMessage] = new State();
            action?.Invoke();
        }
        catch (Exception e)
        {
            if (value.GetMethodInfo().GetParameters().Length > 0)
            {
                Debug.LogError($"Invoke! Signatures do not match: {value.Method}");
                return;
            }

            Debug.LogError(e);
        }
    }

    public static void Invoke<T>(string eventMessage, T arg)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T>)value;
            _states[eventMessage] = new State(arg);
            action?.Invoke(arg);
        }
        catch (Exception e)
        {
            var valueParameters = value.GetMethodInfo().GetParameters();

            if (valueParameters[0].ParameterType != arg.GetType() || valueParameters.Length < 1)
            {
                Debug.LogError($"Invoke! Signatures do not match: incoming - {arg.GetType()} <> source - {value.Method}");
                return;
            }
            

            Debug.LogError(e);
        }
    }

    public static void Invoke<T1, T2>(string eventMessage, T1 arg1, T2 arg2)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2>)value;
            _states[eventMessage] = new State(arg1, arg2);
            action?.Invoke(arg1, arg2);
        }
        catch (Exception e)
        {
            var valueParameters = value.GetMethodInfo().GetParameters();
            var args = new object[] { arg1, arg2 };

            if (valueParameters.Length != args.Length)
            {
                Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} <> source - {value.Method}");
                return;
            }

            for (var i = 0; i < valueParameters.Length; i++)
            {
                if (valueParameters[i].ParameterType != args[i].GetType())
                {
                    Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} <> source - {value.Method}");
                    return;
                }
            }

            Debug.LogError(e);
        }
    }

    public static void Invoke<T1, T2, T3>(string eventMessage, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3>)value;
            _states[eventMessage] = new State(arg1, arg2, arg3);
            action?.Invoke(arg1, arg2, arg3);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            var valueParameters = value.GetMethodInfo().GetParameters();
            var args = new object[] { arg1, arg2, arg3};

            if (valueParameters.Length != args.Length)
            {
                Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} <> source - {value.Method}");
                return;
            }

            for (var i = 0; i < valueParameters.Length; i++)
            {
                if (valueParameters[i].ParameterType != args[i].GetType())
                {
                    Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} <> source - {value.Method}");
                    return;
                }
            }

            Debug.LogError(e);
        }
    }

    public static void Invoke<T1, T2, T3, T4>(string eventMessage, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3, T4>)value;
            _states[eventMessage] = new State(arg1, arg2, arg3, arg4);
            action?.Invoke(arg1, arg2, arg3, arg4);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            var valueParameters = value.GetMethodInfo().GetParameters();
            var args = new object[] { arg1, arg2, arg3, arg4 };

            if (valueParameters.Length != args.Length)
            {
                Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} {arg4.GetType()} <> source - {value.Method}");
                return;
            }

            for (var i = 0; i < valueParameters.Length; i++)
            {
                if (valueParameters[i].ParameterType != args[i].GetType())
                {
                    Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} {arg4.GetType()} <> source - {value.Method}");
                    return;
                }
            }

            Debug.LogError(e);
        }
    }

    public static void Invoke<T1, T2, T3, T4, T5>(string eventMessage, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3, T4, T5>)value;
            _states[eventMessage] = new State(arg1, arg2, arg3, arg4, arg5);
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            var valueParameters = value.GetMethodInfo().GetParameters();
            var args = new object[] { arg1, arg2, arg3, arg4, arg5 };

            if (valueParameters.Length != args.Length)
            {
                Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} {arg4.GetType()} {arg5.GetType()} <> old - {value.Method}");
                return;
            }

            for (var i = 0; i < valueParameters.Length; i++)
            {
                if (valueParameters[i].ParameterType != args[i].GetType())
                {
                    Debug.LogError($"Invoke! Signatures do not match: incoming - {arg1.GetType()} {arg2.GetType()} {arg3.GetType()} {arg4.GetType()} {arg5.GetType()} <> old - {value.Method}");
                    return;
                }
            }

            Debug.LogError(e);
        }
    }
    #endregion
}
